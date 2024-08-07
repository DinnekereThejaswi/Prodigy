using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Orders
{
    public class AllOrdersVM
    {
        public int OrderNo { get; set; }
        public string RefNo { get; set; }
        public string Customer { get; set; }
        public DateTime Date { get; set; }
        public decimal? Amount { get; set; }
        public decimal GoldRate { get; set; }
        public string Type { get; set; }
        public int CustomerID { get; set; }
        public string Staff { get; set; }
        public string Karat { get; set; }
        public decimal FixedWt { get; set; }
    }
}
