using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.HttpRequestHandler
{
    public class ProdigyCoreToken
    {
        public TokenCredentials Credentials { get; set; }
        public ClientInfo ClientInfomation { get; set; }
    }

    public class TokenCredentials
    {
        public string UserID { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }

    }

    public class ClientInfo
    {
        public string ClientID { get; set; }
    }
}
