using System;
using System.Windows.Forms;

namespace FIMSyncTest {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    class LogHelper{
        public static ToolStripStatusLabel cMsg { get; set; }
        public static ToolStripProgressBar cProgBar { get; set; }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void Msg(string sMsg = ""){
            if (cMsg == null) return;
            if (sMsg != "") { 
                sMsg += "...";
                System.IO.File.AppendAllText(Config.LogFile, DateTime.Now.ToString() + Config.Separator + sMsg + "\r\n");
                }
            cMsg.Text = sMsg;
            Application.DoEvents();
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void Prg(int Max = -1, int Value = -1){
            if (Max == -1) {
                cProgBar.Maximum = 1;
                cProgBar.Value = 0;
                //cProgBar.Visible = false;
                return;
            }
            if (Value == -1){
                if (cProgBar.Value<cProgBar.Maximum) cProgBar.Value++;
                return;
            }
            cProgBar.Maximum = Max;
            cProgBar.Value = Value;
            cProgBar.Visible = true;
            Application.DoEvents();
            }
        }
    }
