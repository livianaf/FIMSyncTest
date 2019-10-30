using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.IO;

namespace FIMSyncTest {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    public class Config {
        public static DataTable tFDatos { get; private set; }
        public static DataTable tAttributes { get; private set; }
        public static DataTable tCfg { get; private set; }
        public static DataTable tProfiles { get; private set; }
        public static Dictionary<string, DataRow> dFDatos { get; private set; }
        public static Dictionary<string, DataRow> dAtribs { get; private set; }
        public static Dictionary<string, DataRow> dProfiles { get; private set; }
        public static Dictionary<string, string> dConfig { get; private set; }
        public static Dictionary<string, string> dDefValues { get; private set; }
        public static List<string> lValuesCols { get; private set; }
        public static List<string> lValuesRows { get; private set; }
        public static List<string> lValuesTypes { get; private set; }
        public static List<string> lValuesLinks { get; private set; }// attributes with Link in Usage column
        public static List<string> lObjCols { get; private set; }
        public static List<string> lAuxCols { get; private set; }

        public static Color BackColor_Mark { get; private set; }
        public static Color ForeColor_Mark { get; private set; }
        public static Color ForeColor_Change { get; private set; }
        public static Color ForeColor_Sort { get; private set; }
        public static Color BackColor_Normal { get; private set; }
        public static Color ForeColor_Normal { get; private set; }
        public static float FontSize { get; private set; }
        public static int RowHeight { get; private set; }
        public static string ObjectAliasAttribute { get; private set; }
        public static Bitmap DefImage { get; private set; }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void ResetLogs() {
            foreach (FileInfo fi in new DirectoryInfo(Config.PathLog).GetFiles("*.log")) fi.Delete();
            foreach (FileInfo fi in new DirectoryInfo(Config.PathImages).GetFiles("*.jpg")) fi.Delete();
            foreach (FileInfo fi in new DirectoryInfo(Config.PathRunDetails).GetFiles("*.xml")) fi.Delete();
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static void InitConfig(string sXlsFlile, string DatasourcesSheet ) {
            LogHelper.Msg("Loading " + sXlsFlile);

            ExcelHelper Xls = new ExcelHelper(sXlsFlile);
            tFDatos = Xls.Sheet(DatasourcesSheet).Table();
            tAttributes = Xls.Sheet("Attributes").Table();
            tCfg = Xls.Sheet("Config").Table();
            tProfiles = Xls.Sheet("Profiles").Table();
            Xls.CloseExcel();
            Xls = null;

            dFDatos = new Dictionary<string, DataRow>();
            dAtribs = new Dictionary<string, DataRow>();
            dProfiles = new Dictionary<string, DataRow>();
            dConfig = new Dictionary<string, string>();
            dDefValues = new Dictionary<string, string>();

            lValuesCols = new List<string>();
            lValuesRows = new List<string>();
            lValuesTypes = new List<string>();
            lValuesLinks = new List<string>();
            lObjCols = new List<string>();
            lAuxCols = new List<string>();

            for (int i = 0; i < tFDatos.Rows.Count; i++) {
                dFDatos.Add(tFDatos.Rows[i]["Alias"].ToString(), tFDatos.Rows[i]);
                }
            for (int i = 0; i < tAttributes.Rows.Count; i++) {
                dAtribs.Add(tAttributes.Rows[i]["Alias"].ToString(), tAttributes.Rows[i]);
                }
            for (int i = 0; i < tProfiles.Rows.Count; i++) {
                dProfiles.Add(tProfiles.Rows[i]["Alias"].ToString(), tProfiles.Rows[i]);
                }
            for (int i = 0; i < tCfg.Rows.Count; i++) {
                dConfig.Add(tCfg.Rows[i]["Attribute"].ToString(), tCfg.Rows[i]["Value"].ToString());
                }
            foreach (string s in dAtribs.Keys) {
                if (dAtribs[s]["Default Value"].ToString() != "") dDefValues.Add(dAtribs[s]["Alias"].ToString(), dAtribs[s]["Default Value"].ToString()); 
                }

            lValuesCols.Add("Attribute"); lValuesCols.Add("Group");
            lObjCols.Add("Object"); lObjCols.Add("Alias");
            lAuxCols.Add("Attribute"); lAuxCols.Add("MV");
            for (int i = 0; i < tFDatos.Rows.Count; i++) {
                lValuesCols.Add(tFDatos.Rows[i]["Alias"].ToString());
                lObjCols.Add(tFDatos.Rows[i]["Alias"].ToString());
                lAuxCols.Add(tFDatos.Rows[i]["Alias"].ToString());
                }
            for (int i = 0; i < tAttributes.Rows.Count; i++) {
                lValuesRows.Add(tAttributes.Rows[i]["Alias"].ToString());
                lValuesTypes.Add(tAttributes.Rows[i]["Group"].ToString());
                if (tAttributes.Rows[i]["Usage"].ToString().Contains("Link")) lValuesLinks.Add(tAttributes.Rows[i]["Alias"].ToString());
                }
            // alias to be used in object lists
            ObjectAliasAttribute = (dConfig.ContainsKey("ObjectAliasAttribute")) ? dConfig["ObjectAliasAttribute"] : "";
            // Grid colors
            BackColor_Mark = (dConfig.ContainsKey("BackColor:Mark")) ? Color.FromName(dConfig["BackColor:Mark"]) : Color.DarkSeaGreen;
            ForeColor_Mark = (dConfig.ContainsKey("ForeColor:Mark")) ? Color.FromName(dConfig["ForeColor:Mark"]) : Color.Blue;
            ForeColor_Change = (dConfig.ContainsKey("ForeColor:Change"))? Color.FromName(dConfig["ForeColor:Change"]):Color.Red;
            ForeColor_Sort = (dConfig.ContainsKey("ForeColor:Sort")) ? Color.FromName(dConfig["ForeColor:Sort"]) : Color.Blue;
            BackColor_Normal = (dConfig.ContainsKey("BackColor:Normal")) ? Color.FromName(dConfig["BackColor:Normal"]) : Color.White;
            ForeColor_Normal = (dConfig.ContainsKey("ForeColor:Normal")) ? Color.FromName(dConfig["ForeColor:Normal"]) : Color.Black;
            // Default image in Deails
            DefImage = new Bitmap(1, 1);
            DefImage.SetPixel(0, 0, Color.White);
            // Font size and row height
            float fontSize = 8.25F;
            int rowHeight = 22;
            FontSize = (dConfig.ContainsKey("FontSize") && !float.TryParse(dConfig["FontSize"].Replace('.', ','), out fontSize)) ? 8.25F : fontSize;
            RowHeight = (dConfig.ContainsKey("RowHeight") && !int.TryParse(dConfig["RowHeight"], out rowHeight)) ? 22 : rowHeight;

            LogHelper.Msg("");
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string Separator { get { return "|"; } }
        public static char SeparatorC { get { return '|'; } }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string LogFile { get { return Config.PathLog + "FIMSyncTest.log"; } }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string DataFile { get { return Config.PathLog + "FIMSyncTest.dat"; } }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string LogMacroFile { get { return Config.PathLog + "FIMSyncTestMacro.log"; } }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string PathRunDetails { get { return GetPathFolder("RunDetails"); } }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string PathImages { get { return GetPathFolder("AttribImages"); } }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string PathLog { get { return GetPathFolder("Log"); } }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string PathExtension { get { return GetPathFolder("Extension"); } }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string PathMacros { get { return GetPathFolder("Macros"); } }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private static string GetPathFolder(string sName) {
            string sPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, sName);
            if (!System.IO.Directory.Exists(sPath)) System.IO.Directory.CreateDirectory(sPath);
            return sPath + "\\";
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string PathOneImage(string source, string attrib, int index) {
            return Config.PathImages + source + "_" + attrib + "_" + index + ".jpg";
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static bool ExistImage(string source, string attrib, int index) {
            return System.IO.File.Exists(Config.PathOneImage(source, attrib, index));
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static Bitmap GetImage(string source, string attrib, int index) {
            using (Bitmap img = new Bitmap(Config.PathOneImage(source, attrib, index))) {
                var targetImage = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                using (var canvas = Graphics.FromImage(targetImage)) {
                    canvas.DrawImageUnscaled(img, 0, 0);
                    }
                return targetImage;
                }
            }
        }
    }
