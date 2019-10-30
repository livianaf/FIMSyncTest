using System;
using System.Text;
using System.Security.Cryptography;

namespace FIMSyncTest.Helpers {
    class CryptHelper {
        public static string Mark = "CRYPTOSTR=";
        public static string Encrypt(string plainText) {
            var data = Encoding.Unicode.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(encrypted);
            }

        public static string Decrypt(string cipher) {
            byte[] data = Convert.FromBase64String(cipher);
            byte[] decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.LocalMachine);
            return Encoding.Unicode.GetString(decrypted);
            }
        }
    }
