using System.Management;

namespace FIMSyncTest.Profiles {
    //_________________________________________________________________________________________________
    //_________________________________________________________________________________________________
    public class FIMMA {
        public FIMMA() { }
        //public FIMMA(string label, string name, string pflE, string pflDI, string pflDS, string pflFI, string pflFS, string pflEDIS, string pflEFIS) {
        //    this.label = label; this.name = name; this.pflE = pflE; this.pflDI = pflDI; this.pflDS = pflDS; this.pflFI = pflFI; this.pflFS = pflFS; this.pflEDIS = pflEDIS; this.pflEFIS = pflEFIS;
        //    }
        public string profileKey { get; set; }
        public string label { get; set; }
        public string name { get; set; }
        public string pflE { get; set; }
        public string pflDI { get; set; }
        public string pflDS { get; set; }
        public string pflFI { get; set; }
        public string pflFS { get; set; }
        public string pflEDIS { get; set; }
        public string pflEFIS { get; set; }
        public ManagementPath mp { get; set; }
        public ManagementObject mo { get; set; }
        public ManagementBaseObject mbo { get; set; }
        public ManagementBaseObject mbor { get; set; }
        }
    }
