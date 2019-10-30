using System.Collections.Generic;

namespace FIMSyncTest.Profiles {
    //_________________________________________________________________________________________________
    //_________________________________________________________________________________________________
    public class FIMServers {
        private static Dictionary<string, FIMServer> dFIMServers { get; set; }
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        public static bool Loaded { get{ return (dFIMServers != null); } }
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        public static Dictionary<string, FIMServer>.KeyCollection Keys { get { return Loaded ? dFIMServers.Keys : null; } }
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        public static FIMServer Item(string Key) { return dFIMServers.ContainsKey(Key)? dFIMServers[Key]: null; }
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        public static void LoadServers( Dictionary<string, cWorker> dWorkers ) {
            int i = 1;
            dFIMServers = new Dictionary<string, FIMServer>();
            foreach ( string s in Config.dProfiles.Keys ) {
                if ( !dFIMServers.ContainsKey(Config.dProfiles[s]["AliasSrv"].ToString()) ) {
                    FIMServer oFS = new FIMServer();
                    oFS.label = Config.dProfiles[s]["AliasSrv"].ToString();
                    oFS.srv = Config.dProfiles[s]["Server"].ToString();
                    oFS.usr = Config.dProfiles[s]["Usr"].ToString();
                    oFS.pwd = Config.dProfiles[s]["Pwd"].ToString();
                    oFS.MAs = new List<FIMMA>();
                    dFIMServers.Add(Config.dProfiles[s]["AliasSrv"].ToString(), oFS);
                    dWorkers["" + i++].oFIMServer = oFS;
                    }
                FIMMA oMA = new FIMMA();
                oMA.profileKey = s;
                oMA.label = Config.dProfiles[s]["AliasMA"].ToString();
                oMA.name = Config.dProfiles[s]["MA"].ToString();
                oMA.pflE = Config.dProfiles[s]["E"].ToString();
                oMA.pflDI = Config.dProfiles[s]["DI"].ToString();
                oMA.pflDS = Config.dProfiles[s]["DS"].ToString();
                oMA.pflFI = Config.dProfiles[s]["FI"].ToString();
                oMA.pflFS = Config.dProfiles[s]["FS"].ToString();
                oMA.pflEDIS = Config.dProfiles[s]["EDIS"].ToString();
                oMA.pflEFIS = Config.dProfiles[s]["EFIS"].ToString();

                //dFIMServers[s].dMAs.Add(oMA.label, oMA);
                dFIMServers[Config.dProfiles[s]["AliasSrv"].ToString()].MAs.Add(oMA);
                }
            }
        }
    }
