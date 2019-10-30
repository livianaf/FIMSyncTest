using System;
using System.Xml;
using System.Text.RegularExpressions;

namespace FIMSyncTest.Profiles {
    //_________________________________________________________________________________________________
    //_________________________________________________________________________________________________
    public class ResultProfile {
        public string r { get; set; }
        public string desc { get; set; }
        public int changes { get; set; }
        public string file { get; set; }
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        public string Xml {
            set {
                if (file != null) {
                    System.IO.File.WriteAllText(Config.PathRunDetails + file, value);
                    }
                try { this.parseXml(value); }
                catch (Exception ex) { desc += "\r\n" + ex.Message; }
                }
            }
        //_________________________________________________________________________________________________
        //_________________________________________________________________________________________________
        private void parseXml(string sXml) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sXml);
            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/run-history/run-details/step-details");
            desc = "";
            foreach (XmlNode node in nodes) {
                var stepresult = node.SelectSingleNode("step-result").InnerText;
                var steptype = node.SelectSingleNode("step-description").SelectSingleNode("step-type").Attributes.GetNamedItem("type").Value;
                desc += " \r\n" + steptype + " (" + stepresult + ") ";
                if (steptype == "export") {
                    desc += " add: " + node.SelectSingleNode("export-counters").SelectSingleNode("export-add").InnerText;
                    desc += " update: " + node.SelectSingleNode("export-counters").SelectSingleNode("export-update").InnerText;
                    desc += " rename: " + node.SelectSingleNode("export-counters").SelectSingleNode("export-rename").InnerText;
                    desc += " delete: " + node.SelectSingleNode("export-counters").SelectSingleNode("export-delete").InnerText;
                    desc += " deladd: " + node.SelectSingleNode("export-counters").SelectSingleNode("export-delete-add").InnerText;
                    }
                if (steptype == "delta-import" || steptype == "full-import") {
                    desc += " add: " + node.SelectSingleNode("staging-counters").SelectSingleNode("stage-add").InnerText;
                    desc += " update: " + node.SelectSingleNode("staging-counters").SelectSingleNode("stage-update").InnerText;
                    desc += " rename: " + node.SelectSingleNode("staging-counters").SelectSingleNode("stage-rename").InnerText;
                    desc += " delete: " + node.SelectSingleNode("staging-counters").SelectSingleNode("stage-delete").InnerText;
                    desc += " deladd: " + node.SelectSingleNode("staging-counters").SelectSingleNode("stage-delete-add").InnerText;
                    }
                if (steptype == "apply-rules") {
                    desc += " projected: " + node.SelectSingleNode("inbound-flow-counters").SelectSingleNode("disconnector-projected-flow").InnerText;
                    desc += " joined: " + node.SelectSingleNode("inbound-flow-counters").SelectSingleNode("disconnector-joined-flow").InnerText;
                    desc += " flow upd: " + node.SelectSingleNode("inbound-flow-counters").SelectSingleNode("connector-flow").InnerText;
                    desc += " delete: " + node.SelectSingleNode("inbound-flow-counters").SelectSingleNode("connector-delete-remove-mv").InnerText;
                    }
                }
            changes = Regex.Matches(desc, ": ").Count - Regex.Matches(desc, ": 0").Count;
            }
        }
    }
