using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.OldPurchase
{
    public class AllPurchaseVM
    {
        public int EstNo { get; set; }
        public string Customer { get; set; }
        public DateTime Date { get; set; }
        public decimal? Amount { get; set; }
        public decimal GrossWt { get; set; }
        public int Qty { get; set; }
        public int CustID { get; set; }
        public decimal? GSTAmount { get; set; }
    }

    public class PurchaseBillSearchVM
    {
        public int BillNo { get; set; }
        public string Customer { get; set; }
        public DateTime Date { get; set; }
        public decimal? Amount { get; set; }
        public decimal GrossWt { get; set; }
        public int Quantity { get; set; }
    }
}
