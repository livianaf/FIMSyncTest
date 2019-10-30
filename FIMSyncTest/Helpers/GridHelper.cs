using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using FIMSyncTest.Sources;

namespace FIMSyncTest {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    class GridHelper {
        //_________________________________________________________________________________________________________
        // https://www.daniweb.com/programming/software-development/code/374216/making-a-datagridview-look-like-an-excel-sheet
        // http://www.codeproject.com/Articles/691749/Free-NET-Spreadsheet-Control
        // Initialize all DataGridView controls with the same values
        //_________________________________________________________________________________________________________
        public static void InitializeDataGridView(DataGridView dg, bool AllowEdit=false) {
            dg.AllowUserToAddRows = false;
            dg.AllowUserToDeleteRows = false;
            dg.AllowUserToResizeRows = false;
            dg.EnableHeadersVisualStyles = false;
            dg.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;//DataGridViewSelectionMode.CellSelect 
            dg.EditMode = AllowEdit ? DataGridViewEditMode.EditOnKeystrokeOrF2 : DataGridViewEditMode.EditProgrammatically;
            dg.ShowEditingIcon = false;
            //dg.Location = new System.Drawing.Point(0, 124);
            //dg.Name = "dataGridView1";
            //dg.Size = new System.Drawing.Size(250, 125);
            dg.TabIndex = 0;
            dg.RowHeadersWidth = 35;
            //used to attach event-handlers to the events of the editing control(nice name!)
            //dg.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(Mydgv_EditingControlShowing);
            // not implemented here, but I still like the name DataGridViewEditingControlShowingEventHandler :o) LOL
            dg.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dg.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dg.RowHeadersDefaultCellStyle.Padding = new Padding(3);//helps to get rid of the selection triangle?
            dg.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", Config.FontSize, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            dg.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dg.ColumnHeadersDefaultCellStyle.BackColor = Color.Gainsboro;
            dg.RowHeadersDefaultCellStyle.Font = new Font("Verdana", Config.FontSize, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            dg.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dg.RowHeadersDefaultCellStyle.BackColor = Color.Gainsboro;
            dg.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", Config.FontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            dg.RowTemplate.Height = Config.RowHeight;
            dg.ColumnHeadersHeight = Config.RowHeight;
            dg.AllowUserToAddRows = false;
            }
        //_________________________________________________________________________________________________________
        // Initialize all DataGridView controls with a list of columns and rows (or empty)
        //_________________________________________________________________________________________________________
        public static void InitGrid(DataGridView dg, List<string> Columns, Dictionary<string, DataRow> Rows = null, int nRows = 0) {
            dg.Rows.Clear();
            dg.Columns.Clear();
            foreach (string s in Columns) // column list only contains column name
                dg.Columns.Add(s, s);

            // Frozen Columns
            dg.Columns[0].Frozen = true;

            if (Rows != null)
                foreach (string s in Rows.Keys) {
                    if (Rows[s]["Show"].ToString() == "N") continue; 
                    dg.Rows.Add();
                    // row list contains name and group of each attribute
                    dg[0, dg.Rows.Count - 1].Value = s;
                    dg[1, dg.Rows.Count - 1].Value = Rows[s]["Group"]; 
                    // Tag value contains the same value showed in cell to allow undo operations
                    dg[0, dg.Rows.Count - 1].Tag = s;
                    dg[1, dg.Rows.Count - 1].Tag = Rows[s]["Group"];
                    // Row Tag contains the details of row in Excel
                    dg.Rows[dg.Rows.Count - 1].Tag = Rows[s];
                    }
            else // If there is no list rows defined, default number of rows are created
                UpdateRows(dg, nRows); 
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void LoadAttributeData(DataGridView dg, DataRow Row) {
            dg[0, 0].Value = Row["Alias"];
            for (int i = 1; i < dg.Columns.Count; i++) {
                dg[i, 0].Value = Row[dg.Columns[i].Name];
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private static int SearchRowByCn(DataGridView dg, string cn) {
            for (int i = 0; i < dg.Rows.Count; i++) {
                if (dg[0, i].Value.ToString() == cn) return i;
                }
            dg.Rows.Add();
            dg[0, dg.Rows.Count - 1].Value = cn; // Value of CN found in new data source
            return dg.Rows.Count - 1;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private static int SearchOrAddRowByLink(DataGridView dg, Dictionary<string, string> dDatos, List<string> lLinks) {
            int r = SearchRowByLink(dg, dDatos, lLinks);
            if (r >= 0) {
                UpdateRowLinks(dg, dDatos, lLinks, r);
                return r;
                }
            dg.Rows.Add();
            // Value of CN found in new data source
            dg[0, dg.Rows.Count - 1].Value = dDatos["cn"]; 
            // Value of attribute selected as Alias of register
            if (Config.ObjectAliasAttribute != "" && dDatos.ContainsKey(Config.ObjectAliasAttribute) && dDatos[Config.ObjectAliasAttribute] != "") dg[1, dg.Rows.Count - 1].Value = dDatos[Config.ObjectAliasAttribute];
            // Cell tag in column 0 of each row contains the value of each identification attribute of register with this format <attr>[value1]<attr>[value2]...
            dg[0, dg.Rows.Count - 1].Tag = "";
            foreach (string s in lLinks) 
                if (dDatos.Keys.Contains(s) && dDatos[s] != "" && dDatos[s] != "<ND>") dg[0, dg.Rows.Count - 1].Tag += s + "[" + dDatos[s] + "]";
                
            return dg.Rows.Count - 1;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private static void UpdateRowLinks(DataGridView dg, Dictionary<string, string> dDatos, List<string> lLinks, int Row) {
            foreach (string s in lLinks) {
                if (dDatos.Keys.Contains(s) && dDatos[s] != "" && dDatos[s] != "<ND>" && !dg[0, Row].Tag.ToString().Contains("[" + dDatos[s] + "]")) 
                    dg[0, Row].Tag += s + "[" + dDatos[s] + "]";
                }
            }
        //_________________________________________________________________________________________________________
        // Utilizado para buscar una fila con un conjunto de datos del registro
        //_________________________________________________________________________________________________________
        private static int SearchRowByLink(DataGridView dg, Dictionary<string, string> dDatos, List<string> lLinks) {
            for (int i = 0; i < dg.Rows.Count; i++) {
                foreach (string s in lLinks) {
                    if (dDatos.Keys.Contains(s) && dg[0, i].Tag.ToString().Contains("[" + dDatos[s] + "]")) { return i; }
                    }
                }
            return -1;
            }
        //_________________________________________________________________________________________________________
        // Utilizado para buscar una fila con un único dato del registro
        //_________________________________________________________________________________________________________
        public static int SearchRowByLink(DataGridView dg, string sValue) {
            for (int i = 0; i < dg.Rows.Count; i++) 
                  if (dg[0, i].Tag.ToString().Contains("[" + sValue + "]")) return i; 
            return -1;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void FillObjRows(List<string> lLinks, DataGridView dg, Dictionary<string, Dictionary<string, string>> dDatos, string Column) {
            for (int i = 0; i < dDatos.Count; i++) {
                
                // UNUSED: search row by CN value. If it is not found an new row is added
                // int fila = SearchRowByCn(dg, dDatos["" + i]["cn"]); 

                // Search row with same identification attribute value. If it is not found an new row is added
                int fila = SearchOrAddRowByLink(dg, dDatos["" + i], lLinks); 
                dg[Column, fila].Value = dDatos["" + i]["cn"];      // Write value of CN i assigned column
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void FillAtribRows(DataGridView dg, Dictionary<string, Dictionary<string, string>> dDatos, string Column, bool Primero = false) {
            if (dDatos.Count == 0) {
                for (int i = 0; i < dg.Rows.Count; i++){
                    dg[Column, i].Value = "";
                    dg[Column, i].Tag = null;// If tag is null it cannot be edited
                    }
                return;
                }
            dg.Columns[Column].Tag = dDatos["0"]["#ID#"]; // Column tag contains ID (if it is a AD data source it contains DN value)
            for (int i = 0; i < dg.Rows.Count; i++) {
                if (dg[Column, i].Value == null || dg[Column, i].Value.ToString() != dDatos["0"][dg[0, i].Value.ToString()])
                    dg[Column, i].Style.ForeColor = Config.ForeColor_Change;
                else
                    dg[Column, i].Style.ForeColor = Config.ForeColor_Normal;//dg[0, i].Style.ForeColor;//Color.Black;
                
                // Tag value contains the same value showed in cell to allow undo operations
                dg[Column, i].Value = dDatos["0"][dg[0, i].Value.ToString()];
                dg[Column, i].Tag = dDatos["0"][dg[0, i].Value.ToString()];
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void MarkDifferences(DataGridView dg, int Column) {
            if (Column < 2) Column = 2;
            string sBase = "", sComp = "", sBaseSort = "", sCompSort = "";
            string[] lBase = null, lComp = null;
            for (int r = 0; r < dg.Rows.Count; r++) {
                sBase = dg[Column, r].Value.ToString();
                lBase = sBase.Split(Config.SeparatorC);
                Array.Sort(lBase);
                sBaseSort = string.Join(Config.Separator, lBase);
                for (int c = 2; c < dg.ColumnCount; c++) {
                    sComp = dg[c, r].Value.ToString();
                    lComp = sComp.Split(Config.SeparatorC);
                    Array.Sort(lComp);
                    sCompSort = string.Join(Config.Separator, lComp);
                    if (sBase == sComp)
                        dg[c, r].Style.ForeColor = Config.ForeColor_Normal;
                    else if (sBaseSort == sCompSort)
                        dg[c, r].Style.ForeColor = Config.ForeColor_Sort;
                    else
                        dg[c, r].Style.ForeColor = Config.ForeColor_Change;
                    }
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void ResetRows(DataGridView dg) {
            DataGridViewColumn c = dg.SortedColumn;
            if (c != null) c.SortMode = DataGridViewColumnSortMode.NotSortable;
            dg.Rows.Clear();
            if (c != null) c.SortMode = DataGridViewColumnSortMode.Automatic;
        }
        //_________________________________________________________________________________________________________
        // Add required empty rows
        //_________________________________________________________________________________________________________
        private static void UpdateRows(DataGridView dg, int nRows) {
            for (int i = dg.Rows.Count; i < nRows; i++)
                dg.Rows.Add();
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void RowHeaders(DataGridView dg) {
            //int rowNumber = 1;
            //foreach (DataGridViewRow row in dg.Rows) {
            //    if (row.IsNewRow) continue;
            //    row.HeaderCell.Value = (rowNumber++).ToString();
            //    }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string GetCurrentRow(DataGridView dg) {
            return dg.RowCount > 0 && dg.SelectedCells.Count >= 1 ? dg[0, dg.SelectedCells[0].RowIndex].Value.ToString() : "";
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void SetCurrentRow(DataGridView dg, string sSelectedRow) {
            for (int i = 0; i < dg.RowCount; i++) {
                if (dg[0, i].Value != null && dg[0, i].Value.ToString() == sSelectedRow) {
                    dg.ClearSelection();
                    dg[0, i].Selected = true;
                    dg.FirstDisplayedScrollingRowIndex = i;
                    dg.CurrentCell = dg[0, i];
                    break;
                    }
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void LoadObjectsList(List<string> lLinks, DataGridView dg, DSources oDSrc, int max = 0, string sFilter = "") {
            Dictionary<string, Dictionary<string, string>> dV;
            string sSelectedRow = GridHelper.GetCurrentRow(dg);
            GridHelper.ResetRows(dg);
            LogHelper.Prg(oDSrc.DS.Count, 0); 
            foreach(KeyValuePair<string,IDataSource> ds in oDSrc.DS){
                LogHelper.Msg("Loading Data from " + ds.Value.Name);
                dV = ds.Value.FindAllEntries(max, "", sFilter);
                GridHelper.FillObjRows(lLinks, dg, dV, ds.Key);
                LogHelper.Prg(0);
                }
            GridHelper.SetCurrentRow(dg, sSelectedRow);
            LogHelper.Prg(); LogHelper.Msg(); 
            }
        //_________________________________________________________________________________________________________
        // Unused by the moment. When row to be refresh is lost you must use CleanAttributesList
        //_________________________________________________________________________________________________________
        public static string ConfirmRowForRefresh(DataGridView dgo, string RowForRefresh) {
            if (RowForRefresh == null) return null; 
            for (int i = 0; i < dgo.Rows.Count; i++) {
                if (RowForRefresh == dgo[0, i].Value.ToString()) return RowForRefresh;
                }
            return null;
            }
        //_________________________________________________________________________________________________________
        // Unused by the moment. 
        //_________________________________________________________________________________________________________
        public static void CleanAttributesList(DataGridView dg, DSources oDSrc, DataGridView dgo, string RowForRefresh) {
            foreach (KeyValuePair<string, IDataSource> ds in oDSrc.DS) {
                GridHelper.CleanAtribRows(dg, ds.Key);
                }
            }
        //_________________________________________________________________________________________________________
        // Unused by the moment. 
        //_________________________________________________________________________________________________________
        public static void CleanAtribRows(DataGridView dg, string Column) {
            for (int i = 0; i < dg.Rows.Count; i++) {
                dg[Column, i].Value = "";
                dg[Column, i].Tag = null;// If tag is null it cannot be edited
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void LoadAttributesList(DataGridView dg, DSources oDSrc, DataGridView dgo, string RowForRefresh) {
            int row = SearchRowByLink(dgo, RowForRefresh);
            if (row < 0) {
                for (int i = 0; i < dgo.Rows.Count; i++) {
                    if (RowForRefresh == dgo[0, i].Value.ToString()) row = i; 
                    }
                }
            Dictionary<string, Dictionary<string, string>> dV;
            foreach (KeyValuePair<string, IDataSource> ds in oDSrc.DS) {
            if (row >= 0 && dgo[dgo.Columns[ds.Key].Index, row].Value != null) {
                dV = ds.Value.FindAllAttributes(dgo[dgo.Columns[ds.Key].Index, row].Value.ToString());
                GridHelper.FillAtribRows(dg, dV, ds.Key);
                }
            else
                CleanAtribRows(dg, ds.Key);
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void DeleteRow(DataGridView dg, DSources oDSrc, DataGridView dgo,string Column) {
            string cn = "";
            foreach (KeyValuePair<string, IDataSource> ds in oDSrc.DS) {
                if (dgo[dgo.Columns[ds.Key].Index, dgo.CurrentRow.Index].Value != null && (Column == "" || Column == ds.Key)) {
                    cn = dgo[dgo.Columns[ds.Key].Index, dgo.CurrentRow.Index].Value.ToString();
                    ds.Value.DeleteEntry(cn);
                    }
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void UpdateAttributeValue(DataGridView dg, DSources oDSrc, int Row, int Col) {
            string newValue = dg[Col, Row].Value != null ? dg[Col, Row].Value.ToString() : "";
            if (dg[Col, Row].Tag == null) { dg[Col, Row].Value = ""; return; }// If Tag == null there does not exist a register for that data source so, it cannot be edited
            if (newValue == dg[Col, Row].Tag.ToString()) return;// si no cambia el valor, termina
            // First two columns cannot be edited
            if (Col < 2 && dg[Col, Row].Value.ToString() != dg[Col, Row].Tag.ToString()) { dg[Col, Row].Value = dg[Col, Row].Tag.ToString(); return; }
            string sColumn = dg.Columns[Col].Name;// Column name identify tue data source
            string sID = dg.Columns[Col].Tag.ToString();// Column tag contains ID (if it is a AD data source it contains DN value)
            string value = oDSrc.DS[sColumn].UpdateAttributeValue(sID, sColumn, dg[0, Row].Value.ToString(), newValue, dg[Col, Row].Tag.ToString());
            dg[Col, Row].Value = value;// Update value in screen
            dg[Col, Row].Tag = value;// Tag value contains the same value showed in cell to allow undo operations
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void CopyRow(DataGridView dgo, DataGridView dgd, int Row, bool bSort, bool bMultiValue) {
            bool bHayImgs = false; 
            dgd.Rows.Clear();
            dgd.Columns.Clear();
            dgd.Columns.Add("R", "R");
            dgd.Columns.Add(dgo[0, Row].Value.ToString(), dgo[0, Row].Value.ToString());

            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            dgd.Columns.Add(imgCol);

            dgd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgd.Columns[0].Width = 30;
            for (int i = 2; i < dgo.Columns.Count; i++) {
                dgd.Rows.Add();
                dgd[0, dgd.Rows.Count - 1].Value = " ";
                dgd[1, dgd.Rows.Count - 1].Value = dgo.Columns[i].Name;
                dgd[1, dgd.Rows.Count - 1].Style.ForeColor = Color.Blue;
                dgd[1, dgd.Rows.Count - 1].Style.Font=new Font(dgd.Font, FontStyle.Bold);
                dgd[2, dgd.Rows.Count - 1].Value = Config.DefImage;
                // If it is not multiline it is shown as is
                if (!bMultiValue) {
                    dgd.Rows.Add();
                    dgd[0, dgd.Rows.Count - 1].Value = "";
                    dgd[1, dgd.Rows.Count - 1].Value = (dgo[i, Row].Value == null ? "" : dgo[i, Row].Value).ToString();
                    if (Config.ExistImage(dgo.Columns[i].Name, dgo[0, Row].Value.ToString(), 0)) {
                        Bitmap img = Config.GetImage(dgo.Columns[i].Name, dgo[0, Row].Value.ToString(), 0); 
                        dgd[2, dgd.Rows.Count - 1].Value = img;
                        dgd.AutoResizeRow(dgd.Rows.Count - 1, DataGridViewAutoSizeRowMode.AllCells);
                        dgd.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        bHayImgs = true;
                        }
                    else
                        dgd[2, dgd.Rows.Count - 1].Value = Config.DefImage;
                        
                    continue;
                    }
                // If it is multivalue it is splitted by Config.SeparatorC 
                List<string> tmp = (dgo[i, Row].Value == null ? "" : dgo[i, Row].Value).ToString().Split(Config.SeparatorC).ToList();
                if (bSort) tmp.Sort();
                for (int j = 0; j < tmp.Count; j++) {
                    dgd.Rows.Add();
                    dgd[0, dgd.Rows.Count - 1].Value = "";
                    dgd[1, dgd.Rows.Count - 1].Value = tmp[j];
                    if (Config.ExistImage(dgo.Columns[i].Name, dgo[0, Row].Value.ToString(), j)) {
                        Bitmap img = Config.GetImage(dgo.Columns[i].Name, dgo[0, Row].Value.ToString(), j); 
                        dgd[2, dgd.Rows.Count - 1].Value = img;
                        dgd.AutoResizeRow(dgd.Rows.Count - 1, DataGridViewAutoSizeRowMode.AllCells);
                        dgd.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        bHayImgs = true;
                        }
                    else
                        dgd[2, dgd.Rows.Count - 1].Value = Config.DefImage;
                        
                    }
                }
            for (int i = 0; i < dgd.Rows.Count; i++) {
                if (dgd[0, i].Value.ToString() == " ") continue;
                string s = dgd[1, i].Value.ToString();
                if (s == "") continue;
                int cont = 0;
                for (int j = 0; j < dgd.Rows.Count; j++) {
                    if (dgd[0, j].Value.ToString() != " " && dgd[1, j].Value.ToString() == s) cont++;
                    }
                dgd[0, i].Value = "" + cont;
                }
            // If there does not exists a image column, the last column is removed
            if (!bHayImgs) dgd.Columns.RemoveAt(2);
            }
        //_________________________________________________________________________________________________________
        // Mark a row with different color
        //_________________________________________________________________________________________________________
        public static void MarkRow(DataGridView dg, int Row) {
            dg[0, Row].Style.ForeColor = dg[0, Row].Style.ForeColor == Config.ForeColor_Mark ? Config.ForeColor_Normal : Config.ForeColor_Mark;
            dg[1, Row].Style.ForeColor = dg[1, Row].Style.ForeColor == Config.ForeColor_Mark ? Config.ForeColor_Normal : Config.ForeColor_Mark;
            }
        //_________________________________________________________________________________________________________
        // Mark a all rows in list by alias
        //_________________________________________________________________________________________________________
        public static void MarkRows( DataGridView dg, List<string> Rows ) {
            foreach ( string s in Rows ) {
                for ( int i = 0; i < dg.Rows.Count; i++ ) {
                    if ( dg[0, i].Value.ToString() == s ) { MarkRow(dg, i); break; }
                    }
                }
            }
        }
    }
