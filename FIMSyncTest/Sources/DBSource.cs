using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Drawing;
using System.Data.SqlClient;

namespace FIMSyncTest.Sources {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    class DBSource : MarshalByRefObject, IDataSource {
        private DataRow Row;
        private DataTable tAttributes;
        private string connString { set; get; }
        private SqlConnection conn { set; get; }
        private string objType { set; get; }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public string Name { private set; get; }
        public string Type { private set; get; }
        public string Rol { private set; get; }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public DBSource(DataRow Row, DataTable tAttributes) { 
            this.Row = Row;
            this.tAttributes = tAttributes;
            this.Name = Row["Alias"].ToString();
            this.Rol = Row["Rol"].ToString();
            this.Type = Row["Type"].ToString();
            connString = Row["Connection"].ToString();
            objType = Row["Base"].ToString();
            conn = new SqlConnection(connString);
            try {
                conn.Open();
                }
            catch (SqlException ex) {
                LogHelper.Msg("DB Connection failed: " + ex.Message);
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public string CreateObject(Dictionary<string, string> data) { return ""; }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public Dictionary<string, Dictionary<string, string>> FindAllEntries(int limit = 0, string sOtherAttribs = "", string sAdditionalFilter = "") {
            SqlCommand cmd = new SqlCommand();
            Dictionary<string, Dictionary<string, string>> salida = new Dictionary<string, Dictionary<string, string>>();
            SqlDataReader reader;
            string sSql = "SELECT";
            if (limit > 0) sSql += " TOP " + limit;
            sSql += " * FROM mms_metaverse where cn is not null AND [object_type]='" + objType + "'";
            if (sAdditionalFilter != "" && Config.ObjectAliasAttribute != "") {
                if (sAdditionalFilter.Contains("*"))
                    sSql += " and " + Config.ObjectAliasAttribute + " LIKE '" + sAdditionalFilter.Replace('*', '%') + "'";
                else
                    sSql += " and " + Config.ObjectAliasAttribute + " = '" + sAdditionalFilter + "'";
                } 
            cmd.CommandText = sSql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            if (conn.State != ConnectionState.Open) return salida;

            reader = cmd.ExecuteReader();
            if (reader.HasRows) {
                while (reader.Read()) {
                    Dictionary<string, string> r = new Dictionary<string, string>();
                    for (int i = 0; i < tAttributes.Rows.Count; i++) {
                        string sUsage = tAttributes.Rows[i]["Usage"].ToString();

                        if ((sUsage.Contains("Link")                                                                                 // If attribute is marked with Link
                            || sUsage.Contains("Alias"))                                                                               // If attribute is marked with Alias
                            && tAttributes.Rows[i][this.Name].ToString() != ""                                                        // If attribute is present in this source
                            && reader.GetValue(reader.GetOrdinal(tAttributes.Rows[i][this.Name].ToString())) != System.DBNull.Value) {// If attribute has a value

                            r.Add(tAttributes.Rows[i]["Alias"].ToString(), reader.GetString(reader.GetOrdinal(tAttributes.Rows[i][this.Name].ToString())));
                            }
                        }
                    salida.Add("" + salida.Count, r);
                    }
                }
            else {
                Console.WriteLine("No rows found.");
                }
            reader.Close();
            return salida;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public Dictionary<string, Dictionary<string, string>> FindAllAttributes(string cn, bool bOnlyOne = true) {
            SqlCommand cmd = new SqlCommand();
            Dictionary<string, Dictionary<string, string>> salida = new Dictionary<string, Dictionary<string, string>>();
            SqlDataReader reader;

            cmd.CommandText = "SELECT " + (bOnlyOne ? "TOP 1" : "") + " * FROM mms_metaverse where cn ='" + cn + "' AND [object_type]='" + objType + "'";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            if (conn.State != ConnectionState.Open) return salida;

            //delete image files
            (new DirectoryInfo(Config.PathImages)).GetFiles(this.Name + "_*.jpg").ToList().ForEach(f => f.Delete());

            reader = cmd.ExecuteReader();
            if (reader.HasRows) {
                while (reader.Read()) {
                    Dictionary<string, string> r = new Dictionary<string, string>();
                    for (int i = 0; i < tAttributes.Rows.Count; i++) {
                        string name = tAttributes.Rows[i][Row["Alias"].ToString()].ToString().ToLower();
                        string alias = tAttributes.Rows[i]["Alias"].ToString();
                        string sUsage = tAttributes.Rows[i]["Usage"].ToString();
                        string EsMV = tAttributes.Rows[i]["MV"].ToString();
                        bool added = false;
                        if (EsMV != "N" && name != "") {
                            if (sUsage.Contains("Jpeg") ) 
                                r.Add(alias, FindMVAttributeBinary(cn, alias, name));
                            else
                                r.Add(alias, FindMVAttribute(cn, alias, name));
                            added = true;
                            }
                        for (int c = 0; c < reader.FieldCount && name != "" && !added; c++) {
                            if (reader.GetName(c).ToLower() == name) {
                                if (sUsage.Contains("Jpeg") && reader.IsDBNull(c) == false) {
                                    r.Add(alias, BitConverter.ToString(reader[c] as byte[]).Replace("-", "").Substring(0, 30));
                                    using (MemoryStream ms = new MemoryStream(reader[c] as byte[])) {
                                        Bitmap.FromStream(ms).Save(Config.PathImages + this.Name + "_" + alias + "_0.jpg");
                                        }
                                    }
                                else {
                                    r.Add(alias, reader.IsDBNull(c) ? "<ND>" : reader[c].ToString());
                                    }
                                added = true;
                                }
                            }
                        if(!added)  r.Add(alias,"");
                        }
                    r.Add("#ID#", cn);
                    salida.Add("" + salida.Count, r);
                    }
                }
            reader.Close();
            return salida;
            }
        //_________________________________________________________________________________________________________
        //  select distinct 
        //  cn,
        //  stuff((select '|' + [string_value_indexable] from [FIMSynchronizationService].[dbo].[mms_metaverse] AS d
        //      JOIN [FIMSynchronizationService].[dbo].[mms_metaverse_multivalue] AS e
        //      ON (d.object_id = e.object_id) where cn='00009' and [attribute_name]='proxyAddressCollection'
        //      for xml path('')), 1, 1, '') as proxyAddresses,
        //  stuff((select'|' + [string_value_indexable] from [FIMSynchronizationService].[dbo].[mms_metaverse] AS d
        //      JOIN [FIMSynchronizationService].[dbo].[mms_metaverse_multivalue] AS e
        //      ON (d.object_id = e.object_id) where cn='00009' and [attribute_name]='otherMailbox'
        //      for xml path('')), 1, 1, '') as otherMailbox 
        //  from 
        //      [FIMSynchronizationService].[dbo].[mms_metaverse] AS d
        //  where 
        //      cn='00009'
        //
        //  select 
        //  stuff((select '|' + [string_value_indexable] from [FIMSynchronizationService].[dbo].[mms_metaverse] AS d
        //      JOIN [FIMSynchronizationService].[dbo].[mms_metaverse_multivalue] AS e
        //      ON (d.object_id = e.object_id) where cn='00009' and [attribute_name]='proxyAddressCollection'
        //      for xml path('')), 1, 1, '') as proxyAddresses
        //      ,
        //  stuff((select'|' + [string_value_indexable] from [FIMSynchronizationService].[dbo].[mms_metaverse] AS d
        //      JOIN [FIMSynchronizationService].[dbo].[mms_metaverse_multivalue] AS e
        //      ON (d.object_id = e.object_id) where cn='00009' and [attribute_name]='otherMailbox'
        //      for xml path('')), 1, 1, '') as otherMailbox 
        //  from 
        //      [FIMSynchronizationService].[dbo].[mms_metaverse] AS d
        //  where 
        //      cn='00009'
        //_________________________________________________________________________________________________________
        private string FindMVAttribute(string cn, string colAlias, string colName) {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string salida = "";
            cmd.CommandText = @"select stuff((select '" + Config.Separator + @"' + ISNULL([string_value_indexable],'') + ISNULL([string_value_not_indexable],'') 
                                       from [mms_metaverse] AS d JOIN [mms_metaverse_multivalue] AS e ON (d.object_id = e.object_id) 
                                       where d.cn='" + cn + @"' AND d.[object_type]='" + objType + "' and e.[attribute_name]='" + colName + @"' for xml path('')), 1, 1, '') as " + colAlias;

            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            if (conn.State != ConnectionState.Open) return salida;

            reader = cmd.ExecuteReader();
            if (reader.HasRows) {
                reader.Read();
                salida = reader.IsDBNull(0) ? "<ND>" : reader[0].ToString();
                }
            reader.Close();
            return salida;
            }
        //_________________________________________________________________________________________________________
        // Image attributes
        //_________________________________________________________________________________________________________
        private string FindMVAttributeBinary(string cn, string colAlias, string colName) {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string salida = "";
            cmd.CommandText = @"select [binary_value_not_indexable],[binary_value_indexable]  
                                       from [mms_metaverse] AS d JOIN [mms_metaverse_multivalue] AS e ON (d.object_id = e.object_id) 
                                       where d.cn='" + cn + @"' AND d.[object_type]='" + objType + "' and e.[attribute_name]='" + colName + "'";

            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            if (conn.State != ConnectionState.Open) return salida;
            int cont = 0;
            int col = 0;

            reader = cmd.ExecuteReader();
            if (reader.HasRows) {
                while (reader.Read()) {
                    col = reader.IsDBNull(0) == false ? 0 : 1;
                    if (reader.IsDBNull(col) == false) {
                        if (salida != "") salida += Config.Separator;
                        salida += BitConverter.ToString(reader[col] as byte[]).Replace("-", "").Substring(0, 30);
                        using (MemoryStream ms = new MemoryStream(reader[col] as byte[])) {
                            Bitmap.FromStream(ms).Save(Config.PathImages + this.Name + "_" + colAlias + "_" + cont++ + ".jpg");
                            }
                        }
                    }
                }
            reader.Close();

            return salida;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public bool DeleteAllEntries(string sTables=null) {
            if (sTables == null) return true;
            string[] lTables = sTables.Split(';');
            foreach (string sTable in lTables) {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "TRUNCATE TABLE " + sTable;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                if (conn.State != ConnectionState.Open) { LogHelper.Msg("Connection Closed to " + this.Name); return false; }
                int r = cmd.ExecuteNonQuery();
                LogHelper.Msg("Deleted " + r + " rows in " + this.Name + " [" + sTable + "]");
                }
            return true; 
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public bool DeleteEntry(string cn) { return true; }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public string UpdateAttributeValue(string Id, string Column, string AtrAlias, string NewValue, string OldValue) { return OldValue; }
        }
    }
