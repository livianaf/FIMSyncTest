using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using FIMSyncTest.Sources;
using FIMSyncTest.Helpers;
using FIMSyncTest.Profiles;

namespace FIMSyncTest {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    public partial class fFIMSyncTest : Form {
        private DSources oDSrc { get; set; }
        private string XlxConfig = "FIMSyncData.xlsx";
        private string DatasourcesSheet = "Datasources";
        private bool Initialized { get; set; }
        public string RowForRefresh { get; set; }
        private Dictionary<string, cWorker> dWorkers { get; set; }
        private fDetails oDetails { get; set; }
        private MacroHelper mh { get; set; }
        private List<string> SelectedRows = new List<string>();
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public fFIMSyncTest() {
            InitializeComponent();
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void Form1_Load(object sender, EventArgs e) {
            Config.ResetLogs(); 
            LogHelper.cMsg = this.cMsg;
            LogHelper.cProgBar = this.cProgBar;
            cWinLog.Init();
            ExtensionHelper.Init(this, dgValues);
            ExtensionHelper.EnableNewObject(mNewObject);
            cObjClass.Visible = mNewObject.Visible;
            cBU.Visible = mNewObject.Visible;
            ExtensionHelper.EnableCustomActions(mCustomActions);
            ExtensionHelper.EnableDelete(mDelete);
            ExtensionHelper.EnableDeleteMV(mDeleteMV);
            ExtensionHelper.EnableProfileActions(mRunMAProfiles);
            ExtensionHelper.EnableCustomMenus(mMenu, mExecCustomMenu_Click);
            cMaxRunProfiles.Visible = mRunMAProfiles.Visible;
            mProfilesOptions.Visible = mRunMAProfiles.Visible;
            ExtensionHelper.EnableMacroActions(mExecMacro);
            cLstMacros.Visible = mExecMacro.Visible;
            mEditMacro.Visible = mExecMacro.Visible;
            mShowLogWindow.Visible = mExecMacro.Visible;
            if (!mExecMacro.Visible) 
                cFilter.Size = new System.Drawing.Size(300, 23);
            if (System.IO.File.Exists(Config.LogFile)) System.IO.File.Delete(Config.LogFile);
            ReadPrevScreenData();
            oDetails = new fDetails();
            MacroHelper.LoadMacroList(cLstMacros);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void ReadPrevScreenData() {
            if (!System.IO.File.Exists(Config.DataFile)) return;
            string[] lValues = System.IO.File.ReadAllText(Config.DataFile).Split('\n');
            if ( lValues.Length > 0 ) cMax.Text = lValues[0];
            if ( lValues.Length > 1 ) cFilter.Text = lValues[1];
            if ( lValues.Length > 2 ) SelectedRows = new List<string>(lValues[2].Split(','));
            if ( lValues.Length > 3 && lValues[3].Split(',').Length == 2 ) { XlxConfig = lValues[3].Split(',')[0]; DatasourcesSheet = lValues[3].Split(',')[1]; }

            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void SaveScreenData() {
            if (System.IO.File.Exists(Config.DataFile)) System.IO.File.Delete(Config.DataFile);
            if ( SelectedRows.Contains("") ) SelectedRows.Remove("");
            System.IO.File.WriteAllText(Config.DataFile, cMax.Text + '\n' + cFilter.Text + '\n' + string.Join(",", SelectedRows) + '\n' + XlxConfig + ',' + DatasourcesSheet);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void dgvValues_RowEnter(object sender, DataGridViewCellEventArgs e) {
            // Row Tag contains complete detail of Excel Row after InitGrid is executed but this event is fired before
            if (dgValues.Rows[e.RowIndex].Tag == null) return;
            GridHelper.LoadAttributeData(dgAux, (DataRow)dgValues.Rows[e.RowIndex].Tag);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void dgv_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e) {
            DataGridView dg = (DataGridView)sender;
            if (dg.Name != dgObjs.Name) dgObjs.Columns[e.Column.Index].Width = dg.Columns[e.Column.Index].Width;
            if (dg.Name != dgAux.Name) dgAux.Columns[e.Column.Index].Width = dg.Columns[e.Column.Index].Width;
            if (dg.Name != dgValues.Name) dgValues.Columns[e.Column.Index].Width = dg.Columns[e.Column.Index].Width;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void dgv_Scroll(object sender, ScrollEventArgs e) {
            DataGridView dg = (DataGridView)sender;
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
                if (dg.Name != dgObjs.Name) dgObjs.HorizontalScrollingOffset = dg.HorizontalScrollingOffset;
                if (dg.Name != dgAux.Name) dgAux.HorizontalScrollingOffset = dg.HorizontalScrollingOffset;
                if (dg.Name != dgValues.Name) dgValues.HorizontalScrollingOffset = dg.HorizontalScrollingOffset;
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void dgvValues_ColumnSortModeChanged(object sender, DataGridViewColumnEventArgs e) {
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void dgvValues_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            dgObjs.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.Programmatic;
            dgObjs.Sort(dgObjs.Columns[e.ColumnIndex], dgValues.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
            GridHelper.RowHeaders(dgObjs);
            GridHelper.RowHeaders(dgValues);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        internal void mLoadData_Click(object sender, EventArgs e) {
            if (!LoadConfig()) return;
            Cursor.Current = Cursors.WaitCursor;
            dgObjs.Enabled = false;
            GridHelper.LoadObjectsList(Config.lValuesLinks, dgObjs, oDSrc, GetMax(cMax), cFilter.Text);
            mNewObject.Enabled = true;
            dgObjs.Enabled = true;
            mDelete.Enabled = (dgObjs.RowCount > 0);
            mDeleteMV.Enabled = (dgObjs.RowCount > 0);
            mCustomActions.Enabled = (dgObjs.RowCount > 0);
            mRunMAProfiles.Enabled = true;// (dgObjs.RowCount > 0);
            mDetails.Enabled = (dgObjs.RowCount > 0);
            // This calls avoid ArgumentOutOfRangeException when user go down in dgv
            dgObjs.PerformLayout();
            dgAux.PerformLayout();
            dgValues.PerformLayout();
            // Update macro list
            MacroHelper.LoadMacroList(cLstMacros);
            Cursor.Current = Cursors.Default;
            // update last screen data
            SaveScreenData();
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private int GetMax(ToolStripTextBox cMaxCtrl) {
            int max = 0;
            int.TryParse(cMaxCtrl.Text, out max);
            if (max < 1) max = 1000;
            return max;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private bool LoadConfig() {
            if (Initialized && Control.ModifierKeys != Keys.Shift) return true;// If it is already initialized and Keys.Shift is not pressed, return
            if (!File.Exists(XlxConfig)) return false;//!File.Exists(XlxConfig) && !File.Exists(@"F:\" + XlxConfig)
            Config.InitConfig(XlxConfig, DatasourcesSheet);//File.Exists(XlxConfig) ? XlxConfig : @"F:\" + XlxConfig
            GridHelper.InitializeDataGridView(dgObjs); 
            GridHelper.InitializeDataGridView(dgAux); 
            GridHelper.InitializeDataGridView(dgValues, true); 
            GridHelper.InitializeDataGridView(oDetails.dgv);
            // Scrollbars
            dgObjs.ScrollBars = ScrollBars.Vertical;
            dgAux.ScrollBars = ScrollBars.None;

            LogHelper.Msg("Connecting Data Sources");
            this.Refresh();
            oDSrc = new DSources(Config.tFDatos, Config.tAttributes);
            GridHelper.InitGrid(dgObjs, Config.lObjCols); 
            GridHelper.InitGrid(dgAux, Config.lAuxCols, null, 1); 
            GridHelper.InitGrid(dgValues, Config.lValuesCols, Config.dAtribs);
            GridHelper.MarkRows(dgValues, SelectedRows);

            cObjClass.Items.Clear();
            cObjClass.Items.AddRange(ExtensionHelper.GetNewObjectClasses());
            if (cObjClass.Items.Count > 0) cObjClass.SelectedIndex = 0;
            cBU.Items.Clear();
            cBU.Items.AddRange(Config.dConfig["ADSource Alias"].Split(';'));
            if (cBU.Items.Count > 0) cBU.SelectedIndex = 0;
            mProfilesOptions.Items.Clear();
            mProfilesOptions.Items.AddRange(Config.dConfig["Profile Options"].Split(';'));
            if (mProfilesOptions.Items.Count > 0) mProfilesOptions.SelectedIndex = 0;
            // Optional: Change with of dropdownlist with a functionality of available in MacroHelper
            MacroHelper.ChangeListBoxWidth(cObjClass);
            MacroHelper.ChangeListBoxWidth(cBU);
            MacroHelper.ChangeListBoxWidth(mProfilesOptions);
            // Profiles config
            dWorkers = new Dictionary<string, cWorker>();
            dWorkers.Add("1", new cWorker(null, null, lbPfStatus1, null));
            dWorkers.Add("2", new cWorker(null, null, lbPfStatus2, null));
            dWorkers.Add("3", new cWorker(null, null, lbPfStatus3, null));
            dWorkers.Add("4", new cWorker(null, null, lbPfStatus4, null));
            dWorkers.Add("5", new cWorker(null, null, lbPfStatus5, null));
            dWorkers.Add("6", new cWorker(null, null, lbPfStatus6, null));

            // Load servers info
            FIMServers.LoadServers(dWorkers);

            Initialized = true;
            return true;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        internal void dgObjs_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            RowForRefresh = dgObjs[0, e.RowIndex].Value.ToString();
            GridHelper.LoadAttributesList(dgValues, oDSrc, dgObjs, RowForRefresh);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        internal void dgValues_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            // This Try Capture: Operation is not valid because it results in a reentrant call to the SetCurrentCellAddressCore function.
            // It is throw sometimes a second time during catch too ?!?
            try {
                try {
                    GridHelper.UpdateAttributeValue(dgValues, oDSrc, e.RowIndex, e.ColumnIndex);
                    }
                // This Try Capture: Operation is not valid because it results in a reentrant call to the SetCurrentCellAddressCore function.
                // It is throw if a user is editing one Cell and focus is changed to other Cell with a mouse click
                catch ( Exception ex ) {
                    LogHelper.Msg(ex.Message);
                    }
                }
            catch ( Exception ) { }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        internal void mNewObject_Click(object sender, EventArgs e) {
            if (cObjClass.Text == "" || cBU.Text == "") return;
            if (ExtensionHelper.NewObject(cObjClass.Text, cBU.Text, Config.dConfig, Config.dDefValues, oDSrc.DS))
                mLoadData_Click(sender, e);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        internal void mDelete_Click(object sender, EventArgs e) { Delete(sender, e); }
        internal void Delete(object sender, EventArgs e, string Column = "") {
            if (dgObjs.RowCount == 0) return;
            if (dgObjs.CurrentRow.Index < 0) return;
            GridHelper.DeleteRow(dgValues, oDSrc, dgObjs, Column);
            mLoadData_Click(sender, e);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        internal void mCustActions_Click(object sender, EventArgs e) {
            // If LoadData is pending, return without errors
            if (!FIMServers.Loaded) return;
            ExtensionHelper.CustomActions(oDSrc.DS);
            mLoadData_Click(sender, e);
            if (RowForRefresh != null) GridHelper.LoadAttributesList(dgValues, oDSrc, dgObjs, RowForRefresh);// LoadObjectsList is already completed in mLoadData
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        internal void mRunMAProfiles_Click(object sender, EventArgs e) {
            int i = 1;
            // If LoadData is pending, return without errors
            if (!FIMServers.Loaded) {
                cTimerNext.Enabled = true;// Enable a timer to allow pending refresh before continue with macro
                return;
                }
            mRunMAProfiles.Enabled = false;
            foreach (string s in FIMServers.Keys) {
                FIMServers.Item(s).num = GetMax(cMaxRunProfiles);
                FIMServers.Item(s).runPf = mProfilesOptions.Text;
                if (i <= 6) {
                    dWorkers["" + i].oFIMServer = FIMServers.Item(s);
                    dWorkers["" + i++].Run(FIMServers.Item(s));
                    }
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void fFIMSyncTest_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.F5 && RowForRefresh != null) {
                GridHelper.LoadObjectsList(Config.lValuesLinks, dgObjs, oDSrc, GetMax(cMax), cFilter.Text);
                GridHelper.LoadAttributesList(dgValues, oDSrc, dgObjs, RowForRefresh);
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void lbPfStatus_MouseEnter(object sender, EventArgs e) {
            ToolStripStatusLabel c = (ToolStripStatusLabel)sender;
            if (c.ToolTipText.Trim() == "") return;
            int nLineas = c.ToolTipText.Split('\n').Length;
            Size size = TextRenderer.MeasureText(c.ToolTipText, this.Font);
            this.toolTip1.Show(c.ToolTipText, statusStrip1, c.Bounds.Right, c.Bounds.Top - size.Height - 20);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void lbPfStatus_MouseLeave(object sender, EventArgs e) {
            this.toolTip1.Hide(statusStrip1);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void lbPfStatus_TextChanged(object sender, EventArgs e) {
            if (lbPfStatus1.Tag.ToString() == "END" && lbPfStatus2.Tag.ToString() == "END" && lbPfStatus3.Tag.ToString() == "END" && lbPfStatus4.Tag.ToString() == "END" && lbPfStatus5.Tag.ToString() == "END" && lbPfStatus6.Tag.ToString() == "END") {
                mRunMAProfiles.Enabled = true;
                if (RowForRefresh != null) {
                    GridHelper.LoadObjectsList(Config.lValuesLinks, dgObjs, oDSrc, GetMax(cMax), cFilter.Text); 
                    GridHelper.LoadAttributesList(dgValues, oDSrc, dgObjs, RowForRefresh);
                    }
                cTimerNext.Enabled = true;// Enable a timer to allow pending refresh before continue with macro
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void BorraFichLogs() {
            string[] lPaths = (Config.dConfig.ContainsKey("Delete Logs Paths") ? Config.dConfig["Delete Logs Paths"] : "").Split(';');
            string[] lMasks = (Config.dConfig.ContainsKey("Delete Logs Masks") ? Config.dConfig["Delete Logs Masks"] : "").Split(';');
            for (int i = 0; i < lPaths.Length; i++) {
                if (lPaths[i] == "" || !Directory.Exists(lPaths[i])) continue;
                for (int j = 0; j < lMasks.Length; j++) {
                    if (lMasks[j] == "" || lMasks[j] == "*.*") continue;
                    foreach (string f in Directory.EnumerateFiles(lPaths[i], lMasks[j])) {
                        try { File.Delete(f); }
                        catch (Exception) { }
                        }
                    }
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void mDetails_Click(object sender, EventArgs e) {
            if (dgValues.RowCount == 0) return;
            if (dgValues.CurrentRow.Index < 0) return;
            if (Control.ModifierKeys == Keys.Control) {
                GridHelper.MarkDifferences(dgValues, dgValues.CurrentCell.ColumnIndex);
                return;
                }
            bool bMultiValue = false;
            if (dgAux[1, 0].Value != null && dgAux[1, 0].Value.ToString().ToUpper() != "N") bMultiValue = true;
            GridHelper.CopyRow(dgValues, oDetails.dgv, dgValues.CurrentRow.Index, (Control.ModifierKeys != Keys.Shift), bMultiValue);
            oDetails.ShowDialog();
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void fFIMSyncTest_FormClosing(object sender, FormClosingEventArgs e) {
            cWinLog.Dispose(); 
            oDetails.Close();
            oDetails.Dispose();
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void mDeleteMV_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Content of all connected Meteverse databases will be deleted. \nAre you sure?", "ALERT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            foreach (KeyValuePair<string, IDataSource> ds in oDSrc.DS) {
                if (Config.dConfig.ContainsKey("DeleteMV")) ds.Value.DeleteAllEntries(Config.dConfig["DeleteMV"]);
                if (Config.dConfig.ContainsKey("DeleteMVHist")) ds.Value.DeleteAllEntries(Config.dConfig["DeleteMVHist"]);
                }
            mLoadData_Click(sender, e);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void dgValues_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) { mDetails_Click(sender, e); }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void dgValues_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) {
            if (dgValues[0, e.RowIndex].Style.ForeColor == Config.ForeColor_Mark) {
                if (dgValues.Rows[e.RowIndex].DefaultCellStyle.BackColor != Config.BackColor_Mark)
                    dgValues.Rows[e.RowIndex].DefaultCellStyle.BackColor = Config.BackColor_Mark;//dgValues[0, e.RowIndex].Style.BackColor = Color.DarkSeaGreen;
                }
            else if (dgValues.Rows[e.RowIndex].DefaultCellStyle.BackColor == Config.BackColor_Mark)
                dgValues.Rows[e.RowIndex].DefaultCellStyle.BackColor = Config.BackColor_Normal;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void dgValues_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                var hti = dgValues.HitTest(e.X, e.Y);
                if (hti.RowIndex >= 0 && hti.RowIndex < dgValues.Rows.Count) {
                    GridHelper.MarkRow(dgValues, hti.RowIndex);
                    if ( SelectedRows.Contains(dgValues[0, hti.RowIndex].Value.ToString()) ) SelectedRows.Remove(dgValues[0, hti.RowIndex].Value.ToString());
                    else SelectedRows.Add(dgValues[0, hti.RowIndex].Value.ToString());
                    SaveScreenData();
                    }
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void mExecMacro_Click(object sender, EventArgs e) {
            if (cLstMacros.Text == "") return;
            mExecMacro.Enabled = false;

            MacroHelper.Running = false;
            MacroHelper.ExecName = "";
            MacroHelper.NextScriptName = "";
            MacroHelper.MustCallBack = false;

            if (mh == null) mh = new MacroHelper(this, dgValues);
            if (MacroHelper.AboutConditions(cMaxRunProfiles)) {
                string sMacro = MacroHelper.NewAboutMacro();
                mh.ExecMacro(sMacro);
                File.Delete(sMacro);
                }
            else{
                mh.ExecMacro(MacroHelper.GetSelectedMacroPath(cLstMacros));
                }
            //mExecMacro.Enabled = true; // It is not activated till the end of macro execution
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void mExecCustomMenu_Click(object sender, EventArgs e) {
            ToolStripMenuItem MnuItem = (ToolStripMenuItem)sender;
            string sMacroName = MacroHelper.GetSelectedMacroPath(MnuItem.Tag.ToString());
            if (!File.Exists(sMacroName)) return;

            mExecMacro.Enabled = false;

            MacroHelper.Running = false;
            MacroHelper.ExecName = "";
            MacroHelper.NextScriptName = "";
            MacroHelper.MustCallBack = false;

            if (mh == null) mh = new MacroHelper(this, dgValues);
            mh.ExecMacro(sMacroName);
            //mExecMacro.Enabled = true; // It is not activated till the end of macro execution
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void cTimerStart_Tick(object sender, EventArgs e) {
            cTimerStart.Enabled = false;
            mRunMAProfiles_Click(sender, e);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void cTimerNext_Tick(object sender, EventArgs e) {
            cTimerNext.Enabled = false;
            if (cLstMacros.Text == "") return;
            if (mh == null) return;

            MacroHelper.Running = false;
            if (!MacroHelper.MustCallBack) return;
            MacroHelper.MustCallBack = false;
            if (MacroHelper.NextScriptName == "") MacroHelper.NextScriptName = MacroHelper.GetSelectedMacroPath(cLstMacros);

            mh.ExecMacroNextSectionAfterAsyncCall(MacroHelper.NextScriptName);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void mEditMacro_Click(object sender, EventArgs e) {
            string sMacroPath = MacroHelper.GetSelectedMacroPath(cLstMacros);
            if (Control.ModifierKeys == Keys.Control) {
                sMacroPath = MacroHelper.NewMacro((File.Exists(sMacroPath) ? "" : sMacroPath));
                MacroHelper.LoadMacroList(cLstMacros);
                }

            try { System.Diagnostics.Process.Start(sMacroPath); }
            catch (Exception) { }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        internal void mShowLogWindow_Click(object sender, EventArgs e) { cWinLog.Show(this); }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private void cTimerNextMacro_Tick(object sender, EventArgs e) {
            cTimerNextMacro.Enabled = false;
            if (cLstMacros.Text == "") return;
            if (mh == null) return;

            MacroHelper.Running = false;
            MacroHelper.MustCallBack = false;
            mh.ExecNextMacro();
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        internal bool ReadAgentResult(int Index, ref bool bError, ref bool bChanges, ref string sMsg) {
            ToolStripStatusLabel lbl;
            switch (Index) {
                case 1: lbl = lbPfStatus1; break;
                case 2: lbl = lbPfStatus2; break;
                case 3: lbl = lbPfStatus3; break;
                case 4: lbl = lbPfStatus4; break;
                case 5: lbl = lbPfStatus5; break;
                case 6: lbl = lbPfStatus6; break;
                default: return false;
                }
            bError = (lbl.ForeColor == Color.Red);
            bChanges = (lbl.ForeColor == Color.Blue);
            sMsg = lbl.ToolTipText;
            return true;
            }
        }
    }
