using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace.Models.BJEComm
{
    public class InventoryVM
    {
        public string barcode { get; set; }
        public string branchcode { get; set; }
        public int qty { get; set; }
        public int is_in_stock { get; set; }
    }
}
