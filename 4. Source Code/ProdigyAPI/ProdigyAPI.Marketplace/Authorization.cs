using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace
{
    public abstract class Authorization
    {
        public HttpClient clients = new HttpClient();
        public MagnaDbEntities dbContext = new MagnaDbEntities();
        public abstract string GetAccessToken();        

        public abstract string RefreshAccessToken();

        public virtual BhimaEcomIntegrationEnvironment GetEnvironment(string marketplace)
        {
            BhimaEcomIntegrationEnvironment env = dbContext.BhimaEcomIntegrationEnvironments.Where(e => e.Code == marketplace).FirstOrDefault();
            return env;
        }
    }
}
