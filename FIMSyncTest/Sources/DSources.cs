using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace FIMSyncTest.Sources {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    class DSources {
        public Dictionary<string, IDataSource> DS = new Dictionary<string, IDataSource>();
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public DSources(DataTable tFDatos, DataTable tAttributes) {
            Cursor.Current = Cursors.WaitCursor;
            for (int i = 0; i < tFDatos.Rows.Count; i++) {
                LogHelper.Msg("Connecting Data Source " + tFDatos.Rows[i]["Alias"].ToString());
                LogHelper.Prg(tFDatos.Rows.Count, i+1);
                switch (tFDatos.Rows[i]["Type"].ToString()) {
                    case "DB":
                        DS.Add(tFDatos.Rows[i]["Alias"].ToString(), new DBSource(tFDatos.Rows[i], tAttributes));
                        break;
                    case "AD":
                        DS.Add(tFDatos.Rows[i]["Alias"].ToString(), new ADSource(tFDatos.Rows[i], tAttributes));
                        break;
                    case "LDS":
                        DS.Add(tFDatos.Rows[i]["Alias"].ToString(), new LDSource(tFDatos.Rows[i], tAttributes));
                        break;
                    }
                }
            LogHelper.Prg(); LogHelper.Msg(); Cursor.Current = Cursors.Default;
            }
        }
    }
