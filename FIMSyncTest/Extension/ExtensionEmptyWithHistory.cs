//_________________________________________________________________________________________________
//css_host /version:v4.0 /platform:anycpu;
//css_reference FIMSyncTest.exe;
//css_reference System.Core.dll;
//css_reference System.Data.dll;
//css_ref System.Core;
//css_ref System.Data;
//css_ref System.Data.DataSetExtensions;
//_________________________________________________________________________________________________
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Text;
using FIMSyncTest;
//_________________________________________________________________________________________________
//_________________________________________________________________________________________________
public class FIMSyncExtension : MarshalByRefObject {
    //_________________________________________________________________________________________________________
    public bool NewObject(string sObjClass, string BUName, Dictionary<string, string> dConfig, Dictionary<string, string> dDefValues, Dictionary<string, FIMSyncTest.Sources.IDataSource> DS) {
        return false;
        }
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    public bool GetDataForADObjectCreation(string ADRol, string ADName, Dictionary<string, string> dConfig, Dictionary<string, string> data, ref string sPath, ref string objclass, ref string objclassID) {
        return false;
        }
    //_________________________________________________________________________________________________________
    public Dictionary<string, string> SetAdditionalAttributesToNewADObject(string ADRol, string ADName, Dictionary<string, string> dConfig, Dictionary<string, string> data, string sPath, string objclass, string objclassID) {
        Dictionary<string, string> additionalValues = new Dictionary<string, string>(); 
        return additionalValues;
        }
    //_________________________________________________________________________________________________________
    public bool EnableNewObject(ref string sMenuText, ref string sMenuToolTip) {
        sMenuText = "&New";
        sMenuToolTip = "Create a new object";
        return false;// true;
        }
    //_________________________________________________________________________________________________________
    public bool EnableCustomActions(ref string sMenuText, ref string sMenuToolTip) {
        sMenuText = "Run AD &Actions";
        sMenuToolTip = "It update an attribute in AD to force replication";
        return false;
        }
    //_________________________________________________________________________________________________________
    public bool EnableCustomMenus(ref List<string> sMenuText, ref List<string> sMenuToolTip, ref List<string> sMacroName) {
        sMenuText.Add("Export History");
        sMenuToolTip.Add("Search first selected DN in Export History");
        sMacroName.Add("LanzaFIMExport"); 
        return true;
        }
    //_________________________________________________________________________________________________________
    public bool EnableDelete(ref string sMenuText, ref string sMenuToolTip) {
        sMenuText = "&Delete";
        sMenuToolTip = "Delete selected object";
        return false;
        }
    //_________________________________________________________________________________________________________
    public bool EnableDeleteMV(ref string sMenuText, ref string sMenuToolTip) {
        sMenuText = "Delete &MV";
        sMenuToolTip = "Delete all MV content";
        return false;
        }
    //_________________________________________________________________________________________________________
    public bool EnableProfileActions(ref string sMenuText, ref string sMenuToolTip) {
        sMenuText = "&Run MA Profiles";
        sMenuToolTip = "F10 or Click = Run MA Profiles";
        return false;
        }
    //_________________________________________________________________________________________________________
    public bool EnableMacroActions(ref string sMenuText, ref string sMenuToolTip) {
        sMenuText = "R&un Macro";
        sMenuToolTip = "Execute selected macro";
        return false;
        }
    //_________________________________________________________________________________________________________
    public string[] GetNewObjectClasses() {
        return new string[] { "user" };
        }
    //_________________________________________________________________________________________________________
    public void CustomActions(Dictionary<string, FIMSyncTest.Sources.IDataSource> DS) {
		return;
        }
    }
