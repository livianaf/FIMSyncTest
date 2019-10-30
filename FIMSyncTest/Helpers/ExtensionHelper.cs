using System;
using System.Collections.Generic;
using System.IO;
using CSScriptLibrary;
using System.Windows.Forms;
using FIMSyncTest.Sources;

namespace FIMSyncTest.Helpers {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    public class ExtensionHelper {
        private static IFIMSyncExtension oExtension { get; set; }
        private static MacroHelper mh { set; get; }
        public static bool Present { get; set; }
        //_________________________________________________________________________________________________________
        // Interface of Extension
        //_________________________________________________________________________________________________________
        public interface IFIMSyncExtension {
            bool CheckLoaded( MacroHelper mh );
            bool NewObject( MacroHelper mh, string sObjClass, string BUName, Dictionary<string, string> dConfig, Dictionary<string, string> dDefValues, Dictionary<string, IDataSource> DS);
            bool GetDataForADObjectCreation( MacroHelper mh, string ADRol, string ADName, Dictionary<string, string> dConfig, Dictionary<string, string> data, ref string sPath, ref string objclass, ref string objclassID);
            Dictionary<string, string> SetAdditionalAttributesToNewADObject( MacroHelper mh, string ADRol, string ADName, Dictionary<string, string> dConfig, Dictionary<string, string> data, string sPath, string objclass, string objclassID);
            bool EnableNewObject( MacroHelper mh, ref string sMenuText, ref string sMenuToolTip);
            bool EnableCustomActions( MacroHelper mh, ref string sMenuText, ref string sMenuToolTip);
            bool EnableDelete( MacroHelper mh, ref string sMenuText, ref string sMenuToolTip);
            bool EnableDeleteMV( MacroHelper mh, ref string sMenuText, ref string sMenuToolTip);
            bool EnableProfileActions( MacroHelper mh, ref string sMenuText, ref string sMenuToolTip);
            bool EnableMacroActions( MacroHelper mh, ref string sMenuText, ref string sMenuToolTip);
            void CustomActions( MacroHelper mh, Dictionary<string, IDataSource> DS);
            string[] GetNewObjectClasses( MacroHelper mh);
            bool EnableCustomMenus( MacroHelper mh, ref List<string> sMenuText, ref List<string> sMenuToolTip, ref List<string> sMacroName);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        internal static string GetFirstExtension() {
            string[] filePaths = Directory.GetFiles(Config.PathExtension, "*.cs");
            foreach (string s in filePaths) return s;
            return "";
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void Init( fFIMSyncTest pf, DataGridView pdg ) { mh = new MacroHelper(pf, pdg); Init(); }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void Init() {
            Present = false;
            string sExtensionFile = GetFirstExtension();
            if (sExtensionFile == "") return;
            try {
                if (!File.Exists(sExtensionFile)) return;
                var helper = new AsmHelper(CSScript.CompileFile(sExtensionFile), null, false);
                oExtension = helper.CreateAndAlignToInterface<IFIMSyncExtension>("FIMSyncExtension");
                Present = true;
                }
            catch (Exception ex) {
                string sMsg = string.Format("[{0}]\n[{1}]", sExtensionFile, ex.Message);
                LogHelper.Msg("ERROR:\n" + sMsg);
                MessageBox.Show("ERROR:\n" + sMsg, "FIMSync Extension", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static bool NewObject(string sObjClass, string BUName, Dictionary<string, string> dConfig, Dictionary<string, string> dDefValues, Dictionary<string, IDataSource> DS) {
            if (!Present || !TestExtension()) return false;
            return oExtension.NewObject(mh, sObjClass, BUName, dConfig, dDefValues, DS); 
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static bool GetDataForADObjectCreation(string ADRol, string ADName, Dictionary<string, string> dConfig, Dictionary<string, string> data, ref string sPath, ref string objclass, ref string objclassID) {
            if (!Present || !TestExtension()) return false;
            return oExtension.GetDataForADObjectCreation(mh, ADRol, ADName, dConfig, data, ref sPath, ref objclass, ref objclassID); 
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static Dictionary<string, string> SetAdditionalAttributesToNewADObject(string ADRol, string ADName, Dictionary<string, string> dConfig, Dictionary<string, string> data, string sPath, string objclass, string objclassID) {
            if (!Present || !TestExtension()) return null;
            return oExtension.SetAdditionalAttributesToNewADObject(mh, ADRol, ADName, dConfig, data, sPath, objclass, objclassID);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string[] GetNewObjectClasses() {
            if (!Present || !TestExtension()) return new string[] { };

            return oExtension.GetNewObjectClasses(mh);
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static bool TestExtension() {
            if (!Present) return Present;
            try {
                bool r = oExtension.CheckLoaded(mh);
                return Present;
                }
            catch (System.Runtime.Remoting.RemotingException ex) {
                string sMsg = string.Format("TestExtension RemotingException [{0}]\n[{1}]", GetFirstExtension(), ex.Message);
                LogHelper.Msg("ERROR:\n" + sMsg);
                //MessageBox.Show("ERROR:\n" + sMsg, "FIMSync Extension", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            catch (Exception ex) {
                string sMsg = string.Format("TestExtension Exception [{0}]\n[{1}]", GetFirstExtension(), ex.Message);
                LogHelper.Msg("ERROR:\n" + sMsg);
                //MessageBox.Show("ERROR:\n" + sMsg, "FIMSync Extension", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            Init();
            return Present;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void EnableNewObject(ToolStripMenuItem mNewObject) {
            if (!Present || !TestExtension()) return;

            string sMenuText = "", sMenuToolTip = "";
            bool bVisible = oExtension.EnableNewObject(mh, ref sMenuText, ref sMenuToolTip);
            mNewObject.Text = sMenuText;
            mNewObject.ToolTipText = sMenuToolTip;
            mNewObject.Visible = bVisible;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void EnableCustomActions(ToolStripMenuItem mCustomActions) {
            mCustomActions.Visible = Present;
            if (!Present || !TestExtension()) return;

            string sMenuText = "", sMenuToolTip = "";
            bool bVisible = oExtension.EnableCustomActions(mh, ref sMenuText, ref sMenuToolTip);
            mCustomActions.Text = sMenuText;
            mCustomActions.ToolTipText = sMenuToolTip;
            mCustomActions.Visible = bVisible;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void EnableCustomMenus(MenuStrip mMenu, EventHandler CustomMenuHandler) {
            if (!Present || !TestExtension()) return;

            List<string> sMenuText = new List<string>(), sMenuToolTip = new List<string>(), sMacroName = new List<string>();
            bool bCrear = oExtension.EnableCustomMenus(mh, ref sMenuText, ref sMenuToolTip, ref sMacroName);
            // si no devuelve algo consistente, retorna
            if (bCrear == false || sMenuText.Count == 0 || sMenuText.Count != sMenuToolTip.Count || sMenuText.Count != sMacroName.Count) return;
            ToolStripMenuItem[] items = new ToolStripMenuItem[2]; // You would obviously calculate this value at runtime
            for (int i = 0; i < sMenuText.Count; i++) {
                ToolStripMenuItem MnuItem = new ToolStripMenuItem();
                MnuItem.Text = sMenuText[i];
                MnuItem.ToolTipText = sMenuToolTip[i];
                MnuItem.Tag = sMacroName[i];
                MnuItem.Click += new EventHandler(CustomMenuHandler);
                MnuItem.Visible = true;
                mMenu.Items.Add(MnuItem);
                }
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void EnableDelete(ToolStripMenuItem mNewObject) {
            if (!Present || !TestExtension()) {
                mNewObject.Visible = false;
                return;
                }

            string sMenuText = "", sMenuToolTip = "";
            bool bVisible = oExtension.EnableDelete(mh, ref sMenuText, ref sMenuToolTip);
            mNewObject.Text = sMenuText;
            mNewObject.ToolTipText = sMenuToolTip;
            mNewObject.Visible = bVisible;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void EnableDeleteMV(ToolStripMenuItem mNewObject) {
            if (!Present || !TestExtension()) {
                mNewObject.Visible = false;
                return;
                }

            string sMenuText = "", sMenuToolTip = "";
            bool bVisible = oExtension.EnableDeleteMV(mh, ref sMenuText, ref sMenuToolTip);
            mNewObject.Text = sMenuText;
            mNewObject.ToolTipText = sMenuToolTip;
            mNewObject.Visible = bVisible;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void EnableProfileActions(ToolStripMenuItem mNewObject) {
            if (!Present || !TestExtension()) {
                mNewObject.Visible = false;
                return;
                }

            string sMenuText = "", sMenuToolTip = "";
            bool bVisible = oExtension.EnableProfileActions(mh, ref sMenuText, ref sMenuToolTip);
            mNewObject.Text = sMenuText;
            mNewObject.ToolTipText = sMenuToolTip;
            mNewObject.Visible = bVisible;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void EnableMacroActions(ToolStripMenuItem mNewObject) {
            if (!Present || !TestExtension()) {
                mNewObject.Visible = false;
                return;
                }

            string sMenuText = "", sMenuToolTip = "";
            bool bVisible = oExtension.EnableMacroActions(mh, ref sMenuText, ref sMenuToolTip);
            mNewObject.Text = sMenuText;
            mNewObject.ToolTipText = sMenuToolTip;
            mNewObject.Visible = bVisible;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void CustomActions(Dictionary<string, IDataSource> DS) {
            if (!Present || !TestExtension()) return;
            oExtension.CustomActions(mh, DS); 
            }
        }
    }
