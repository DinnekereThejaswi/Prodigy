using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Stock
{
    public class StockCheckerVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string Counter { get; set; }
        public string GS { get; set; }
        public string Item { get; set; }
        public virtual TagSummary BarcodedTags { get; set; }
        public virtual TagSummary ScannedTags { get; set; }
        public virtual TagSummary RemainingTags { get; set; }
    }

    public class TagSummary
    {
        public string StockType { get; set; }
        public int Count { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal Dcts { get; set; }
    }
}
