using System.Collections.Generic;
using System.Management;

namespace FIMSyncTest.Profiles {
    //_________________________________________________________________________________________________
    //_________________________________________________________________________________________________
    public class FIMServer {
        public FIMServer() { }
        //public FIMServer(string label, string srv, string usr, string pwd, List<FIMMA> MAs) {
        //    this.label = label; this.srv = srv; this.usr = usr; this.pwd = pwd; this.MAs = MAs; this.num = 3;
        //    }
        public string label { get; set; }
        public int num { get; set; }
        public string runPf { get; set; }
        public string srv { get; set; }
        public string usr { get; set; }
        public string pwd { get; set; }
        public string resultConn { get; set; }
        public string lastRun { get; set; }
        public ConnectionOptions wmico { get; set; }
        public ManagementScope ms { get; set; }
        public ObjectGetOptions oo { get; set; }
        public List<FIMMA> MAs { get; set; }
        }
    }
