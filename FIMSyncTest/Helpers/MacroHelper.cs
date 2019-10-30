using System;
using System.Collections.Generic;
using System.Linq;
using CSScriptLibrary;
using System.Windows.Forms;
using System.IO;
using FIMSyncTest.Profiles;

namespace FIMSyncTest {
    //_________________________________________________________________________________________________________
    // Interface of each macro 
    //_________________________________________________________________________________________________________
    public interface IFIMSyncMacro {
        void EntryScript(MacroHelper mh, string sParam = "");
        void LastExecTerminated(MacroHelper mh, string sValue, string sParam = "");
        void EndScript(MacroHelper mh, string sParam = "");
        }
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    public class MacroHelper : MarshalByRefObject {
        //_________________________________________________________________________________________________________
        #region Fill the combo with the list of macro file names
        //_________________________________________________________________________________________________________
        internal static void LoadMacroList(ToolStripComboBox cLstMacros) {
            string[] filePaths = Directory.GetFiles(Config.PathMacros, "*.cs");
            string sLastValue = cLstMacros.Text;
            cLstMacros.Items.Clear();
            foreach (string s in filePaths) cLstMacros.Items.Add(Path.GetFileNameWithoutExtension(s));
            if (cLstMacros.Items.Count > 0) {
                cLstMacros.SelectedIndex = 0;
                for (int i = 0; i < cLstMacros.Items.Count; i++)
                    if (cLstMacros.Items[i].ToString() == sLastValue) cLstMacros.SelectedIndex = i;
                }
            ChangeListBoxWidth(cLstMacros);
            }
        //_________________________________________________________________________________________________________
        internal static string GetSelectedMacroPath(ToolStripComboBox cLstMacros) {
            return Config.PathMacros + cLstMacros.Text + ".cs";
            }
        //_________________________________________________________________________________________________________
        internal static string GetSelectedMacroPath(string sMacroName) {
            return Config.PathMacros + sMacroName + ".cs";
            }
        //_________________________________________________________________________________________________________
        internal static bool Running { set; get; }
        internal static bool MustCallBack { set; get; }
        internal static string NextScriptName { set; get; }
        internal static string ExecName { set; get; }
        #endregion
        //_________________________________________________________________________________________________________
        #region private properties
        private fFIMSyncTest f { set; get; }
        private DataGridView dg { set; get; }
        private string currentMacro { set; get; }
        private string currentMacroParam { set; get; }
        private List<string> pendingMacros { set; get; }
        bool Debug = true;
        bool Info = true;
        bool Warning = true;
        bool Error = true;
        #endregion
        //_________________________________________________________________________________________________________
        #region Constructor 
        //_________________________________________________________________________________________________________
        // Initialize helper with commonly used resources (form object and grid object) Other objects are accesible from there
        internal MacroHelper(fFIMSyncTest pf, DataGridView pdg) { f = pf; dg = pdg; currentMacro = ""; currentMacroParam = ""; pendingMacros = new List<string>(); }
        #endregion
        // Available methods to each macro macro __________________________________________________________________
        #region Menu Load Data
        //_________________________________________________________________________________________________________
        public void LoadData() { LogDebug(string.Format("LoadData()")); f.mLoadData_Click(null, null); }
        //_________________________________________________________________________________________________________
        public bool SetMaxLoadItems(int iNum) {
            if (iNum < 0) { LogError(string.Format("SetMaxLoadItems({0})", iNum)); return false; }
            f.cMax.Text = "" + iNum;
            LogDebug(string.Format("SetMaxLoadItems({0})", iNum));
            return true;
            }
        //_________________________________________________________________________________________________________
        public bool SetFilter(string sFilter) {
            LogDebug(string.Format("SetFilter({0})", sFilter)); 
            f.cFilter.Text = sFilter;
            return true;
            }
        //_________________________________________________________________________________________________________
        public string GetFilter() { LogDebug(string.Format("GetFilter()")); return f.cFilter.Text; }
        #endregion
        #region Menu New Object
        //_________________________________________________________________________________________________________
        public bool SetObjectType(string sType) {
            int indice = f.cObjClass.FindStringExact(sType);
            if (indice < 0) { LogError(string.Format("SetObjectType({0})", sType)); return false; }
            f.cObjClass.SelectedIndex = indice;
            LogDebug(string.Format("SetObjectType({0})", sType));
            return true;
            }
        //_________________________________________________________________________________________________________
        public string GetObjectType() {
            LogDebug(string.Format("GetObjectType()"));
            return f.cObjClass.Text;
            }
        //_________________________________________________________________________________________________________
        public string GetObjectSourceOption() {
            LogDebug(string.Format("GetObjectSourceOption()"));
            return f.cBU.Text;
            }
        //_________________________________________________________________________________________________________
        public bool SetObjectSourceOption(string sSource) {
            int indice = f.cBU.FindStringExact(sSource);
            if (indice < 0) { LogError(string.Format("SetObjectSourceOption({0})", sSource)); return false; }
            f.cBU.SelectedIndex = indice;
            LogDebug(string.Format("SetObjectSourceOption({0})", sSource));
            return true;
            }
        //_________________________________________________________________________________________________________
        public List<string> GetObjectTypeList() {
            List<string> lObjectTypeList = new List<string>();
            foreach (string s in f.cObjClass.Items) lObjectTypeList.Add(s);
            LogDebug(string.Format("GetObjectTypeList()"));
            return lObjectTypeList;
            }
        //_________________________________________________________________________________________________________
        public List<string> GetObjectSourceOptionList() {
            List<string> lSourceOptionList = new List<string>();
            foreach(string s in f.cBU.Items) lSourceOptionList.Add(s);
            LogDebug(string.Format("GetObjectSourceOptionList()"));
            return lSourceOptionList;
            }
        //_________________________________________________________________________________________________________
        public List<string> NewObject() {
            LogDebug(string.Format("NewObject()[class={0},option={1}]", GetObjectType(), GetObjectSourceOption()));
            List<string> lInitialNames = GetObjList();
            f.mNewObject_Click(null, null);
            List<string> lFinalNames = GetObjList();
            List<string> lNewNames = lFinalNames.Where(x => !lInitialNames.Contains(x)).ToList<string>();
            return lNewNames;
            }
        #endregion
        #region Menu Custom Actions
        //_________________________________________________________________________________________________________
        public void ExecCustomActions() { LogDebug(string.Format("ExecCustomActions()")); f.mCustActions_Click(null, null); }
        #endregion
        #region Menu Delete
        //_________________________________________________________________________________________________________
        public bool DeleteObject(string sAlias, string sSource = "") {
            if (sAlias == "" || !ShowObject(sAlias)) { LogError(string.Format("DeleteObject({0}, {1})", sAlias, sSource)); return false; }
            f.Delete(null, null, sSource);
            LogDebug(string.Format("DeleteObject({0}, {1})", sAlias, sSource));
            return true;
            }
        #endregion
        #region Log Actions
        //_________________________________________________________________________________________________________
        public void LogInfo(string sCadena) { if(Info) cWinLog.Log(Level.Info, sCadena); }
        public void LogError(string sCadena) { if(Error) cWinLog.Log(Level.Error, sCadena); }
        public void LogWarning(string sCadena) { if(Warning) cWinLog.Log(Level.Warning, sCadena); }
        public void LogDebug(string sCadena) { if(Debug) cWinLog.Log(Level.Debug, sCadena); }
        public void ShowLog(bool bOpc) { f.mShowLogWindow_Click(null, null); cWinLog.Show(bOpc); }
        public void EnableLogLevels(bool bDebug, bool bInfo, bool bWarning, bool bError) { Debug = bDebug; Info = bInfo; Warning = bWarning; Error = bError; }
        public void ClearLog() { cWinLog.ClearLog(); LogDebug(string.Format("ClearLog()")); }
        #endregion
        #region Menu MA Profiles Execution
        //_________________________________________________________________________________________________________
        public bool SetRunProfileOption(string sType) {
            int indice = f.mProfilesOptions.FindStringExact(sType);
            if (indice >= 0) { LogDebug(string.Format("SetRunProfileOption({0})", sType)); f.mProfilesOptions.SelectedIndex = indice; return true; }
            LogError(string.Format("SetRunProfileOption({0})", sType));
            return false;
            }
        //_________________________________________________________________________________________________________
        public List<string> GetRunProfileOptionList() {
            List<string> lRunProfileOptionList = new List<string>();
            foreach (string s in f.mProfilesOptions.Items) lRunProfileOptionList.Add(s);
            LogDebug(string.Format("GetRunProfileOptionList()"));
            return lRunProfileOptionList;
            }
        //_________________________________________________________________________________________________________
        public bool SetMaxRunProfiles(int iNum) {
            if (iNum < 1 || iNum > 10) { LogError(string.Format("SetMaxRunProfiles({0})", iNum)); return false; }
            f.cMaxRunProfiles.Text = "" + iNum;
            LogDebug(string.Format("SetMaxRunProfiles({0})", iNum));
            return true;
            }
        //_________________________________________________________________________________________________________
        public void RunMAProfilesAsync(string sExecName = "", string sNextScriptName = "") {
            if (StillRunningMAProfiles()) { LogError(string.Format("RunMAProfilesAsync({0},{1})", sExecName, sNextScriptName)); return; }
            LogDebug(string.Format("RunMAProfilesAsync({0},{1})", sExecName, sNextScriptName));
            MacroHelper.Running = true;
            MacroHelper.ExecName = sExecName;
            MacroHelper.NextScriptName = sNextScriptName.Trim() == "" ? currentMacro : Config.PathMacros + sNextScriptName + ".cs";
            MacroHelper.MustCallBack = true;
            f.cTimerStart.Enabled = true;
            }
        //_________________________________________________________________________________________________________
        public List<string> RunMAProfile( string sAlias, string sProfile ) {
            if ( MacroHelper.Running || !f.mRunMAProfiles.Enabled ) { LogError(string.Format("RunMAProfile({0},{1})", sAlias, sProfile)); return null; }
            ResultProfile r = WmiHelper.RunProfile(sAlias, sProfile);
            List<string> result = new List<string>();
            result.Add(r.r);
            result.Add("changes: " + r.changes);
            result.Add(r.file);
            result.Add(r.desc);
            return result;
            }
        //_________________________________________________________________________________________________________
        public bool StillRunningMAProfiles() { LogDebug(string.Format("StillRunningMAProfiles()running: [{0}] disabled: [{1}]", MacroHelper.Running, !f.mRunMAProfiles.Enabled)); return MacroHelper.Running || !f.mRunMAProfiles.Enabled; }
        //_________________________________________________________________________________________________________
        public bool ReadAgentResult(int Index, ref bool bError, ref bool bChanges, ref string sMsg) { 
            if (StillRunningMAProfiles()) { LogError(string.Format("MAProfilesEndOK()")); return false; }
            bool r = f.ReadAgentResult(Index, ref bError, ref bChanges, ref sMsg);
            if (!r) 
                LogError(string.Format("MAProfilesEndOK()"));
            else
                LogDebug(string.Format("MAProfilesEndOK()"));
            return r;
            }
        #endregion
        #region View Object Operations
        //_________________________________________________________________________________________________________
        public bool ShowObject(string sAlias) {
            LogDebug(string.Format("ShowObject({0})", sAlias));
            return SelectObjOfList(sAlias);
            }
        //_________________________________________________________________________________________________________
        public string GetCurrentObject() {
            LogDebug(string.Format("CurrentObject"));
            return GetSelectObjOfList();
            }
        //_________________________________________________________________________________________________________
        public string GetObjFilterValue(string sAlias) {
            LogDebug(string.Format("GetObjFilterValue({0})", sAlias));
            for (int Row = 0; Row < f.dgObjs.RowCount; Row++)
                if (f.dgObjs[0, Row].Value.ToString() == sAlias) return f.dgObjs[1, Row].Value.ToString();
            return "";
            }
        //_________________________________________________________________________________________________________
        public List<string> GetListObjects() {
            LogDebug(string.Format("GetListObjects()"));
            return GetObjList();
            }
        //_________________________________________________________________________________________________________
        public List<string> GetListAttributes(string sGroup = "") {
            LogDebug(string.Format("GetListAttributes({0})", sGroup));
            List<string> lNames = new List<string>();
            for (int Row = 0; Row < dg.RowCount; Row++)
                if (sGroup == "" || sGroup == dg[1, Row].Value.ToString())
                    lNames.Add(dg[0, Row].Value.ToString());
            return lNames;
            }
        //_________________________________________________________________________________________________________
        public List<string> GetListSources() {
            LogDebug(string.Format("GetListSources()"));
            List<string> lNames = new List<string>();
            for (int Col = 2; Col < dg.ColumnCount; Col++) lNames.Add(dg.Columns[Col].Name);
            return lNames;
            }
        //_________________________________________________________________________________________________________
        public List<string> GetSelectedAttributes() {
            LogDebug(string.Format("GetSelectedAttribute()"));
            List<string> lNames = new List<string>();
            foreach (DataGridViewCell item in dg.SelectedCells) {
                string name = dg[0, item.RowIndex].Value.ToString();
                if (!lNames.Contains(name)) lNames.Add(name);
                }
            return lNames;
            }
        //_________________________________________________________________________________________________________
        public List<string> GetSelectedSources() {
            LogDebug(string.Format("GetSelectedSource()"));
            List<string> lNames = new List<string>();
            foreach (DataGridViewCell item in dg.SelectedCells) {
                string name = dg.Columns[item.ColumnIndex].Name;
                if (!lNames.Contains(name)) lNames.Add(name);
                }
            return lNames;
            }
        #endregion
        #region Edit Object Operations
        //_________________________________________________________________________________________________________
        public string GetGridValue(string sSource, string sAttrib) {
            int RowValor = GetRowIndex(sAttrib);
            int ColValor = GetColIndex(sSource);
            if (RowValor >= 0 && ColValor >= 0) { LogDebug(string.Format("GetGridValue({0},{1})", sSource, sAttrib)); return GetGridValue(ColValor, RowValor); }
            LogError(string.Format("GetGridValue({0},{1})", sSource, sAttrib));
            return "";
            }
        //_________________________________________________________________________________________________________
        public bool MarkAttribute(string sAttrib) {
            int RowValor = GetRowIndex(sAttrib);
            if (RowValor >= 0) { LogDebug(string.Format("MarkAttribute({0})", sAttrib)); GridHelper.MarkRow(dg, RowValor); return true; }
            LogError(string.Format("MarkAttribute({0})", sAttrib));
            return false;
            }
        //_________________________________________________________________________________________________________
        public bool ShowGridCell(string sSource, string sAttrib, bool bFullRow) {
            int RowValor = GetRowIndex(sAttrib);
            int ColValor = GetColIndex(sSource);
            if (RowValor >= 0 && ColValor >= 0) { LogDebug(string.Format("ShowGridCell({0},{1},{2})", sSource, sAttrib, bFullRow)); return ShowGridCell(ColValor, RowValor, bFullRow); }
            LogError(string.Format("ShowGridCell({0},{1},{2})", sSource, sAttrib, bFullRow));
            return false;
            }
        //_________________________________________________________________________________________________________
        public void SetGridValue(string sSource, string sAttrib, string value) {
            int RowValor = GetRowIndex(sAttrib);
            int ColValor = GetColIndex(sSource);
            if (RowValor >= 0 && ColValor >= 0) { LogDebug(string.Format("SetGridValue({0},{1},{2})", sSource, sAttrib, value)); SetGridValue(ColValor, RowValor, value); }
            else LogError(string.Format("SetGridValue({0},{1},{2})", sSource, sAttrib, value));
            }
        //_________________________________________________________________________________________________________
        public bool IsValidGridPosition(string sSource, string sAttrib) {
            int RowValor = GetRowIndex(sAttrib);
            int ColValor = GetColIndex(sSource);
            if (RowValor >= 0 && ColValor >= 0) { LogDebug(string.Format("IsValidGridPosition({0},{1})", sSource, sAttrib)); return true; }
            LogError(string.Format("IsValidGridPosition({0},{1})", sSource, sAttrib));
            return false;
            }
        //_________________________________________________________________________________________________________
        public bool AreEqualAtributeValues(string Source1, string Source2, List<string> lAttribs) {
            string[] lSrc = null, lDst = null;
            LogDebug(string.Format("AreEqualAtributeValues({0},{1})?", Source1, Source2));
            if (lAttribs.Count == 0) return false;
            foreach (string sAttr in lAttribs) {
                if (!IsValidGridPosition(Source1, sAttr) || !IsValidGridPosition(Source2, sAttr)) return false; 
                string sSrc = GetGridValue(Source1, sAttr);
                string sDst = GetGridValue(Source2, sAttr);
                if (sSrc != sDst) {
                    if (sSrc.Contains(Config.SeparatorC) && sDst.Contains(Config.SeparatorC)) {
                        lSrc = sSrc.Split(Config.SeparatorC); Array.Sort(lSrc); sSrc = string.Join(Config.Separator, lSrc);
                        lDst = sDst.Split(Config.SeparatorC); Array.Sort(lDst); sDst = string.Join(Config.Separator, lDst);
                        if (sSrc != sDst) { cWinLog.Log(Level.Warning, string.Format("AreEqualAtributeValues({0},{1}) - {2}", Source1, Source2, sAttr)); return false; }
                        }
                    else { cWinLog.Log(Level.Warning, string.Format("AreEqualAtributeValues({0},{1}) - {2}", Source1, Source2, sAttr)); return false; }
                    }
                }
            LogDebug(string.Format("AreEqualAtributeValues({0},{1}) = YES", Source1, Source2));
            return true; 
            }
        #endregion
        #region Macro Operations
        //_________________________________________________________________________________________________________
        public void AddPendingMacro(string sNextMacro, string sParam = "") {
            LogDebug(string.Format("AddPendingMacro({0}, {1})", sNextMacro, sParam));
            pendingMacros.Add(sNextMacro + Config.Separator + sParam); 
            }
        #endregion
        //_________________________________________________________________________________________________________
        #region Macro Start Execution Next Macro (with Interface Alignment)
        //_________________________________________________________________________________________________________
        public void ExecNextMacro() {
            if (pendingMacros.Count == 0) return;
            string sMacroFile = pendingMacros[0].Split(Config.SeparatorC)[0];
            string sParam = pendingMacros[0].Replace(sMacroFile + Config.Separator, "");
            pendingMacros.Remove(pendingMacros[0]);
            sMacroFile = Config.PathMacros + sMacroFile + ".cs";
            ExecMacro(sMacroFile, sParam);
            }
        #endregion
        //_________________________________________________________________________________________________________
        #region Macro Start Execution (with Interface Alignment)
        //_________________________________________________________________________________________________________
        public void ExecMacro(string sMacroFile, string sParam = "") {
            LogHelper.Msg();
            try {
                if (!File.Exists(sMacroFile)) return;
                using (var helper = new AsmHelper(CSScript.CompileFile(sMacroFile), null, false)) {
                    IFIMSyncMacro script = helper.CreateAndAlignToInterface<IFIMSyncMacro>("Script");
                    script.EntryScript(this, sParam);
                    // Save data till the end of macro execution (EndScript)
                    currentMacro = sMacroFile;
                    currentMacroParam = sParam;
                    }
                }
            catch (Exception ex) {
                string sMsg = string.Format("[{0}]\n[{1}]\n{2}", sMacroFile, sParam, ex.Message);
                LogHelper.Msg("ERROR:\n" + sMsg);
                LogError(sMsg.Replace('\n', '\\'));
                MessageBox.Show("ERROR:\n" + sMsg, "ExecMacro", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            // If there is not pending activities, call to EndScript
            while (!MacroHelper.Running && currentMacro != "") ExecMacroEnd(currentMacro, currentMacroParam);
            if (currentMacro == "" && pendingMacros.Count == 0) f.mExecMacro.Enabled = true;
            }
        #endregion
        //_________________________________________________________________________________________________________
        #region Macro Execution Next Section after Async call (with Interface Alignment)
        //_________________________________________________________________________________________________________
        public void ExecMacroNextSectionAfterAsyncCall(string sMacroFile) {
            LogHelper.Msg();
            try {
                if (!File.Exists(sMacroFile)) return;
                using (var helper = new AsmHelper(CSScript.CompileFile(sMacroFile), null, false)) {
                    IFIMSyncMacro script = helper.CreateAndAlignToInterface<IFIMSyncMacro>("Script");
                    script.LastExecTerminated(this, MacroHelper.ExecName, currentMacroParam);
                    }
                }
            catch (Exception ex) {
                string sMsg = string.Format("[{0}]\n[{1}]\n{2}", sMacroFile, currentMacroParam, ex.Message);
                LogHelper.Msg("ERROR:\n" + sMsg);
                LogError(sMsg.Replace('\n', '\\'));
                MessageBox.Show("ERROR:\n" + sMsg, "ExecMacroNextSectionAfterAsyncCall", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            // If there is not pending activities, call to EndScript
            if (!MacroHelper.Running && currentMacro != "") ExecMacroEnd(currentMacro, currentMacroParam);
            }
        #endregion
        //_________________________________________________________________________________________________________
        #region Macro End Execution (with Interface Alignment)
        //_________________________________________________________________________________________________________
        public void ExecMacroEnd(string sMacroFile, string sParam = "") {
            LogHelper.Msg();
            try {
                currentMacro = ""; currentMacroParam = "";
                if (!File.Exists(sMacroFile)) return;
                using (var helper = new AsmHelper(CSScript.CompileFile(sMacroFile), null, false)) {
                    IFIMSyncMacro script = helper.CreateAndAlignToInterface<IFIMSyncMacro>("Script");
                    script.EndScript(this, sParam);
                    // If an Async method is called, again a new call to EndScript is pending
                    if (MacroHelper.Running) { currentMacro = sMacroFile; currentMacroParam = sParam; }
                    }
                }
            catch (Exception ex) {
                string sMsg = string.Format("[{0}]\n[{1}]\n{2}", sMacroFile, sParam, ex.Message);
                LogHelper.Msg("ERROR:\n" + sMsg);
                LogError(sMsg.Replace('\n', '\\'));
                MessageBox.Show("ERROR:\n" + sMsg, "ExecMacroEnd", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            // If there is not pending activities, call to EndScript
            while (!MacroHelper.Running && currentMacro != "") ExecMacroEnd(currentMacro, currentMacroParam);
            // Enable timer to call next pending macro or enable menu
            if (!MacroHelper.Running && pendingMacros.Count > 0) 
                f.cTimerNextMacro.Enabled = true;
            else
                f.mExecMacro.Enabled = true;
            }
        #endregion
        //_________________________________________________________________________________________________________
        #region New Macro
        public static string NewMacro(string sProposedName = "") {
            int cont = 1;
            string sMacroName = sProposedName == ""?"newMacro" + cont:Path.GetFileNameWithoutExtension(sProposedName);
            string sMacroPath = Path.Combine(Config.PathMacros, sMacroName + ".cs");
            while (File.Exists(sMacroPath)) {
                sMacroName = "newMacro" + ++cont;
                sMacroPath = Path.Combine(Config.PathMacros, sMacroName + ".cs");
                }
            using (StreamWriter sw = File.AppendText(sMacroPath)) {
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("//css_reference ..\\FIMSyncTest.exe;");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("using System;");
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine("using System.Windows.Forms;");
                sw.WriteLine(" ");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("// Menu Load Data");
                sw.WriteLine("// void LoadData();");
                sw.WriteLine("// bool SetMaxLoadItems(int iNum);");
                sw.WriteLine("// bool SetFilter(string sFilter);");
                sw.WriteLine("// string GetFilter(); ");
                sw.WriteLine(" ");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("// Menu New Object ");
                sw.WriteLine("// bool SetObjectType(string sType);");
                sw.WriteLine("// bool SetObjectSourceOption(string sSource);");
                sw.WriteLine("// string GetObjectType();");
                sw.WriteLine("// string GetObjectSourceOption();");
                sw.WriteLine("// List<string> GetObjectTypeList(); ");
                sw.WriteLine("// List<string> GetObjectSourceOptionList(); ");
                sw.WriteLine("// List<string> NewObject(); ");
                sw.WriteLine(" ");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("// Menu Custom Actions ");
                sw.WriteLine("// void ExecCustomActions(); ");
                sw.WriteLine(" ");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("// Menu Delete ");
                sw.WriteLine("// bool DeleteObject(string sAlias, string sSource = \"\");");
                sw.WriteLine(" ");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("// Menu MA Profiles Execution");
                sw.WriteLine("// bool SetRunProfileOption(string sType);");
                sw.WriteLine("// List<string> GetRunProfileOptionList();");
                sw.WriteLine("// bool SetMaxRunProfiles(int iNum); ");
                sw.WriteLine("// void RunMAProfilesAsync(string sExecName = \"\", string sNextScriptName = \"\");");
                sw.WriteLine("// List<string> RunMAProfile( string sAlias, string sProfile); // return short result, number of changes, XML file name and detailed result");
                sw.WriteLine("// bool StillRunningMAProfiles();");
                sw.WriteLine("// bool ReadAgentResult(int Index, ref bool bError, ref bool bChanges, ref string sMsg);");
                sw.WriteLine("// Note: If 'RunMAProfilesAsync' is used, it must be the last call in macro and wait 'LastExecTerminated' event to continue if it is necessary.");
                sw.WriteLine(" ");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("// Macro Operations");
                sw.WriteLine("// void AddPendingMacro(string sNextMacro, string sParam = \"\");");
                sw.WriteLine(" ");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("// View Object Operations");
                sw.WriteLine("// string GetCurrentObject();");
                sw.WriteLine("// bool ShowObject(string sAlias);");
                sw.WriteLine("// string GetObjFilterValue(string sAlias);");
                sw.WriteLine("// List<string> GetListObjects();");
                sw.WriteLine("// List<string> GetListAttributes(string sGroup = \"\");");
                sw.WriteLine("// List<string> GetListSources();");
                sw.WriteLine("// List<string> GetSelectedAttributes();");
                sw.WriteLine("// List<string> GetSelectedSources();");
                sw.WriteLine(" ");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("// Edit Object Operations");
                sw.WriteLine("// string GetGridValue(string sSource, string sAttrib);");
                sw.WriteLine("// void SetGridValue(string sSource, string sAttrib, string value);");
                sw.WriteLine("// bool MarkAttribute(string sAttrib);");
                sw.WriteLine("// bool ShowGridCell(string sSource, string sAttrib, bool bFullRow);");
                sw.WriteLine("// bool IsValidGridPosition(string sSource, string sAttrib); ");
                sw.WriteLine("// bool AreEqualAtributeValues(string Source1, string Source2, List<string> lAttribs);");
                sw.WriteLine(" ");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("// Log Operations");
                sw.WriteLine("// void LogInfo(string sCadena);");
                sw.WriteLine("// void LogError(string sCadena);");
                sw.WriteLine("// void LogWarning(string sCadena);");
                sw.WriteLine("// void LogDebug(string sCadena);");
                sw.WriteLine("// void EnableLogLevels(bool bDebug, bool bInfo, bool bWarning, bool bError);");
                sw.WriteLine("// void ClearLog();");
                sw.WriteLine("// void ShowLog(bool bOpc);");
                sw.WriteLine(" ");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("public class Script : MarshalByRefObject {");
                sw.WriteLine("\t//_____________________________________________________________________________________________");
                sw.WriteLine("\tpublic void EntryScript(FIMSyncTest.MacroHelper mh, string sParam = \"\") {");
                sw.WriteLine("\t\tmh.ShowLog(true); ");
                sw.WriteLine("\t\tmh.LogInfo(string.Format(\"Inicio Macro[" + sMacroName + "(\" + sParam + \")]\"));");
                sw.WriteLine("\t\t}");
                sw.WriteLine("\t//_____________________________________________________________________________________________");
                sw.WriteLine("\tpublic void LastExecTerminated(FIMSyncTest.MacroHelper mh, string sValue, string sParam = \"\"){");
                sw.WriteLine("\t\tmh.LogInfo(string.Format(\"Terminado Async RunProfiles({0}) en Macro[" + sMacroName + "(\" + sParam + \")]\", sValue));");
                sw.WriteLine( "\t\t}");
                sw.WriteLine("\t//_____________________________________________________________________________________________");
                sw.WriteLine("\tpublic void EndScript(FIMSyncTest.MacroHelper mh, string sParam = \"\"){");
                sw.WriteLine("\t\tmh.LogInfo(string.Format(\"Fin Macro[" + sMacroName + "(\" + sParam + \")]\"));");
                sw.WriteLine("\t\t}");
                sw.WriteLine( "\t}");
                }
            return sMacroPath;
            }
        #endregion
        //_________________________________________________________________________________________________________
        #region Nueva Macro About
        internal static bool AboutConditions(ToolStripTextBox cMaxRunProfiles) {
            if (Control.ModifierKeys == (Keys.Control | Keys.Shift) && cMaxRunProfiles.Text == "About") return true;
            return false;
            }
        internal static string NewAboutMacro() {
            int cont = 1;
            string sMacroName = "newAboutMacro" + cont + ".cs";
            string sMacroPath = Path.Combine(Config.PathMacros, sMacroName);
            while (File.Exists(sMacroPath)) {
                sMacroName = "newAboutMacro" + ++cont + ".cs";
                sMacroPath = Path.Combine(Config.PathMacros, sMacroName);
                }
            using (StreamWriter sw = File.AppendText(sMacroPath)) {
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("//css_reference ..\\FIMSyncTest.exe;");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("using System;");
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine("using System.Windows.Forms;");
                sw.WriteLine(" ");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("//_________________________________________________________________________________________________");
                sw.WriteLine("public class Script : MarshalByRefObject {");
                sw.WriteLine("\t//_____________________________________________________________________________________________");
                sw.WriteLine("\tpublic void EntryScript(FIMSyncTest.MacroHelper mh) {");
                sw.WriteLine("\t\tmh.ShowLog(true); ");
                sw.WriteLine("\t\tApplication.DoEvents();System.Threading.Thread.Sleep(4000);Application.DoEvents();");
                sw.WriteLine("\t\tmh.ShowLog(false); ");
                sw.WriteLine("\t\t}");
                sw.WriteLine("\t//_____________________________________________________________________________________________");
                sw.WriteLine("\tpublic void LastExecTerminated(FIMSyncTest.MacroHelper mh, string sValue){");
                sw.WriteLine("\t\tmh.LogInfo(string.Format(\"Terminado Async RunProfiles({0})\", sValue));");
                sw.WriteLine("\t\t}");
                sw.WriteLine("\t//_____________________________________________________________________________________________");
                sw.WriteLine("\tpublic void EndScript(FIMSyncTest.MacroHelper mh){");
                sw.WriteLine("\t\tmh.LogInfo(string.Format(\"Fin Macro(" + sMacroName + ")\"));");
                sw.WriteLine("\t\t}");
                sw.WriteLine("\t}");
                }
            return sMacroPath;
            }
        #endregion
        //_________________________________________________________________________________________________________
        #region Métodos auxiliares para las macros
        //_________________________________________________________________________________________________________
        private string GetNextMacro() {
            if (pendingMacros.Count == 0) return ""; 
            string sNextMacro = pendingMacros[0].Split(Config.SeparatorC)[0]; 
            pendingMacros.Remove(sNextMacro);
            return sNextMacro;
            }
        //_________________________________________________________________________________________________________
        private string GetGridValue(int Col, int Row) { return dg[Col, Row].Value == null ? "" : dg[Col, Row].Value.ToString(); }
        //_________________________________________________________________________________________________________
        private void SetGridValue(int Col, int Row, string value) {
            if (dg != null) {
                dg[Col, Row].Value = value;
                DataGridViewCellEventArgs e = new DataGridViewCellEventArgs(Col, Row);
                f.dgValues_CellEndEdit(null, e);
                }
            }
        //_________________________________________________________________________________________________________
        private int GetColIndex(string sName) {
            for (int Col = 0; Col < dg.ColumnCount; Col++)
                if (dg.Columns[Col].Name == sName)
                    return Col;
            return -1;
            }
        //_________________________________________________________________________________________________________
        private int GetRowIndex(string sName) {
            for (int Row = 0; Row < dg.RowCount; Row++)
                if (GetGridValue(0, Row) == sName)
                    return Row;
            return -1;
            }
        //_________________________________________________________________________________________________________
        private List<string> GetObjList() {
            List<string> lNames = new List<string>();
            for (int Row = 0; Row < f.dgObjs.RowCount; Row++) lNames.Add(f.dgObjs[0, Row].Value.ToString());
            return lNames;
            }
        //_________________________________________________________________________________________________________
        private bool SelectObjOfList(string sAlias) {
            int Row = GridHelper.SearchRowByLink(f.dgObjs, sAlias);
            if (Row < 0) return false;
            f.dgObjs.Rows[Row].Selected = true;
            f.dgObjs.CurrentCell = f.dgObjs.Rows[Row].Cells[0];
            DataGridViewCellMouseEventArgs e = new DataGridViewCellMouseEventArgs(0, Row, 0, 0, new MouseEventArgs(MouseButtons.Left,1,1,1,1));
            f.dgObjs_RowHeaderMouseDoubleClick(null, e);
            return true;
            }
        //_________________________________________________________________________________________________________
        private string GetSelectObjOfList() { return f.RowForRefresh; }
        //_________________________________________________________________________________________________________
        private bool ShowGridCell(int Col, int Row, bool bFullRow) {
            if (dg != null && Col >= 0 && Row >= 0) {
                if (bFullRow) dg.Rows[Row].Selected = true;
                dg.CurrentCell = dg.Rows[Row].Cells[Col];
                dg.FirstDisplayedCell = dg.CurrentCell;
                return true;
                }
            return false;
            }
        //_________________________________________________________________________________________________________
        internal static void ChangeListBoxWidth(ToolStripComboBox cLst) {
            if (cLst.Items.Count == 0) return; 
            using (System.Drawing.Graphics graphics = cLst.GetCurrentParent().CreateGraphics()) {
                int maxWidth = 0;
                foreach (string s in cLst.Items) {
                    System.Drawing.SizeF area = graphics.MeasureString(s, cLst.Font);
                    maxWidth = Math.Max((int)area.Width, maxWidth);
                    }
                cLst.DropDownWidth = maxWidth+10;
                cLst.Width = maxWidth+20; // Requiere Autosize=false
                }
            }

        #endregion
        }
    }
