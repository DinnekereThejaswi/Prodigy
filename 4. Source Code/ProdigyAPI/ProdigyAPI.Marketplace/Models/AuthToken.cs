using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace.Models
{
    public class AuthToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string Username { get; set; }
        public string Issued { get; set; }
        public string Expires { get; set; }
        public string Error { get; set; }
        public string ErrorDescription { get; set; }
        public string refresh_token { get; set; }


    }
}
