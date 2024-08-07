using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProdigyAPI.Marketplace.Models.BJEComm;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace
{
    public sealed class BhimaECommerceAuth : Authorization
    {
        public string hostURL = string.Empty;
        public string marketplace = "Bhima";
        public override string GetAccessToken()
        {
            string authToken = string.Empty;
            DateTime authTokenExpireDate = DateTime.MinValue;
            BhimaEcomIntegrationEnvironment env = base.GetEnvironment(marketplace);
            if (env == null)
                throw new Exception("Environment detail is not found.");

            authToken = env.AuthToken;
            authTokenExpireDate = Convert.ToDateTime(env.AuthTokenExpiry);

            if (!string.IsNullOrEmpty(authToken) && authTokenExpireDate >= DateTime.Now) {
                return authToken;
            }
            else {
                authToken = RefreshAccessToken();
                return authToken;
            }
        }

        public override string RefreshAccessToken()
        {
            string authToken = string.Empty;
            try {
                BhimaEcomIntegrationEnvironment env = base.GetEnvironment(marketplace);
                if (env == null)
                    throw new Exception("Environment detail is not found.");
                string refreshToken = env.RefreshToken;
                if (string.IsNullOrEmpty(refreshToken))
                    throw new Exception("Refresh token is not found.");

                DateTime refreshtokenExpiryDate = Convert.ToDateTime(env.RefreshTokenExpiry);
                if (DateTime.Now > refreshtokenExpiryDate) {
                    throw new Exception("Refresh token is expired. A new refresh token is required to get the access token. Update the new refresh token and the refresh token expiry date to continue.");
                }

                hostURL = GetEcomAPIURL("RenewToken").Url;
                using (HttpClient client = new HttpClient()) {
                    AuthtokenRequestVM tokenRequest = new AuthtokenRequestVM { token = refreshToken };
                    string body = JsonConvert.SerializeObject(tokenRequest);
                    HttpContent content = new StringContent(body, Encoding.UTF8, "Application/JSON");
                    HttpResponseMessage response = client.PostAsync(hostURL, content).Result;
                    var result = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode) {
                        var tokenSuccessResponse = JsonConvert.DeserializeObject<AuthtokenResponseVM>(result);
                        SaveTokentoDb(tokenSuccessResponse);
                    }
                    else {
                        var tokenFailureResponse = JsonConvert.DeserializeObject<TokenErrorResponseVM>(result);
                        throw new Exception("Failed to get token. Error: " + tokenFailureResponse.message);
                    }
                }

            }
            catch (Exception ex) {
                throw (ex);
            }
            return authToken;
        }

        public EcomAPIURL GetEcomAPIURL(string transType)
        {
            EcomAPIURL ecomApiURL = dbContext.EcomAPIURLs.Where(api => api.TransType == transType).FirstOrDefault();
            if (ecomApiURL == null)
                throw new Exception(string.Format($"E-comm API url for the transaction type {transType} is not found."));
            return ecomApiURL;
        }        

        private void SaveTokentoDb(AuthtokenResponseVM tokenResponse)
        {
            BhimaEcomIntegrationEnvironment env = dbContext.BhimaEcomIntegrationEnvironments.Where(c => c.Code == marketplace).FirstOrDefault();
            if (env == null)
                throw new Exception("Environment details for environment " + marketplace + " is not found.");

            env.AuthToken = tokenResponse.access_token;
            env.AuthTokenExpiry = Convert.ToDateTime(tokenResponse.expiresIn);
            dbContext.Entry(env).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }
    }
}
