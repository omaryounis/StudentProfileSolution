using StudentProfile.DAL.Models;
using StudentProfile.DAL.Models.VM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace StudentProfile.Components
{
    public class Passwordhelper
    {
        #region password Encryption
        public static string GenerateLoginToken(String password, string userName)
        {




            // create hash password to be equel password stored in db.
            var _password = HashText(password);
            // create login key
            var loginKey = $"{userName.ToLower()}_{_password}";

            var loginHash = HashText(loginKey);

            return loginHash;

        }


        public static string HashText(string text)
        {
            var hashText = $"#4%912#{text}(09)$%*KM";

            var hash = new StringBuilder();
            var md5provider = new MD5CryptoServiceProvider();
            var bytes = md5provider.ComputeHash(new UnicodeEncoding().GetBytes(hashText));

            foreach (var t in bytes) hash.Append(t.ToString("x2")); //lowerCase; X2 if uppercase desired

            return hash.ToString();


        }

        public static string HashTextWithoutPattern(string text)
        {


            var hash = new StringBuilder();
            var md5provider = new MD5CryptoServiceProvider();
            var bytes = md5provider.ComputeHash(new UnicodeEncoding().GetBytes(text));

            foreach (var t in bytes) hash.Append(t.ToString("x2")); //lowerCase; X2 if uppercase desired

            return hash.ToString();


        }
        //public static string HashText(string text)
        //{
        //    var fulltext = $"#!#{text}+-20!20@";

        //    var hash = new StringBuilder();
        //    MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
        //    byte[] bytes = md5provider.ComputeHash(new UnicodeEncoding().GetBytes(fulltext));

        //    for (int i = 0; i < bytes.Length; i++)
        //    {
        //        hash.Append(bytes[i].ToString("x2")); //lowerCase; X2 if uppercase desired
        //    }


        //    return hash.ToString();


        //}



        public static StudentProfile.DAL.Models.DashBoard_Users Login(DashBoard_Users loggedUser)
        {

            SqlParameter[] sqlParams = new SqlParameter[]
             {
                  new SqlParameter("@token",GenerateLoginToken(loggedUser.Password,loggedUser.Username))

             };

            StudentProfile.DAL.Models.DashBoard_Users user = new StudentProfile.DAL.Models.DashBoard_Users();

            decimal outt = 0;




            //DataSet Ds2 = SqlHelper.ExecuteDataset(SqlHelper.Gam3aConnectionString, CommandType.Text, "SELECT * FROM dbo.DashBoard_Users");
            //string updat = "";
            //foreach (DataRow item in Ds2.Tables[0].Rows)
            //{

            //    updat += $"update dbo.DashBoard_Users SET Username = '{StringCipher.Encrypt(item["unencryptedUsername"].ToString(), System.Configuration.ConfigurationManager.AppSettings["encryption.MasterKey"].ToString())}' where ID= {Convert.ToInt32(item["ID"])} ; ";
            //    //  var res=   SqlHelper.ExecuteNonQuery(SqlHelper.Gam3aConnectionString, CommandType.Text, $"");



            //}

            
            DataSet Ds = SqlHelper.ExecuteDataset(SqlHelper.Gam3aConnectionString, "PR_StudentProfileLogin", sqlParams);

            if (Ds != null && Ds.Tables != null && Ds.Tables[0].Rows.Count >= 1)
            {
                foreach (DataRow item in Ds.Tables[0].Rows)
                {


                    if (item["ID"] != DBNull.Value)
                    {
                        user.ID = Convert.ToInt32(item["ID"]);
                    }
                    if (item["Username"] != DBNull.Value)
                    {
                        user.Username = item["Username"].ToString();
                    }

                    if (item["Name"] != DBNull.Value)
                    {
                        user.Name = item["Name"].ToString();
                    }

                    if (item["Mobile"] != DBNull.Value)
                    {
                        user.Mobile = item["Mobile"].ToString();
                    }
                    if (item["IsAdmin"] != DBNull.Value)
                    {
                        user.IsAdmin = Convert.ToBoolean(item["IsAdmin"]);
                    }
                    if (item["LastChangeDatetime"] != DBNull.Value)
                    {
                        user.LastChangeDatetime = Convert.ToDateTime(item["LastChangeDatetime"]);
                    }
                    if (item["CreatedDate"] != DBNull.Value)
                    {
                        user.CreatedDate = Convert.ToDateTime(item["CreatedDate"]);
                    }
                    if (item["IsStudent"] != DBNull.Value)
                    {
                        user.IsStudent = Convert.ToBoolean(item["IsStudent"]);
                    }
                    if (item["AccID"] != DBNull.Value)
                    {
                        user.AccID = Convert.ToInt32(item["AccID"]);
                    }
                    //using(SchoolAccGam3aEntities db = new SchoolAccGam3aEntities())
                    //{
                    //    if(decimal.TryParse(loggedUser.Username, out outt)) { 
                    //    if(db.INTEGRATION_All_Students.Where(x=> x.STUDENT_ID == outt).Count()>0) { user.IsStudent = true; }
                    //    }
                    //}


                }

            }


















            return user;
        }

        public static bool IsValidPassword(DashBoard_Users user)
        {

            var loggedUser = Login(user);
            if (loggedUser != null && loggedUser.ID > 0)
            {

                return true;
            }

            return false;
        }

        public static int SaveChangePassword(ChangePasswordViewModel model)
        {
            var hased = $"{model.UserName.ToLower()}_{'_' + model.NewPassword}";
            SqlParameter[] sqlParams = new SqlParameter[]
             {
                  new SqlParameter("@UserID",model.ID),
                  new SqlParameter("@NewPasswod",HashText(model.NewPassword))

             };


            int Ds = SqlHelper.ExecuteNonQuery(SqlHelper.Gam3aConnectionString, "PR_ChangePassword", sqlParams);

            return Ds;

        }


        #endregion password Encryption



        public static string EncryptData(string strData)
        {
            //byte[] key = { }; //Encryption Key   
            //byte[] IV = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
            //byte[] inputByteArray;



            try
            {
                string returnstring;
                var salt = GenerateSalt();
                using (var keyBytes = new Rfc2898DeriveBytes(System.Web.Configuration.WebConfigurationManager.AppSettings["encryption.MasterKey"].ToString(), salt))
                {
                    var key = keyBytes.GetBytes(8);

                    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                    des.GenerateIV();
                    byte[] inputByteArray = Encoding.UTF8.GetBytes(strData);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, des.IV), CryptoStreamMode.Write);
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    returnstring = Convert.ToBase64String(ms.ToArray());
                }

                //URL Encryption Avoid Reserved Characters
                returnstring = returnstring.Replace("/", "-2F-");
                returnstring = returnstring.Replace("+", "-2B-");
                returnstring = returnstring.Replace("=", "-3D-");

                return returnstring;
            }
            //try
            //{
            //    key = Encoding.UTF8.GetBytes(System.Configuration.ConfigurationManager.AppSettings["encryption.MasterKey"].ToString());
            //    // DESCryptoServiceProvider is a cryptography class defind in c#.  
            //    DESCryptoServiceProvider ObjDES = new DESCryptoServiceProvider();
            //    inputByteArray = Encoding.UTF8.GetBytes(strData);
            //    MemoryStream Objmst = new MemoryStream();
            //    CryptoStream Objcs = new CryptoStream(Objmst, ObjDES.CreateEncryptor(key, IV), CryptoStreamMode.Write);
            //    Objcs.Write(inputByteArray, 0, inputByteArray.Length);
            //    Objcs.FlushFinalBlock();

            //    return Convert.ToBase64String(Objmst.ToArray());//encrypted string  
            //}
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        private static byte[] GenerateSalt()
        {
            var randomBytes = new byte[8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public static string DecryptData(string strData)
        {
            byte[] key = { };// Key   
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] inputByteArray = new byte[strData.Length];

            try
            {
                key = Encoding.UTF8.GetBytes(System.Web.Configuration.WebConfigurationManager.AppSettings["encryption.MasterKey"].ToString());
                DESCryptoServiceProvider ObjDES = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(strData);
                ObjDES.GenerateIV();
                MemoryStream Objmst = new MemoryStream();
                CryptoStream Objcs = new CryptoStream(Objmst, ObjDES.CreateDecryptor(key, ObjDES.IV), CryptoStreamMode.Write);
                Objcs.Write(inputByteArray, 0, inputByteArray.Length);
                Objcs.FlushFinalBlock();

                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(Objmst.ToArray());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

    }


    public static class StringCipher
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = new UnicodeEncoding().GetBytes((plainText));
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                //  return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                return new UnicodeEncoding().GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
