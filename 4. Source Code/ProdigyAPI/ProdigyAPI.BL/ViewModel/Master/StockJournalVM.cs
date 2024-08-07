using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public enum StockTransactionType
    {
        Sales = 0,
        SalesReturn,
        OldPurchase,
        BarcodeIssue,
        BarcodeReceipt,
        Issue,
        Receipt,
        Repair,
        CustomerOrder,
        Other
    }
    public class StockJournalVM
    {
        public StockTransactionType StockTransType { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int DocumentNo { get; set; }
        public string GS { get; set; }
        public string Counter { get; set; }
        public string Item { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
    }
}
