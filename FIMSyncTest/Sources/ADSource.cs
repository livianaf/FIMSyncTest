using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Drawing;
using System.DirectoryServices;
using FIMSyncTest.Helpers;
using System.Windows.Forms;

namespace FIMSyncTest.Sources {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    class ADSource : MarshalByRefObject, IDataSource, IDisposable  {
        private DataRow Row;
        private string adPath;
        private DirectoryEntry rootDE;
        private DataTable tAttributes;
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public string Name { private set; get; }
        public string Type { private set; get; }
        public string Rol { private set; get; }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public void Dispose() {
            rootDE.Dispose();
            tAttributes.Dispose();
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public ADSource(DataRow Row, DataTable tAttributes) {
            this.Row = Row;
            this.tAttributes = tAttributes;
            this.Name = Row["Alias"].ToString();
            this.Rol = Row["Rol"].ToString();
            this.Type = Row["Type"].ToString();
            adPath = "LDAP://" + Row["Server"].ToString() + ":" + Row["Port"].ToString();
            string subsetRoot = Row["Base"].ToString();
            AuthenticationTypes vAt;
            LogHelper.Msg(adPath + "/" + subsetRoot + " - [" + Row["Connection"].ToString() + "]");
            if (Row["Connection"].ToString() == "" || !Enum.TryParse<AuthenticationTypes>(Row["Connection"].ToString(), true, out vAt))
                this.rootDE = new DirectoryEntry(adPath + "/" + subsetRoot, Row["Usr"].ToString(), Row["Pwd"].ToString());
            else
                this.rootDE = new DirectoryEntry(adPath + "/" + subsetRoot, Row["Usr"].ToString(), Row["Pwd"].ToString(), vAt);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void AddPropertiesToLoad(DirectorySearcher oDs, string sOtherAttribs = "") {
            for (int i = 0; i < tAttributes.Rows.Count; i++) {
                string sUsage = tAttributes.Rows[i]["Usage"].ToString();
                if (sUsage.Contains("Link") || sUsage.Contains("ID") || sUsage.Contains("Alias")) oDs.PropertiesToLoad.Add(tAttributes.Rows[i][Row["Alias"].ToString()].ToString());
                }
            if (sOtherAttribs != "") {
                foreach (string s in sOtherAttribs.Split(';')) {
                    oDs.PropertiesToLoad.Add(s); 
                    }
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private Dictionary<string, string> AddPropertiesLoaded(SearchResult r, bool bAllAtribs = false, string sOtherAttribs = "") {
            Dictionary<string, string> salida = new Dictionary<string, string>();
            for (int i = 0; i < tAttributes.Rows.Count; i++) {
                string sUsage = tAttributes.Rows[i]["Usage"].ToString();
                string name = tAttributes.Rows[i][Row["Alias"].ToString()].ToString();
                string alias = tAttributes.Rows[i]["Alias"].ToString();
                if (bAllAtribs || (sUsage.Contains("Link") || sUsage.Contains("ID") || sUsage.Contains("Alias") || sOtherAttribs.Contains(name))) {
                    if (r.Properties[name].Count == 0)
                        salida.Add(alias, name == "" ? "" : "<ND>");
                    if (r.Properties[name].Count == 1 && sUsage.Contains("Jpeg") && !sUsage.Contains("Guid")) {
                        salida.Add(alias, BitConverter.ToString(r.Properties[name][0] as byte[]).Replace("-", "").Substring(0, 30));
                        using (MemoryStream ms = new MemoryStream(r.Properties[name][0] as byte[])) {
                            Bitmap.FromStream(ms).Save(Config.PathImages + this.Name + "_" + alias + "_0.jpg");
                            }
                        }
                    if (r.Properties[name].Count == 1 && !sUsage.Contains("Jpeg") && !sUsage.Contains("Guid"))
                        salida.Add(alias, r.Properties[name][0].ToString());
                    if (r.Properties[name].Count == 1 && sUsage.Contains("Guid"))
                        salida.Add(alias, name == "objectGUID" ? ObjectsHelper.ConvertGuidToString((byte[])r.Properties[name][0]) : r.Properties[name][0].ToString());
                    if (r.Properties[name].Count == 1 && sUsage.Contains("ID") && !sUsage.Contains("Guid"))
                        salida.Add("#ID#", r.Properties[name][0].ToString());
                    if (r.Properties[name].Count > 1) {
                        string s = "";
                        for (int j = 0; j < r.Properties[name].Count; j++) {
                            if (sUsage.Contains("Jpeg")) {
                                s += (s == "" ? "" : Config.Separator) + BitConverter.ToString(r.Properties[name][j] as byte[]).Replace("-", "").Substring(0, 30);
                                using (MemoryStream ms = new MemoryStream(r.Properties[name][j] as byte[])) {
                                    Bitmap.FromStream(ms).Save(Config.PathImages + this.Name + "_" + alias + "_" + j + ".jpg");
                                    }
                                }
                            else
                                s += (s == "" ? "" : Config.Separator) + r.Properties[name][j].ToString();
                            }
                        salida.Add(alias, s);
                        }
                    }
                }
            return salida;
            }
        //_________________________________________________________________________________________________________
        // Get more than 1000 elements: http://stackoverflow.com/questions/10853011/get-2000-of-6000-records-from-directorysearcher
        //_________________________________________________________________________________________________________
        public Dictionary<string, Dictionary<string, string>> FindAllEntries(int limit = 0, string sOtherAttribs = "", string sAdditionalFilter = "") {
            Dictionary<string, Dictionary<string, string>> salida = new Dictionary<string, Dictionary<string, string>>();
            DirectorySearcher oDs = new DirectorySearcher(rootDE);
            SearchResultCollection results = null;
            oDs.SearchScope = SearchScope.Subtree;
            oDs.PageSize = 1000;
            if (limit > 0) oDs.SizeLimit = limit;
            string sFilter = "(|(objectClass=user)(objectClass=contact)(objectClass=group)(objectClass=msExchDynamicDistributionList)(objectClass=DSObject))";
            if (sAdditionalFilter != "" && Config.ObjectAliasAttribute != "") sFilter = "(&(" + Config.ObjectAliasAttribute + "=" + sAdditionalFilter + ")" + sFilter + ")";
            oDs.Filter = sFilter;
            AddPropertiesToLoad(oDs, sOtherAttribs);

            try { results = oDs.FindAll(); LogHelper.Msg(); }
            catch (Exception ex) { oDs.Dispose(); LogHelper.Msg(ex.Message); return salida; }

            foreach (SearchResult r in results) {
                string resultDN = r.Properties["distinguishedName"][0].ToString();
                string resultCN = r.Properties["cn"][0].ToString();
                if (!resultDN.Equals(rootDE.Properties["distinguishedname"][0]))
                    salida.Add("" + salida.Count, AddPropertiesLoaded(r, false, sOtherAttribs));
                }
            oDs.Dispose();
            return salida;
            }
        //_________________________________________________________________________________________________________
        // Set iMax !=1 to return data of all instances found
        // Only one object must be found in AD source with the same CN in data[0][lista atributos]
        //_________________________________________________________________________________________________________
        public Dictionary<string, Dictionary<string, string>> FindAllAttributes(string cn, bool bOnlyOne = true) {
            Dictionary<string, Dictionary<string, string>> salida = new Dictionary<string, Dictionary<string, string>>();
            DirectorySearcher oDs = new DirectorySearcher(rootDE);
            oDs.SearchScope = SearchScope.Subtree;
            oDs.PageSize = 1000;
            oDs.Filter = "(cn=" + cn + ")";
            oDs.PropertiesToLoad.Add("*");
            oDs.PropertiesToLoad.Add("+");
            SearchResultCollection results = oDs.FindAll();
            if (results.Count > 1 && bOnlyOne) {
                MessageBox.Show("There are " + results.Count + " objects with the same CN [" + cn + "]. It is not possible in LAB. Only first one is showed.", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            //delete image files
            (new DirectoryInfo(Config.PathImages)).GetFiles(this.Name + "_*.jpg").ToList().ForEach(f => f.Delete());

            foreach (SearchResult r in results) {
                string resultDN = r.Properties["distinguishedName"][0].ToString();
                string resultCN = r.Properties["cn"][0].ToString();
                if (!resultDN.Equals(rootDE.Properties["distinguishedname"][0]))
                    salida.Add("" + salida.Count, AddPropertiesLoaded(r, true));
                }
            oDs.Dispose();
            return salida;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public bool DeleteAllEntries(string lTables = null) { return false; }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public bool DeleteEntry(string cn) {
            if (Row["Writable"].ToString().ToLower() != "yes") {
                if (Row["Writable"].ToString().ToLower() == "ask") {
                    if (MessageBox.Show("Content will be modified. \nAre you sure?", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) 
                        return false;
                    }
                else return false; 
                }
            Dictionary<string, Dictionary<string, string>> salida = new Dictionary<string, Dictionary<string, string>>();
            DirectorySearcher oDs = new DirectorySearcher(rootDE);
            oDs.SearchScope = SearchScope.Subtree;
            oDs.PageSize = 1000;
            oDs.Filter = "(cn=" + cn + ")";
            oDs.PropertiesToLoad.Add("distinguishedName");
            oDs.PropertiesToLoad.Add("+");
            SearchResultCollection results = oDs.FindAll();
            if (results.Count > 1) {
                MessageBox.Show("There are " + results.Count + " objects with the same CN [" + cn + "]. It is not possible in LAB. Firstly you have to delete one of them manually.", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                oDs.Dispose();
                return false;
                }
            foreach (SearchResult r in results) {
                string resultDN = r.Properties["distinguishedName"][0].ToString();
                DirectoryEntry de = LoadEntry(resultDN);
                DirectoryEntry dp = de.Parent;
                try { dp.Children.Remove(de); }
                catch (Exception ex) { LogHelper.Msg(ex.Message); }
                }
            oDs.Dispose();
            return true;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private int AtrByAlias(string AtrAlias) {
            for (int i = 0; i < tAttributes.Rows.Count; i++) {
                if ( tAttributes.Rows[i]["Alias"].ToString().ToLower() == AtrAlias.ToLower() ) return i;
                }
            return 0;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public string UpdateAttributeValue(string Id, string Column, string AtrAlias, string NewValue, string OldValue) {
            if (Row["Writable"].ToString().ToLower() != "yes") return OldValue; 
            DirectoryEntry entry = LoadEntry(Id);
            string AtrName = tAttributes.Rows[AtrByAlias(AtrAlias)][Column].ToString();
            string sMV = tAttributes.Rows[AtrByAlias(AtrAlias)]["MV"].ToString();
            string sUsage = tAttributes.Rows[AtrByAlias(AtrAlias)]["Usage"].ToString();
            try {
                if (sUsage.Contains("Jpeg")) {
                    if (NewValue == "")
                        entry.Properties[AtrName].Clear();
                    else
                        LogHelper.Msg("Attribute non-editable");
                    }
                else if (sMV == "N") {
                    if (NewValue == "")
                        entry.Properties[AtrName].Clear();
                    else
                        entry.Properties[AtrName].Value = NewValue;
                    }
                else {
                    if (NewValue == "")
                        entry.Properties[AtrName].Clear();
                    else
                        entry.Properties[AtrName].Value = NewValue.Split(Config.SeparatorC);
                    }

                entry.CommitChanges();
                LogHelper.Msg();
                return NewValue;
                }
            catch (Exception ex) {
                LogHelper.Msg(ex.Message);
                return OldValue;
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private DirectoryEntry LoadEntry(string entryDN) {
            DirectoryEntry objectDE = new DirectoryEntry(adPath + "/" + entryDN, Row["Usr"].ToString(), Row["Pwd"].ToString());
            try {
                Guid temp = objectDE.Guid;
                // entryDN exists
                return objectDE;
                }
            catch (Exception) {
                // entryDN does not exists
                return null;
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public string CreateObject(Dictionary<string, string> data) {
            if (Row["Writable"].ToString().ToLower() != "yes") return "";
            string sPath = "", objclass = "", objclassID = "";
            
            ExtensionHelper.GetDataForADObjectCreation(this.Rol, this.Name, Config.dConfig, data, ref sPath, ref objclass, ref objclassID);

            sPath += Row["Base"].ToString();
            DirectoryEntry parentDE = new DirectoryEntry(adPath + "/" + sPath, Row["Usr"].ToString(), Row["Pwd"].ToString());
            DirectoryEntry newObjDE = null;
            LogHelper.Msg(objclass + "|" + "CN=" + data["cn"] + "|" + adPath + "/" + sPath); 
            try { newObjDE = parentDE.Children.Add("CN=" + data["cn"], objclass); }
            catch (Exception ex) { LogHelper.Msg("CreateObject: " + ex.Message); MessageBox.Show(ex.Message + "\n" + parentDE.Path, "CreateObject", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); return ""; }

            // Mandatory attributes
            for (int i = 0; i < tAttributes.Rows.Count; i++) {
                string sUsage = tAttributes.Rows[i]["Usage"].ToString();
                string Tipo = tAttributes.Rows[i]["Type"].ToString();
                string sAtrName = tAttributes.Rows[i][this.Name].ToString();
                string sAtrAlias = tAttributes.Rows[i]["Alias"].ToString();

                if (Tipo.Contains(objclassID) && sUsage.Contains("Mandatory") && sAtrAlias != "" && sAtrName != "" && data.ContainsKey(sAtrAlias)) {
                    newObjDE.Properties[sAtrName].Add(data[sAtrAlias]);
                    }
                }
            newObjDE.CommitChanges();
            // Other additional attributes
            Dictionary<string, string> additionalAttributes = ExtensionHelper.SetAdditionalAttributesToNewADObject(this.Rol, this.Name, Config.dConfig, data, sPath, objclass, objclassID);
            foreach (string s in additionalAttributes.Keys) {
                bool retry = true;
                int cont = 0;
                string sValue = additionalAttributes[s];
                string sAtrName = tAttributes.Rows[AtrByAlias(s)][Name].ToString();
                sAtrName = sAtrName == "cn" || sAtrName == "" ? s : sAtrName;//Si es vacío no está mapeado pero debería estarlo. No puede ser 'cn' ya que es de los principales a asignar por lo que significa que no lo ha encontrado en la lista
                while ( retry ) {
                    newObjDE.Properties[sAtrName].Clear();
                    newObjDE.Properties[sAtrName].Add(sValue);
                    try { newObjDE.CommitChanges(); retry = false; }
                    catch ( Exception ex ) {
                        LogHelper.Msg("Error setting [" + sAtrName + "] with [" + sValue + "]: "+ ex.Message);
                        sValue = additionalAttributes[s] + "X" + cont++;
                        if ( ex.Message.Contains("already exist") ) { retry = true; }
                        if ( ex.Message.Contains("constraint violation") ) { retry = true; }
                        }
                    }
                }

            newObjDE.CommitChanges();
            byte[] temp = (byte[])newObjDE.Properties["objectGUID"].Value;
            string sGuid = ObjectsHelper.ConvertGuidToString(temp);

            // Optional attributes
            for (int i = 0; i < tAttributes.Rows.Count; i++) {
                string sUsage = tAttributes.Rows[i]["Usage"].ToString();
                string Tipo = tAttributes.Rows[i]["Type"].ToString();
                string sAtrName = tAttributes.Rows[i][this.Name].ToString();
                string sAtrAlias = tAttributes.Rows[i]["Alias"].ToString();
                if (sAtrName == "objectGUID") continue;
                if (Tipo.Contains(objclassID) &&                      // If attribute is for created class 
                    !sUsage.Contains("Mandatory") &&                  // If attribute is not mandatory (mandatory attr previously processed)
                    !sUsage.Contains("Auto") &&                       // If attribute is not auto-filled
                    sAtrName != "" &&                                 // If attribute has a valid name for this directory
                    data.ContainsKey(sAtrAlias) &&                    // If attribute has a value
                    data[sAtrAlias].Replace("<ND>", "") != "") {      // If attribute has a valid value
                    if (sUsage.Contains("Jpeg")) {
                        string[] lNames = data[sAtrAlias].Split(Config.SeparatorC);
                        newObjDE.Properties[sAtrName].Clear();
                        foreach (string sName in lNames) {
                            if (System.IO.File.Exists(sName)) {
                                newObjDE.Properties[sAtrName].Add(System.IO.File.ReadAllBytes(sName));
                                }
                            }
                        }
                    else if (tAttributes.Rows[i]["MV"].ToString() == "N") {
                        // newObjDE.Properties[sAtrName].Add(data[sAtrAlias]); // Do not use this way: you get an error message with numeric attributes
                        newObjDE.Properties[sAtrName].Value = data[sAtrAlias]; // This way is always valid
                        }
                    else if (tAttributes.Rows[i]["MV"].ToString() == "B") {
                        if (data[sAtrAlias].ToLower() == "true")
                            newObjDE.Properties[sAtrName].Add(true);
                        if (data[sAtrAlias].ToLower() == "false")
                            newObjDE.Properties[sAtrName].Add(false);
                        }
                    else {
                        newObjDE.Properties[sAtrName].Value = ((object[])(data[sAtrAlias].Split(Config.SeparatorC)));
                        }
                    LogHelper.Msg("Adding " + sAtrName);
                    try { newObjDE.CommitChanges(); }
                    catch (Exception ex) { LogHelper.Msg("Adding Attribute: " + sAtrName + " in " + this.Name + " Error :" + ex.Message); MessageBox.Show("Adding Attribute: " + sAtrName + " in " + this.Name + "\nError :" + ex.Message, "CreateObject", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
                    }
                }
            if (objclass == "user" && this.Rol != "AI") {
                LogHelper.Msg("Enabling User");
                EnableUser(newObjDE, data); 
                }
            return sGuid;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void EnableUser(DirectoryEntry newObjDE, Dictionary<string, string> data) {
            const int ADS_UF_DONT_EXPIRE_PASSWD = 0x10000;
            const int ADS_UF_ACCOUNTDISABLE = 2;        // 0x2
            int val;
            // password
            try {
                newObjDE.AuthenticationType = AuthenticationTypes.Secure;
                newObjDE.Invoke("SetPassword", data["#password"]);
                }
            catch (Exception ex) {
                LogHelper.Msg("Error in Invoke('SetPassword'): " + ex.InnerException.Message); 
                }
            // password doesn't expire
            val = (int)newObjDE.Properties["userAccountControl"].Value;
            val = val | ADS_UF_DONT_EXPIRE_PASSWD;
            // enabled user account
            val = val & ~ADS_UF_ACCOUNTDISABLE;
            newObjDE.Properties["userAccountControl"].Value = val;
            newObjDE.CommitChanges();
            }

        }
    }
