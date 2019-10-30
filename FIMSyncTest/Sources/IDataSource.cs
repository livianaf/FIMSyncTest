using System.Collections.Generic;

namespace FIMSyncTest.Sources {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    public interface IDataSource {
        string Name { get; }
        string Type { get; }
        string Rol { get; }
        string CreateObject(Dictionary<string, string> data);
        Dictionary<string, Dictionary<string, string>> FindAllEntries(int limit = 0, string sOtherAttribs = "", string sAdditionalFilter = "");
        Dictionary<string, Dictionary<string, string>> FindAllAttributes(string cn, bool bOnlyOne = true);
        bool DeleteEntry(string cn);
        bool DeleteAllEntries(string lTables=null);
        string UpdateAttributeValue(string Id, string Column, string AtrAlias, string NewValue, string OldValue);
        }
    }
