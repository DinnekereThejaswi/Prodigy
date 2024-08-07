using Newtonsoft.Json;
using ProdigyAPI.BL.ViewModel.HttpRequestHandler;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.APIHandler
{
    public class TokenManager
    {
        #region Declaration
        HttpClient clients = new HttpClient();
        DateTime refreshTokenExpiryTime = new DateTime();
        DateTime authTokenExpiryTime = new DateTime();
        MagnaDbEntities dbContext = new MagnaDbEntities();
        ProdigyCoreToken authToken = new ProdigyCoreToken();
        
        string token = string.Empty;
        public string hostUrl = string.Empty;
        public string tokenEndPoint = string.Empty;
        string AccessToken = string.Empty;
        string errorMsg = string.Empty;
        string marketplace = "PRODIGY_CORE";
        #endregion

        #region Methods
        public string GetAccessAuthToken(string environmentCode)
        {
            return string.Empty;//GetAccesstoken(environmentCode);
        }

        public ProdigyCoreToken ResetAccessAuthToken(string environmentCode)
        {
            return ResetAccessToken(environmentCode);
        }

        public void UpdateAccessToken(ProdigyCoreToken authToken)
        {
            UpdateTokenApiDetail(authToken);
        }

        public ProdigyCoreToken RefreshAuthToken()
        {
            return _RefreshAuthToken();
        }

        private string GetAccesstoken(string environmentCode)
        {
            //Get AccessToken from the Database
            //For the Time being we have used API Call Only.
            //return EncryptionDecryption.Decryption(authToken.access_token);         
            try {
                IntegrationEnvironment apiDet = GetApiDetail(environmentCode);
                token = apiDet.AuthToken;
                authTokenExpiryTime = Convert.ToDateTime(apiDet.AuthTokenExpiry);
                refreshTokenExpiryTime = Convert.ToDateTime(apiDet.RefreshTokenExpiry);
                hostUrl = apiDet.HostUrl;
                tokenEndPoint = apiDet.TokenEndPoint;
                if (!string.IsNullOrEmpty(token) && refreshTokenExpiryTime >= DateTime.Now) {
                    return token;
                }
                else {
                    ProdigyCoreToken authToken = ResetAccessToken(environmentCode);
                    token = authToken.Credentials.Token;
                    return token;
                }
            }
            catch (Exception excp) {
                //Log Exception to Database
                //LogErrors.LogApplicationError(excp);
                return null;
            }
        }

        private ProdigyCoreToken ResetAccessToken(string environmentCode)
        {
            try {
                IntegrationEnvironment apiDet = GetApiDetail(environmentCode);

                if (apiDet == null) {
                    return null;
                }
                hostUrl = apiDet.HostUrl;
                tokenEndPoint = apiDet.TokenEndPoint;
                string refreshtokenPayload = apiDet.RefreshtokenPayload;
                string refreshToken = apiDet.RefreshToken;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(hostUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/x-www-form-urlencoded"));
                string body = string.Format(refreshtokenPayload, refreshToken);
                HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = null;
                if (string.IsNullOrEmpty(tokenEndPoint))
                    response = client.PostAsync("/auth/token", content).Result;
                else
                    response = client.PostAsync("/" + tokenEndPoint + "/auth/token", content).Result;
                var values = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode) {
                    var Token = response.Content.ReadAsStringAsync().Result;
                    ProdigyCoreToken authToken = JsonConvert.DeserializeObject<ProdigyCoreToken>(Token);
                    UpdateTokenApiDetail(authToken);
                }
                else {
                    ProdigyCoreToken authToken = RefreshAuthToken();
                    UpdateTokenApiDetail(authToken);
                }
                client.Dispose();
                return authToken;
            }
            catch (Exception excp) {
                return null;
            }
        }

        private void UpdateTokenApiDetail(ProdigyCoreToken authToken)
        {
            try {
                if (authToken == null) {
                    return;
                }
                DateTime authTokenExpiry = authToken.Credentials.Expiration;
                int refershTokenExpiryIn = 7;
                if (ConfigurationManager.AppSettings["RefreshTokenExpiryIn"] != null) {
                    refershTokenExpiryIn = Convert.ToInt32(ConfigurationManager.AppSettings["RefreshTokenExpiryIn"].ToString());
                }
                DateTime refreshTokenExpiry = authTokenExpiry.AddDays(refershTokenExpiryIn);
                IntegrationEnvironment intEnv = dbContext.IntegrationEnvironments.Where(c => c.Code == marketplace).FirstOrDefault();
                intEnv.AuthToken = authToken.Credentials.Token;
                intEnv.RefreshToken = SIGlobals.Globals.Base64Encode(authToken.Credentials.RefreshToken);
                intEnv.RefreshTokenExpiry = authTokenExpiry;
                intEnv.AuthTokenExpiry = refreshTokenExpiry;
                intEnv.UpdatedOn = SIGlobals.Globals.GetDateTime();
                dbContext.Entry(intEnv).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            catch (Exception excp) {
                return;
            }
        }

        private ProdigyCoreToken _RefreshAuthToken()
        {
            try {
                IntegrationEnvironment apiDet = GetApiDetail(marketplace);

                if (apiDet == null) {
                    return null;
                }
                hostUrl = apiDet.HostUrl;
                tokenEndPoint = apiDet.TokenEndPoint;
                string authtokenPayload = apiDet.AuthTokenPayload;
                string refreshToken = SIGlobals.Globals.Base64decode(apiDet.AuthTokenPayload);

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(hostUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/JSON"));
                string body = refreshToken;
                HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = null;
                if (string.IsNullOrEmpty(tokenEndPoint))
                    response = client.PostAsync("/auth/token", content).Result;
                else
                    response = client.PostAsync("/" + tokenEndPoint + "/auth/token", content).Result;

                var values = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode) {
                    authToken = JsonConvert.DeserializeObject<ProdigyCoreToken>(values);
                    UpdateTokenApiDetail(authToken);
                }
                else {
                    //ErrorCls errorcls = JsonConvert.DeserializeObject<ErrorCls>(values);
                    //LogErrors.LogInfo(string.Format("status: {0}, {1}", response.StatusCode, errorcls.errorInfo.description));
                }
                return authToken;
            }
            catch (Exception excp) {
                return null;
            }
        }

        private IntegrationEnvironment GetApiDetail(string environmentCode)
        {
            IntegrationEnvironment tokenCred = dbContext.IntegrationEnvironments.Where(c => c.Code == environmentCode).FirstOrDefault();
            return tokenCred;
        }
        #endregion
    }
}
