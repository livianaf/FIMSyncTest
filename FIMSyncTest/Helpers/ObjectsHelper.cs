using System;
using System.Text;

namespace FIMSyncTest.Helpers {
    //_________________________________________________________________________________________________________
    //_________________________________________________________________________________________________________
    class ObjectsHelper {
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        private static string CreateNewGuid() {
            Guid newGuid = System.Guid.NewGuid();
            return ConvertGuidToString(newGuid.ToByteArray());
            }
        //_________________________________________________________________________________________________________
        //_________________________________________________________________________________________________________
        public static string ConvertGuidToString(byte[] byteArray){
            var sb = new StringBuilder();
            foreach (var byteValue in byteArray) {
                sb.AppendFormat("{0:x2}", byteValue);
                }
            return sb.Length == 0
                ? null
                : sb.ToString();
            }
        }
    }
