using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace.Models.BJEComm
{
    #region Token View Models
    public class AuthtokenRequestVM
    {
        public string token { get; set; }
    }

    public class AuthtokenResponseVM
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public DateTime expiresIn { get; set; }
    }

    public class TokenErrorResponseVM
    {
        public string message { get; set; }
    }
    #endregion
}
