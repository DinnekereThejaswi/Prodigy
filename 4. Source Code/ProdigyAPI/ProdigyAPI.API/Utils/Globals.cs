using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Net.Http;
using System.Diagnostics;
using System.Data.Entity.Validation;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using ProdigyAPI.Model.MagnaDb;

namespace ProdigyAPI.Utils
{
    public class GlobalUtilities
    {
        //public static string GetHashedPassword(string userName, string encryptedPassword, out string errorMsg)
        //{
        //    errorMsg = string.Empty;
        //    try {
        //        string passwordSalt = string.Empty;
        //        CryptoLibrary crypt = new CryptoLibrary();
        //        string pwdText = Base64Decode(encryptedPassword);
        //        using (var dbContext = new ProdigyEntities()) {
        //            var portalUser = dbContext.ApplicationUsers.Where(p => p.UserID == userName).FirstOrDefault();
        //            if (portalUser == null)
        //                return string.Empty;
        //            passwordSalt = portalUser.PasswordSalt;
        //        }
        //        string hashedPwd = crypt.EncryptUsingHash(pwdText, passwordSalt);

        //        return hashedPwd;
        //    }
        //    catch (Exception ex) {
        //        errorMsg = ex.Message;
        //        return string.Empty;
        //    }
        //}

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            string decodedString = string.Empty;
            try {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                decodedString = Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception) {
            }
            return decodedString;
        }

        public static string GetHashedPasswordUsingSalt(string base64EncodedPassword, string passwordSalt)
        {
            try {
                CryptoLibrary crypt = new CryptoLibrary();
                string pwdText = Base64Decode(base64EncodedPassword);
                string hashedPwd = crypt.EncryptUsingHash(pwdText, passwordSalt);
                return hashedPwd;
            }
            catch (Exception) {
                return string.Empty;
            }
        }

        public bool WriteAPITraceToDatabase(HttpRequestMessage request, string requestBody, DateTime requestTimestamp, HttpResponseMessage result, string responseBody, DateTime responseTimestamp)
        {
            var configData = ConfigurationManager.AppSettings["EnableAPITracingToDb"];
            if (configData != null && configData.ToString().ToUpper() == "TRUE") {
                ;
            }
            else
                return true;

            try {
                string ipAddress = ((System.Web.HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                MagnaDbEntities db = new MagnaDbEntities();
                WebAPITraceLog webApiTraceLog = new WebAPITraceLog
                {
                    UserName = "Fill_Later",
                    LoggedOn = DateTime.Now,
                    IPAddress = ipAddress,
                    RequestMethod = request.Method != null ? LeftN(request.Method.ToString(), 20) : null,
                    RequestURI = request.RequestUri != null ? LeftN(request.RequestUri.ToString(), 250) : "",
                    ContentType = request.Headers.Accept != null ? LeftN(request.Headers.Accept.ToString(), 100) : "NA",
                    UserAgent = request.Headers.UserAgent != null ? LeftN(request.Headers.UserAgent.ToString(), 200) : "",
                    RequestBody = requestBody,
                    RequestTimestamp = requestTimestamp,
                    HeaderAuthorization = null,//Implemented later
                    ResponsePhrase = LeftN(result.ReasonPhrase, 20),//result.StatusCode.ToString()
                    IsSuccessStatusCode = result.IsSuccessStatusCode,
                    ReponseBody = responseBody,
                    ResponseTimestamp = responseTimestamp
                };
                db.WebAPITraceLogs.Add(webApiTraceLog);
                db.SaveChanges();
            }
            catch (DbEntityValidationException e) {
                foreach (var eve in e.EntityValidationErrors) {
                    System.Diagnostics.Trace.WriteLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors) {
                        System.Diagnostics.Trace.WriteLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }
            }
            catch (Exception ex) {
                System.Diagnostics.Trace.WriteLine("Failed to log API Calls. Error: " + ex.Message);
                return false;
            }
            return true;
        }

        public string LeftN(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            int lengthToTruncate = maxLength;
            if (str.Length < maxLength)
                lengthToTruncate = str.Length;
            return str.Substring(0, lengthToTruncate);
        }

        public static void Trace(string data)
        {
            var configData = ConfigurationManager.AppSettings["EnableAPITracingToOutput"];
            if(configData != null && configData.ToString().ToUpper() == "TRUE")
                System.Diagnostics.Trace.Write(data);
        }

        public static void TraceLine(string data)
        {
            var configData = ConfigurationManager.AppSettings["EnableAPITracingToOutput"];
            if (configData != null && configData.ToString().ToUpper() == "TRUE")
                System.Diagnostics.Trace.WriteLine(data);
        }

        public static string GetHashcode(string value)
        {
            value = value.ToUpper();
            try {
                Encoder enc = System.Text.Encoding.Unicode.GetEncoder();
                
                byte[] unicodeText = new byte[value.Length * 2];
                enc.GetBytes(value.ToCharArray(), 0, value.Length, unicodeText, 0, true);
                
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(unicodeText);
                
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++) {
                    sb.Append(result[i].ToString("X2"));
                }

                // And return it
                return sb.ToString();
            }
            catch (Exception ex) {
                throw (ex);
            }
        }
        


    }
    
        
    
}