using System;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace FIMSyncTest.Profiles {
    //_________________________________________________________________________________________________
    //_________________________________________________________________________________________________
    public class cWorker : IDisposable{
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        private BackgroundWorker myWorker1 = new BackgroundWorker();
        TextBox textBox1 { set; get; }
        TextBox txtResult { set; get; }
        ToolStripStatusLabel lblStatus { set; get; }
        Button btnStart { set; get; }
        public FIMServer oFIMServer { get; set; }
        private bool changes { set; get; }
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        public void Dispose() {
            myWorker1.Dispose();
            textBox1.Dispose();
            txtResult.Dispose();
            lblStatus.Dispose();
            btnStart.Dispose();
            }
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        public cWorker(TextBox textBox1, TextBox txtResult, ToolStripStatusLabel lblStatus, Button btnStart) {
            myWorker1.DoWork += new DoWorkEventHandler(myWorker1_DoWork);
            myWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(myWorker1_RunWorkerCompleted);
            myWorker1.ProgressChanged += new ProgressChangedEventHandler(myWorker1_ProgressChanged);
            myWorker1.WorkerReportsProgress = true;
            myWorker1.WorkerSupportsCancellation = true;
            this.textBox1 = textBox1;
            this.txtResult = txtResult;
            this.lblStatus = lblStatus;
            this.btnStart = btnStart;
            if (textBox1 != null) this.textBox1.ReadOnly = false;
            }

        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        public void Run(FIMServer FIMSrv) {
            if (!myWorker1.IsBusy) {//Check if the worker is already in progress
                if (lblStatus != null) {
                    lblStatus.Tag = "START";
                    lblStatus.Text = "Waiting...";
                    }
                if (btnStart != null) {
                    btnStart.Enabled = false;//Disable the Start button
                    btnStart.Text = "Cancel";
                    btnStart.Enabled = true;
                    }
                if (textBox1 != null)
                    this.textBox1.ForeColor = System.Drawing.Color.Black;
                myWorker1.RunWorkerAsync(FIMSrv);//Call the background worker
                }
            }
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        public void Cancel() {
            myWorker1.CancelAsync();//Issue a cancellation request to stop the background worker
            }
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        protected void myWorker1_DoWork(object sender, DoWorkEventArgs e) {
            changes = false; 
            BackgroundWorker sendingWorker = (BackgroundWorker)sender;//Capture the BackgroundWorker that fired the event
            FIMServer FIMSrv = (FIMServer)e.Argument;//Collect the array of objects the we recived from the main thread

            int maxValue = (int)FIMSrv.num;//Get the numeric value from inside the objects array, don't forget to cast
            StringBuilder sb = new StringBuilder();//Declare a new string builder to store the result.
            // Conect to servers
            string sMsg = "Connecting " + FIMSrv.label + ": " + WmiHelper.ConnectServer(FIMSrv);
            sb.Append(string.Format("{0}{1}", sMsg, Environment.NewLine));
            Log(sb.ToString());

            ResultProfile result = new ResultProfile();
            result.r = "success";
            // Clear logs
            WmiHelper.ClearRuns(FIMSrv);
            // Execute profiles
            for (int i = 1; i <= maxValue && result.r == "success"; i++) {//Start a for loop
                if (!sendingWorker.CancellationPending) {//At each iteration of the loop, check if there is a cancellation request pending 
                    result = PerformHeavyOperation(FIMSrv);
                    sMsg = Environment.NewLine + "Result: " + FIMSrv.label + " [" + FIMSrv.lastRun + "]: " + result.r + result.desc + Environment.NewLine;
                    //if (sMsg.Contains("error")) e.Result = "Error";
                    sb.Append(sMsg);//Append the result to the string builder
                    sendingWorker.ReportProgress(i);//Report our progress to the main thread
                    Log(sb.ToString(), result.changes);
                    System.Threading.Tasks.Task.Delay(5000);
                    }
                else {
                    e.Cancel = true;//If a cancellation request is pending,assgine this flag a value of true
                    break;// If a cancellation request is pending, break to exit the loop
                    }
                changes |= (result.changes > 0);
                }

            e.Result = sb.ToString();// Send our result to the main thread!
            }
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        protected void Log(string sMsg, int changes=0) {
            if (textBox1 == null) return;
            // update textbox in form
            if (this.textBox1.InvokeRequired) {
                this.textBox1.Invoke(new MethodInvoker(delegate() { 
                    this.textBox1.Text = sMsg;
                    this.textBox1.SelectionStart = this.textBox1.Text.Length;
                    this.textBox1.SelectionLength = 0;
                    this.textBox1.ScrollToCaret();
                    if (sMsg.Contains("error") || sMsg.Contains("call-failure")) this.textBox1.ForeColor = System.Drawing.Color.Red;
                    else if (changes > 0) this.textBox1.ForeColor = System.Drawing.Color.Blue;
                    }));
                }
            else {
                this.textBox1.Text = sMsg;
                this.textBox1.SelectionStart = this.textBox1.Text.Length;
                this.textBox1.SelectionLength = 0;
                this.textBox1.ScrollToCaret();
                }
            }
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        protected void myWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (lblStatus != null) lblStatus.Tag = "END";// It is done at begining. In this way, when event arrive tag is already marked
            if (!e.Cancelled && e.Error == null) {//Check if the worker has been cancelled or if an error occured
                string result = (string)e.Result;//Get the result from the background thread
                if (txtResult != null) txtResult.Text = result;//Display the result to the user
                if (lblStatus != null) {
                    lblStatus.Text = oFIMServer.label + ": Done";
                    lblStatus.ForeColor = changes ? System.Drawing.Color.Blue : System.Drawing.Color.Black;
                    lblStatus.ToolTipText = result;
                    if (result.Contains("error")) {
                        lblStatus.Text = oFIMServer.label + ": Error";
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            else if (e.Cancelled) {
                if (lblStatus != null) lblStatus.Text = oFIMServer.label + ": Canceled";
                }
            else {
                if (lblStatus != null) lblStatus.Text = oFIMServer.label + ": Error occured";
                }
            if (btnStart != null) {
                btnStart.Enabled = false;
                btnStart.Text = "Start";
                btnStart.Enabled = true;//Re-enable the start button
                }
            }

        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        protected void myWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            //Show the progress to the user based on the input we got from the background worker
            if (lblStatus != null) lblStatus.Text = string.Format("Running: {0}...", e.ProgressPercentage);
            }

        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        private ResultProfile PerformHeavyOperation(FIMServer FIMSrv) {
            ResultProfile r = new ResultProfile();
            r.r = "success";
            if (FIMSrv.runPf == "DJ") {
                for (int i = 0; i < FIMSrv.MAs.Count && r.r == "success"; i++) {
                    r = WmiHelper.RunProfile(FIMSrv, i, FIMSrv.MAs[i].pflEDIS);
                    }
                }
            if (FIMSrv.runPf == "FJ") {
                for (int i = 0; i < FIMSrv.MAs.Count && r.r == "success"; i++) {
                    r = WmiHelper.RunProfile(FIMSrv, i, FIMSrv.MAs[i].pflEFIS);
                    }
                }
            if (FIMSrv.runPf == "DS") {
                for (int i = 0; i < FIMSrv.MAs.Count && r.r == "success"; i++) {
                    if (r.r == "success") r = WmiHelper.RunProfile(FIMSrv, i, FIMSrv.MAs[i].pflE);
                    if (r.r == "success") r = WmiHelper.RunProfile(FIMSrv, i, FIMSrv.MAs[i].pflDI);
                    if (r.r == "success") r = WmiHelper.RunProfile(FIMSrv, i, FIMSrv.MAs[i].pflDS);
                    }
                }
            if (FIMSrv.runPf == "FS") {
                for (int i = 0; i < FIMSrv.MAs.Count && r.r == "success"; i++) {
                    if (r.r == "success") r = WmiHelper.RunProfile(FIMSrv, i, FIMSrv.MAs[i].pflE);
                    if (r.r == "success") r = WmiHelper.RunProfile(FIMSrv, i, FIMSrv.MAs[i].pflFI);
                    if (r.r == "success") r = WmiHelper.RunProfile(FIMSrv, i, FIMSrv.MAs[i].pflFS);
                    }
                }
            if (FIMSrv.runPf == "DSNOE") {
                for (int i = 0; i < FIMSrv.MAs.Count && r.r == "success"; i++) {
                    if (r.r == "success") r = WmiHelper.RunProfile(FIMSrv, i, FIMSrv.MAs[i].pflDI);
                    if (r.r == "success") r = WmiHelper.RunProfile(FIMSrv, i, FIMSrv.MAs[i].pflDS);
                    }
                }
            if (FIMSrv.runPf == "FSNOE") {
                for (int i = 0; i < FIMSrv.MAs.Count && r.r == "success"; i++) {
                    if (r.r == "success") r = WmiHelper.RunProfile(FIMSrv, i, FIMSrv.MAs[i].pflFI);
                    if (r.r == "success") r = WmiHelper.RunProfile(FIMSrv, i, FIMSrv.MAs[i].pflFS);
                    }
                }
            return r;
            }
        }
    }
