using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace.Models.BJEComm
{
    public class OrderVM
    {
        public string orderreferanceno { get; set; }
        public string branchorderno { get; set; }
        public string status { get; set; }
        public string awno { get; set; }
        public string cancelled_by { get; set; }
        public string comments { get; set; }
    }
}
