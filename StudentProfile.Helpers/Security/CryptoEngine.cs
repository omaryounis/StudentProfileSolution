using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;

namespace StudentProfile.Components.Security
{
    public class EncryptDecrypt
    {

        static string password = new AppSettingsReader().GetValue("EncryptionSecurityKey", typeof(String)).ToString();
        static string salt = new AppSettingsReader().GetValue("EncryptionVIKey", typeof(String)).ToString();
        static string webProtectionProvider = new AppSettingsReader().GetValue("ProtectionProvider", typeof(String)).ToString();
        // Get the key from config file


        public static string Encrypt(string toEncrypt, bool useHashing)
        {

            byte[] resultArray;
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {

                var key = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(salt));

                rijAlg.Key = key.GetBytes(rijAlg.KeySize / 8);
                rijAlg.IV = key.GetBytes(rijAlg.BlockSize / 8);
                //rijAlg.Padding = PaddingMode.Zeros;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(toEncrypt);
                        }
                        resultArray = msEncrypt.ToArray();
                    }
                }
            }

            //Return the encrypted data into unreadable string format
            var strTiEncript = Convert.ToBase64String(resultArray, 0, resultArray.Length).Replace('+', '-').Replace('/', '_');
            strTiEncript = strTiEncript.Remove(strTiEncript.Length - 2, 2);
            return strTiEncript;
        }

        public static Dictionary<string, string> Decrypt(string cipherString, bool useHashing)
        {


            byte[] toEncryptArray = DecodeString(cipherString);
            string decryptedString = null;

            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {

                var key = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(salt));

                rijAlg.Key = key.GetBytes(rijAlg.KeySize / 8);
                rijAlg.IV = key.GetBytes(rijAlg.BlockSize / 8);
                //rijAlg.Padding = PaddingMode.Zeros;


                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);


                using (MemoryStream msDecrypt = new MemoryStream(toEncryptArray))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            decryptedString = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            string[] keyValuePair = decryptedString.Replace("&", "-").Split('-');

            foreach (string keyString in keyValuePair)
            {
                string[] keyValue = keyString.Split('=');
                try
                {
                   
                    dictionary.Add(keyValue[0], keyValue[1]);
                }
                catch 
                {

                    dictionary.Add("id", keyValue[0]);
                }
              
                
            }

            return dictionary;
        }
        public static bool IsBase64Encoded(string str)
        {
            try
            {
                var data = DecodeString(str);
                return true;
            }
            catch
            {
                // If exception is caught, then it is not a base64 encoded string
                return false;
            }
        }
        public static void ProtectConfig()
        {
            Configuration config =
              WebConfigurationManager.OpenWebConfiguration("/");

            ConfigurationSection appSettingsSection = config.GetSection("appSettings");

            if (appSettingsSection != null && !appSettingsSection.SectionInformation.IsProtected)
            {
                appSettingsSection.SectionInformation.ProtectSection(webProtectionProvider);
               

            }
            ConfigurationSection connectionStringsSection = config.GetSection("connectionStrings");
            if (connectionStringsSection != null && !connectionStringsSection.SectionInformation.IsProtected)
            {
                connectionStringsSection.SectionInformation.ProtectSection(webProtectionProvider);
              

            }

            config.Save(ConfigurationSaveMode.Modified);
        }
        private static byte[] DecodeString(string str)
        {
            string incoming = str.Replace('_', '/').Replace('-', '+') + "==";
            //switch (str.Length % 4)
            //{
            //    case 2: incoming += "=="; break;
            //    case 3: incoming += "="; break;
            //}
            return Convert.FromBase64String(incoming);
        }
    }
}
