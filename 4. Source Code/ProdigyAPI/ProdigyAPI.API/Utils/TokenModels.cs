using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProdigyAPI.Utils
{
    public class TokenRequest
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
    }

    public class TokenResponse
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
        public int ExpiresIn { get; set; }

    }

    public class ClientInfo
    {
        public string ClientID { get; set; }
    }

    public class ErrorInfo
    {
        public string ErrorCode { get; set; }
        public string Description { get; set; }
        public string ErrorDescription { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string AuthToken { get; set; }
        public string RefreshToken { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
    }
}