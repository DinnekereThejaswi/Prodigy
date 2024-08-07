using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Sales
{
    public class AllSalesReturnVM
    {
        public int SrEstNo { get; set; }
        public string Date { get; set; }
        public decimal Amount { get; set; }
        public decimal GrossWt { get; set; }
        public int Quantity { get; set; }
        public string TaxAmount { get; set; }
        public int CustID { get; set; }
    }

    public class AttacheSRVM
    {
        public int SRNo { get; set; }
        public string Date { get; set; }
        public decimal Amount { get; set; }
    }
}
