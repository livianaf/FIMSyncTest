using System;
using System.Management;
using System.Security;
using FIMSyncTest.Profiles;

namespace FIMSyncTest {
    class WmiHelper {
        //_________________________________________________________________________________________________________
        // Exec FIM Run Profiles in parallel  in all servers
        //_________________________________________________________________________________________________________
        public static string ConnectServer(FIMServer oSrv) {
            // Si ya está conectado, sale
            if (oSrv.resultConn == "OK") return oSrv.resultConn; 
            // Conecta
            oSrv.resultConn = "OK"; 
            oSrv.wmico = new ConnectionOptions();
            oSrv.wmico.Authentication = AuthenticationLevel.Call;//.PacketPrivacy;//.Call;//.Packet;
            oSrv.wmico.Impersonation = ImpersonationLevel.Impersonate;
            oSrv.wmico.EnablePrivileges = true;
            oSrv.wmico.Locale = "MS_409";
            oSrv.wmico.Username = oSrv.usr;
            oSrv.wmico.Password = oSrv.pwd;
            if (oSrv.srv.ToLower() == "localhost") { 
                oSrv.ms = new ManagementScope(String.Format(@"\\{0}\root\MicrosoftIdentityIntegrationServer", oSrv.srv));
                }
            else
                oSrv.ms = new ManagementScope(String.Format(@"\\{0}\root\MicrosoftIdentityIntegrationServer", oSrv.srv), oSrv.wmico);
            // Explicit connection to WMI namespace
            if (TryConnect(oSrv.ms) != "OK")
                oSrv.resultConn = TryConnect(oSrv.ms);
            if (oSrv.resultConn != "OK") return oSrv.resultConn;

            oSrv.oo = new ObjectGetOptions(null, TimeSpan.MaxValue, true);
            for (int i = 0; i < oSrv.MAs.Count; i++) {
                oSrv.MAs[i].mp = new ManagementPath(string.Format("MIIS_ManagementAgent.Name=" + "'{0}'", oSrv.MAs[i].name));
                oSrv.MAs[i].mo = new ManagementObject(oSrv.ms, oSrv.MAs[i].mp, oSrv.oo);
                oSrv.MAs[i].mbo = oSrv.MAs[i].mo.GetMethodParameters("Execute");
                }
            return oSrv.resultConn;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static bool ClearRuns(FIMServer oSrv) {
            int MinsBackToClear = 20; 
            try {
                SelectQuery myQuery = new SelectQuery("MIIS_Server");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(oSrv.ms, myQuery);

                DateTime dDate = DateTime.Now.AddMinutes(-MinsBackToClear);
                string sDate = dDate.ToString("yyyy-MM-dd HH:mm:ss");
                foreach (ManagementObject server in searcher.Get()) {
                    server.InvokeMethod("ClearRuns", new object[1] { sDate });
                    }
                }
            catch (Exception ex) {
                LogHelper.Msg("ClearRuns Error: " + ex.Message);
                return false;
                }
            return true;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static ResultProfile RunProfile( string sAlias, string sProfile ) {
            ResultProfile r = new ResultProfile();
            r.r = "Servers not available or " + sAlias + " not found";
            if ( !FIMServers.Loaded || !Config.dProfiles.ContainsKey(sAlias) ) return r;

            foreach ( string s in FIMServers.Keys ) 
                if ( FIMServers.Item(s).label == Config.dProfiles[sAlias]["AliasSrv"].ToString() )
                    for ( int i = 0; i < FIMServers.Item(s).MAs.Count; i++ )
                        if ( FIMServers.Item(s).MAs[i].profileKey == sAlias && Config.dProfiles[sAlias].Table.Columns.Contains(sProfile) ) {
                            ConnectServer(FIMServers.Item(s));
                            return RunProfile(FIMServers.Item(s), i, Config.dProfiles[sAlias][sProfile].ToString());
                            }

            r.r = "Profile " + sProfile + " not found";
            return r;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static ResultProfile RunProfile(FIMServer oSrv, int iMA, string sProfile) {
            ResultProfile r = new ResultProfile();
            if (oSrv.resultConn != "OK") { r.r = "No Connected"; return r; }
            oSrv.MAs[iMA].mbo["RunProfileName"] = sProfile;
            oSrv.MAs[iMA].mbor = oSrv.MAs[iMA].mo.InvokeMethod("Execute", oSrv.MAs[iMA].mbo, null);
            oSrv.lastRun = oSrv.MAs[iMA].label + " - " + sProfile;
            r.r = oSrv.MAs[iMA].mbor.GetPropertyValue("returnValue").ToString();
            r.file = oSrv.label + "_" + oSrv.MAs[iMA].name + "_" + sProfile + ".xml";
            oSrv.MAs[iMA].mbor = oSrv.MAs[iMA].mo.InvokeMethod("RunDetails", oSrv.MAs[iMA].mbo, null);
            r.Xml = oSrv.MAs[iMA].mbor.GetPropertyValue("returnValue").ToString();
            return r;
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private static string TryConnect(ManagementScope ms) {
            try {
                ms.Connect();
                return "OK";
                }
            catch (Exception ex) {
                return ex.Message;
                }
            }
        //_________________________________________________________________________________________________________
        // Password stored in a secure string
        //_________________________________________________________________________________________________________
        public static SecureString GetPassword(string pw) {
            SecureString password = new SecureString();
            foreach (char c in pw) {
                password.AppendChar(c);
                }
            // lock the password down
            password.MakeReadOnly();
            return password;
            }
        }
    }
