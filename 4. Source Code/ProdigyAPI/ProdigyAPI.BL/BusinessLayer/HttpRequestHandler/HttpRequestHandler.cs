using ProdigyAPI.BL.ViewModel.HttpRequestHandler;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.APIHandler
{
    public class HttpRequestHandler
    {
        MagnaDbEntities db = new MagnaDbEntities(true);
        public HttpRequestMessage BuildHttpRequest(string methodPath, string query, StringContent content, HttpMethod httpMethod, string environmentCode)
        {
            try
            {
                var integrationEnv = db.IntegrationEnvironments.Where(x => x.Code == environmentCode).FirstOrDefault();
                if (integrationEnv == null)
                    throw new Exception("There is no integration environment found for the environment code " + environmentCode);
                var hostUrl = integrationEnv.HostUrl;
                var token = new TokenManager().GetAccessAuthToken(environmentCode);
                
                string accessToken = token;
                
                UriBuilder uriBuilder = new UriBuilder
                {
                    //Scheme = "http",
                    Host = hostUrl,
                    //Port = 80,
                    Path = methodPath
                };
                if (!string.IsNullOrEmpty(query))
                    uriBuilder.Query = query;

                var request = new HttpRequestMessage
                {
                    Method = httpMethod,
                    RequestUri = uriBuilder.Uri,
                    Headers = {
                        { "Accept", "application/json" },
                        { "Authorization", "Bearer " + accessToken }
                    }
                };
                if (content != null)
                    request.Content = content;
                return request;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private ProdigyCoreToken GetAuthToken(string clientId)
        {
            ProdigyCoreToken accessToken = new ProdigyCoreToken();
            return accessToken;
        }
    }
}
