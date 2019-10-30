namespace FIMSyncTest {
    partial class fFIMSyncTest {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
                }
            base.Dispose(disposing);
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fFIMSyncTest));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.cMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbPfStatus1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbPfStatus2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbPfStatus3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbPfStatus4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbPfStatus5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbPfStatus6 = new System.Windows.Forms.ToolStripStatusLabel();
            this.cProgBar = new System.Windows.Forms.ToolStripProgressBar();
            this.dgObjs = new System.Windows.Forms.DataGridView();
            this.mMenu = new System.Windows.Forms.MenuStrip();
            this.mLoadData = new System.Windows.Forms.ToolStripMenuItem();
            this.cMax = new System.Windows.Forms.ToolStripTextBox();
            this.cFilter = new System.Windows.Forms.ToolStripTextBox();
            this.mNewObject = new System.Windows.Forms.ToolStripMenuItem();
            this.cObjClass = new System.Windows.Forms.ToolStripComboBox();
            this.cBU = new System.Windows.Forms.ToolStripComboBox();
            this.mCustomActions = new System.Windows.Forms.ToolStripMenuItem();
            this.mDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteMV = new System.Windows.Forms.ToolStripMenuItem();
            this.mRunMAProfiles = new System.Windows.Forms.ToolStripMenuItem();
            this.mProfilesOptions = new System.Windows.Forms.ToolStripComboBox();
            this.cMaxRunProfiles = new System.Windows.Forms.ToolStripTextBox();
            this.mDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.mExecMacro = new System.Windows.Forms.ToolStripMenuItem();
            this.cLstMacros = new System.Windows.Forms.ToolStripComboBox();
            this.mEditMacro = new System.Windows.Forms.ToolStripMenuItem();
            this.mShowLogWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.cDSUsrs = new System.Data.DataSet();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgValues = new System.Windows.Forms.DataGridView();
            this.dgAux = new System.Windows.Forms.DataGridView();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cTimerStart = new System.Windows.Forms.Timer(this.components);
            this.cTimerNext = new System.Windows.Forms.Timer(this.components);
            this.cTimerNextMacro = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgObjs)).BeginInit();
            this.mMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cDSUsrs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgAux)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cMsg,
            this.lbPfStatus1,
            this.lbPfStatus2,
            this.lbPfStatus3,
            this.lbPfStatus4,
            this.lbPfStatus5,
            this.lbPfStatus6,
            this.cProgBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 547);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1258, 24);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // cMsg
            // 
            this.cMsg.Name = "cMsg";
            this.cMsg.Size = new System.Drawing.Size(751, 19);
            this.cMsg.Spring = true;
            this.cMsg.Text = "    ";
            this.cMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbPfStatus1
            // 
            this.lbPfStatus1.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.lbPfStatus1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lbPfStatus1.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.lbPfStatus1.Name = "lbPfStatus1";
            this.lbPfStatus1.Size = new System.Drawing.Size(65, 19);
            this.lbPfStatus1.Tag = "END";
            this.lbPfStatus1.Text = "                  ";
            this.lbPfStatus1.ToolTipText = "          ";
            this.lbPfStatus1.MouseEnter += new System.EventHandler(this.lbPfStatus_MouseEnter);
            this.lbPfStatus1.MouseLeave += new System.EventHandler(this.lbPfStatus_MouseLeave);
            this.lbPfStatus1.TextChanged += new System.EventHandler(this.lbPfStatus_TextChanged);
            // 
            // lbPfStatus2
            // 
            this.lbPfStatus2.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.lbPfStatus2.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lbPfStatus2.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.lbPfStatus2.Name = "lbPfStatus2";
            this.lbPfStatus2.Size = new System.Drawing.Size(65, 19);
            this.lbPfStatus2.Tag = "END";
            this.lbPfStatus2.Text = "                  ";
            this.lbPfStatus2.ToolTipText = "          ";
            this.lbPfStatus2.MouseEnter += new System.EventHandler(this.lbPfStatus_MouseEnter);
            this.lbPfStatus2.MouseLeave += new System.EventHandler(this.lbPfStatus_MouseLeave);
            this.lbPfStatus2.TextChanged += new System.EventHandler(this.lbPfStatus_TextChanged);
            // 
            // lbPfStatus3
            // 
            this.lbPfStatus3.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.lbPfStatus3.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lbPfStatus3.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.lbPfStatus3.Name = "lbPfStatus3";
            this.lbPfStatus3.Size = new System.Drawing.Size(65, 19);
            this.lbPfStatus3.Tag = "END";
            this.lbPfStatus3.Text = "                  ";
            this.lbPfStatus3.ToolTipText = "          ";
            this.lbPfStatus3.MouseEnter += new System.EventHandler(this.lbPfStatus_MouseEnter);
            this.lbPfStatus3.MouseLeave += new System.EventHandler(this.lbPfStatus_MouseLeave);
            this.lbPfStatus3.TextChanged += new System.EventHandler(this.lbPfStatus_TextChanged);
            // 
            // lbPfStatus4
            // 
            this.lbPfStatus4.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.lbPfStatus4.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lbPfStatus4.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.lbPfStatus4.Name = "lbPfStatus4";
            this.lbPfStatus4.Size = new System.Drawing.Size(65, 19);
            this.lbPfStatus4.Tag = "END";
            this.lbPfStatus4.Text = "                  ";
            this.lbPfStatus4.ToolTipText = "          ";
            this.lbPfStatus4.MouseEnter += new System.EventHandler(this.lbPfStatus_MouseEnter);
            this.lbPfStatus4.MouseLeave += new System.EventHandler(this.lbPfStatus_MouseLeave);
            this.lbPfStatus4.TextChanged += new System.EventHandler(this.lbPfStatus_TextChanged);
            // 
            // lbPfStatus5
            // 
            this.lbPfStatus5.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.lbPfStatus5.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lbPfStatus5.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.lbPfStatus5.Name = "lbPfStatus5";
            this.lbPfStatus5.Size = new System.Drawing.Size(65, 19);
            this.lbPfStatus5.Tag = "END";
            this.lbPfStatus5.Text = "                  ";
            this.lbPfStatus5.ToolTipText = "          ";
            this.lbPfStatus5.MouseEnter += new System.EventHandler(this.lbPfStatus_MouseEnter);
            this.lbPfStatus5.MouseLeave += new System.EventHandler(this.lbPfStatus_MouseLeave);
            this.lbPfStatus5.TextChanged += new System.EventHandler(this.lbPfStatus_TextChanged);
            // 
            // lbPfStatus6
            // 
            this.lbPfStatus6.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.lbPfStatus6.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lbPfStatus6.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.lbPfStatus6.Name = "lbPfStatus6";
            this.lbPfStatus6.Size = new System.Drawing.Size(65, 19);
            this.lbPfStatus6.Tag = "END";
            this.lbPfStatus6.Text = "                  ";
            this.lbPfStatus6.ToolTipText = "          ";
            this.lbPfStatus6.MouseEnter += new System.EventHandler(this.lbPfStatus_MouseEnter);
            this.lbPfStatus6.MouseLeave += new System.EventHandler(this.lbPfStatus_MouseLeave);
            this.lbPfStatus6.TextChanged += new System.EventHandler(this.lbPfStatus_TextChanged);
            // 
            // cProgBar
            // 
            this.cProgBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.cProgBar.Name = "cProgBar";
            this.cProgBar.Size = new System.Drawing.Size(100, 18);
            // 
            // dgObjs
            // 
            this.dgObjs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgObjs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgObjs.Location = new System.Drawing.Point(0, 0);
            this.dgObjs.Name = "dgObjs";
            this.dgObjs.Size = new System.Drawing.Size(1258, 93);
            this.dgObjs.TabIndex = 2;
            this.dgObjs.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgv_ColumnWidthChanged);
            this.dgObjs.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgObjs_RowHeaderMouseDoubleClick);
            this.dgObjs.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgv_Scroll);
            // 
            // mMenu
            // 
            this.mMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mLoadData,
            this.cMax,
            this.cFilter,
            this.mNewObject,
            this.cObjClass,
            this.cBU,
            this.mCustomActions,
            this.mDelete,
            this.mDeleteMV,
            this.mRunMAProfiles,
            this.mProfilesOptions,
            this.cMaxRunProfiles,
            this.mDetails,
            this.mExecMacro,
            this.cLstMacros,
            this.mEditMacro,
            this.mShowLogWindow});
            this.mMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.mMenu.Location = new System.Drawing.Point(0, 0);
            this.mMenu.Name = "mMenu";
            this.mMenu.ShowItemToolTips = true;
            this.mMenu.Size = new System.Drawing.Size(1258, 27);
            this.mMenu.TabIndex = 3;
            this.mMenu.Text = "menuStrip1";
            // 
            // mLoadData
            // 
            this.mLoadData.Name = "mLoadData";
            this.mLoadData.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mLoadData.Size = new System.Drawing.Size(72, 19);
            this.mLoadData.Text = "L&oad Data";
            this.mLoadData.ToolTipText = "F5 or Click = Load Data\r\nShift + Click = Reload configuration file and Load Data";
            this.mLoadData.Click += new System.EventHandler(this.mLoadData_Click);
            // 
            // cMax
            // 
            this.cMax.Name = "cMax";
            this.cMax.Size = new System.Drawing.Size(30, 23);
            this.cMax.Text = "25";
            this.cMax.ToolTipText = "Limit of items to be recovered form AD.\r\n0 = Without limit.";
            // 
            // cFilter
            // 
            this.cFilter.Name = "cFilter";
            this.cFilter.Size = new System.Drawing.Size(100, 23);
            this.cFilter.ToolTipText = "Filter to be applied to list of objects. Character \'*\' is allowed.\r\nFilter is app" +
    "lied to attribute identified in ObjectAliasAttribute (Config sheet). ";
            // 
            // mNewObject
            // 
            this.mNewObject.Enabled = false;
            this.mNewObject.Name = "mNewObject";
            this.mNewObject.Size = new System.Drawing.Size(43, 19);
            this.mNewObject.Text = "&New";
            this.mNewObject.ToolTipText = "Extension not present!";
            this.mNewObject.Click += new System.EventHandler(this.mNewObject_Click);
            // 
            // cObjClass
            // 
            this.cObjClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cObjClass.Name = "cObjClass";
            this.cObjClass.Size = new System.Drawing.Size(75, 23);
            this.cObjClass.ToolTipText = "Class of new object";
            // 
            // cBU
            // 
            this.cBU.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBU.Name = "cBU";
            this.cBU.Size = new System.Drawing.Size(100, 23);
            this.cBU.ToolTipText = "Type of new object";
            // 
            // mCustomActions
            // 
            this.mCustomActions.Enabled = false;
            this.mCustomActions.Name = "mCustomActions";
            this.mCustomActions.Size = new System.Drawing.Size(110, 19);
            this.mCustomActions.Text = "Run Cust &Actions";
            this.mCustomActions.Click += new System.EventHandler(this.mCustActions_Click);
            // 
            // mDelete
            // 
            this.mDelete.Enabled = false;
            this.mDelete.Name = "mDelete";
            this.mDelete.Size = new System.Drawing.Size(52, 19);
            this.mDelete.Text = "&Delete";
            this.mDelete.ToolTipText = "Delete selected object";
            this.mDelete.Click += new System.EventHandler(this.mDelete_Click);
            // 
            // mDeleteMV
            // 
            this.mDeleteMV.Enabled = false;
            this.mDeleteMV.Name = "mDeleteMV";
            this.mDeleteMV.Size = new System.Drawing.Size(73, 19);
            this.mDeleteMV.Text = "Delete &MV";
            this.mDeleteMV.ToolTipText = "Delete all MV content";
            this.mDeleteMV.Click += new System.EventHandler(this.mDeleteMV_Click);
            // 
            // mRunMAProfiles
            // 
            this.mRunMAProfiles.Enabled = false;
            this.mRunMAProfiles.Name = "mRunMAProfiles";
            this.mRunMAProfiles.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.mRunMAProfiles.Size = new System.Drawing.Size(104, 19);
            this.mRunMAProfiles.Text = "&Run MA Profiles";
            this.mRunMAProfiles.ToolTipText = "F10 or Click = Run MA Profiles";
            this.mRunMAProfiles.Click += new System.EventHandler(this.mRunMAProfiles_Click);
            // 
            // mProfilesOptions
            // 
            this.mProfilesOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mProfilesOptions.Name = "mProfilesOptions";
            this.mProfilesOptions.Size = new System.Drawing.Size(75, 23);
            this.mProfilesOptions.ToolTipText = "Profile Alias";
            // 
            // cMaxRunProfiles
            // 
            this.cMaxRunProfiles.Name = "cMaxRunProfiles";
            this.cMaxRunProfiles.Size = new System.Drawing.Size(30, 23);
            this.cMaxRunProfiles.Text = "2";
            this.cMaxRunProfiles.ToolTipText = "Number of times the profile is executed";
            // 
            // mDetails
            // 
            this.mDetails.Enabled = false;
            this.mDetails.Name = "mDetails";
            this.mDetails.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.mDetails.Size = new System.Drawing.Size(54, 19);
            this.mDetails.Text = "&Details";
            this.mDetails.ToolTipText = "F12 = Show details of current row (sorted)\r\nShift Key + Click =  Show details of " +
    "current row (unsorted)\r\nCtrl Key + Click = Compare all values with current colum" +
    "n";
            this.mDetails.Click += new System.EventHandler(this.mDetails_Click);
            // 
            // mExecMacro
            // 
            this.mExecMacro.Name = "mExecMacro";
            this.mExecMacro.Size = new System.Drawing.Size(77, 19);
            this.mExecMacro.Text = "R&un Macro";
            this.mExecMacro.ToolTipText = "Execute selected macro";
            this.mExecMacro.Click += new System.EventHandler(this.mExecMacro_Click);
            // 
            // cLstMacros
            // 
            this.cLstMacros.AutoSize = false;
            this.cLstMacros.Name = "cLstMacros";
            this.cLstMacros.Size = new System.Drawing.Size(100, 23);
            this.cLstMacros.ToolTipText = "List of available macros";
            // 
            // mEditMacro
            // 
            this.mEditMacro.Name = "mEditMacro";
            this.mEditMacro.Size = new System.Drawing.Size(25, 19);
            this.mEditMacro.Text = "&E";
            this.mEditMacro.ToolTipText = "Click =  Edit selected macro with default editor\r\nCtrl Key + Click = Create and e" +
    "dit new macro file";
            this.mEditMacro.Click += new System.EventHandler(this.mEditMacro_Click);
            // 
            // mShowLogWindow
            // 
            this.mShowLogWindow.Name = "mShowLogWindow";
            this.mShowLogWindow.Size = new System.Drawing.Size(25, 19);
            this.mShowLogWindow.Text = "&L";
            this.mShowLogWindow.ToolTipText = "Show / Hide Macro Execution Log Window";
            this.mShowLogWindow.Click += new System.EventHandler(this.mShowLogWindow_Click);
            // 
            // cDSUsrs
            // 
            this.cDSUsrs.DataSetName = "NewDataSet";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgObjs);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgValues);
            this.splitContainer1.Size = new System.Drawing.Size(1258, 487);
            this.splitContainer1.SplitterDistance = 93;
            this.splitContainer1.TabIndex = 4;
            // 
            // dgValues
            // 
            this.dgValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgValues.Location = new System.Drawing.Point(0, 0);
            this.dgValues.Name = "dgValues";
            this.dgValues.Size = new System.Drawing.Size(1258, 390);
            this.dgValues.TabIndex = 2;
            this.dgValues.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgValues_CellEndEdit);
            this.dgValues.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvValues_ColumnHeaderMouseClick);
            this.dgValues.ColumnSortModeChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgvValues_ColumnSortModeChanged);
            this.dgValues.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgv_ColumnWidthChanged);
            this.dgValues.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvValues_RowEnter);
            this.dgValues.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgValues_RowHeaderMouseDoubleClick);
            this.dgValues.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgValues_RowPrePaint);
            this.dgValues.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgv_Scroll);
            this.dgValues.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgValues_MouseClick);
            // 
            // dgAux
            // 
            this.dgAux.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgAux.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgAux.Location = new System.Drawing.Point(0, 514);
            this.dgAux.Name = "dgAux";
            this.dgAux.Size = new System.Drawing.Size(1258, 33);
            this.dgAux.TabIndex = 2;
            this.dgAux.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgv_ColumnWidthChanged);
            this.dgAux.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgv_Scroll);
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // cTimerStart
            // 
            this.cTimerStart.Interval = 1000;
            this.cTimerStart.Tick += new System.EventHandler(this.cTimerStart_Tick);
            // 
            // cTimerNext
            // 
            this.cTimerNext.Interval = 1000;
            this.cTimerNext.Tick += new System.EventHandler(this.cTimerNext_Tick);
            // 
            // cTimerNextMacro
            // 
            this.cTimerNextMacro.Tick += new System.EventHandler(this.cTimerNextMacro_Tick);
            // 
            // fFIMSyncTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1258, 571);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.dgAux);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.mMenu;
            this.Name = "fFIMSyncTest";
            this.Text = "FIM Syncronization Tests";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fFIMSyncTest_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.fFIMSyncTest_KeyUp);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgObjs)).EndInit();
            this.mMenu.ResumeLayout(false);
            this.mMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cDSUsrs)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgAux)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        internal System.Windows.Forms.DataGridView dgObjs;
        private System.Windows.Forms.MenuStrip mMenu;
        private System.Data.DataSet cDSUsrs;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgValues;
        private System.Windows.Forms.DataGridView dgAux;
        private System.Windows.Forms.ToolStripMenuItem mLoadData;
        internal System.Windows.Forms.ToolStripTextBox cMax;
        private System.Windows.Forms.ToolStripStatusLabel cMsg;
        private System.Windows.Forms.ToolStripProgressBar cProgBar;
        private System.Windows.Forms.ToolStripMenuItem mNewObject;
        internal System.Windows.Forms.ToolStripComboBox cBU;
        internal System.Windows.Forms.ToolStripComboBox cObjClass;
        private System.Windows.Forms.ToolStripMenuItem mDelete;
        private System.Windows.Forms.ToolStripMenuItem mCustomActions;
        internal System.Windows.Forms.ToolStripMenuItem mRunMAProfiles;
        internal System.Windows.Forms.ToolStripComboBox mProfilesOptions;
        internal System.Windows.Forms.ToolStripTextBox cMaxRunProfiles;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem mDetails;
        private System.Windows.Forms.ToolStripMenuItem mDeleteMV;
        internal System.Windows.Forms.ToolStripTextBox cFilter;
        private System.Windows.Forms.ToolStripComboBox cLstMacros;
        internal System.Windows.Forms.Timer cTimerStart;
        private System.Windows.Forms.Timer cTimerNext;
        private System.Windows.Forms.ToolStripMenuItem mEditMacro;
        private System.Windows.Forms.ToolStripMenuItem mShowLogWindow;
        internal System.Windows.Forms.Timer cTimerNextMacro;
        internal System.Windows.Forms.ToolStripStatusLabel lbPfStatus1;
        internal System.Windows.Forms.ToolStripStatusLabel lbPfStatus2;
        internal System.Windows.Forms.ToolStripStatusLabel lbPfStatus3;
        internal System.Windows.Forms.ToolStripStatusLabel lbPfStatus4;
        internal System.Windows.Forms.ToolStripStatusLabel lbPfStatus5;
        internal System.Windows.Forms.ToolStripStatusLabel lbPfStatus6;
        internal System.Windows.Forms.ToolStripMenuItem mExecMacro;
        }
    }

