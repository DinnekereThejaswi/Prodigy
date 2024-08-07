using Newtonsoft.Json;
using ProdigyAPI.Marketplace.Models;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace
{
    public sealed class AmazonConnectorAuth
    {
        #region Declaration
        AmazonErrorLogs LogErrors = new AmazonErrorLogs();
        MagnaDbEntities dbContext = new MagnaDbEntities();

        public string marketplace = "AMAZON";
        public string hostUrl = string.Empty;
        public string tokenEndPoint = string.Empty;
        #endregion

        #region Methods               
        
        public string GetAccesstoken()
        {     
            try {
                IntegrationEnvironment apiDet = GetApiDetail();
                string token = apiDet.AuthToken;
                DateTime authTokenExpiryTime = Convert.ToDateTime(apiDet.AuthTokenExpiry);
                DateTime refreshTokenExpiryTime = Convert.ToDateTime(apiDet.RefreshTokenExpiry);
                hostUrl = apiDet.HostUrl;
                tokenEndPoint = apiDet.TokenEndPoint;
                if (!string.IsNullOrEmpty(token) && authTokenExpiryTime >= DateTime.Now) {
                    return token;
                }
                else {
                    AuthToken authToken = ResetAccessToken();
                    token = authToken.access_token;
                    return token;
                }
            }
            catch (Exception ex) {
                LogErrors.LogApplicationError(ex);
                throw (ex);
            }
        }

        private AuthToken ResetAccessToken()
        {
            try {
                IntegrationEnvironment apiDet = GetApiDetail();
                if (apiDet == null) {
                    throw new Exception("Unable to find API details.");
                }
                hostUrl = apiDet.HostUrl;
                tokenEndPoint = apiDet.TokenEndPoint;
                string refreshtokenPayload = apiDet.RefreshtokenPayload;
                string refreshToken = SIGlobals.Globals.Base64decode(apiDet.RefreshToken);

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
                AuthToken authToken = null;
                if (response.IsSuccessStatusCode) {
                    var Token = response.Content.ReadAsStringAsync().Result;
                    authToken = JsonConvert.DeserializeObject<AuthToken>(Token);
                    SaveTokenInfoToDb(authToken);
                }
                else {
                    authToken = GetNewAuthToken();
                }
                client.Dispose();
                return authToken;
            }
            catch (Exception ex) {
                LogErrors.LogApplicationError(ex);
                throw (ex);
            }
        }

        private void SaveTokenInfoToDb(AuthToken authToken)
        {
            try {
                if (authToken == null) {
                    return;
                }
                DateTime authTokenExpiry = DateTime.Now.AddSeconds(Convert.ToDouble(authToken.expires_in));
                int refershTokenExpiryIn = 7;
                if (ConfigurationManager.AppSettings["RefreshTokenExpiryIn"] != null) {
                    refershTokenExpiryIn = Convert.ToInt32(ConfigurationManager.AppSettings["RefreshTokenExpiryIn"].ToString());
                }
                DateTime refreshTokenExpiry = authTokenExpiry.AddDays(refershTokenExpiryIn);
                IntegrationEnvironment intEnv = dbContext.IntegrationEnvironments.Where(c => c.Code == marketplace).FirstOrDefault();
                intEnv.AuthToken = authToken.access_token;
                intEnv.RefreshToken = SIGlobals.Globals.Base64Encode(authToken.refresh_token);
                intEnv.RefreshTokenExpiry = refreshTokenExpiry;
                intEnv.AuthTokenExpiry = authTokenExpiry;
                intEnv.UpdatedOn = SIGlobals.Globals.GetDateTime();
                dbContext.Entry(intEnv).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            catch (Exception excp) {
                LogErrors.LogApplicationError(excp);
                return;
            }
        }

        private AuthToken GetNewAuthToken()
        {
            try {
                IntegrationEnvironment apiDet = GetApiDetail();
                if (apiDet == null) {
                    throw new Exception("API detail is not found for marketplace " + marketplace);
                }
                hostUrl = apiDet.HostUrl;
                tokenEndPoint = apiDet.TokenEndPoint;
                string authtokenPayload = apiDet.AuthTokenPayload;
                string refreshToken = SIGlobals.Globals.Base64decode(apiDet.AuthTokenPayload);

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(hostUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("Application/x-www-form-urlencoded"));
                string body = refreshToken;
                HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = null;
                if (string.IsNullOrEmpty(tokenEndPoint))
                    response = client.PostAsync("/auth/token", content).Result;
                else
                    response = client.PostAsync("/" + tokenEndPoint + "/auth/token", content).Result;

                var values = response.Content.ReadAsStringAsync().Result;
                AuthToken authToken = null;
                if (response.IsSuccessStatusCode) {
                    authToken = JsonConvert.DeserializeObject<AuthToken>(values);
                    SaveTokenInfoToDb(authToken);
                }
                else {
                    ErrorCls errorcls = JsonConvert.DeserializeObject<ErrorCls>(values);
                }
                return authToken;
            }
            catch (Exception ex) {
                LogErrors.LogApplicationError(ex);
                throw (ex);
            }
        }

        private IntegrationEnvironment GetApiDetail()
        {
            IntegrationEnvironment tokenCred = dbContext.IntegrationEnvironments.Where(c => c.Code == marketplace).FirstOrDefault();
            return tokenCred;
        }
        #endregion
    }
}
