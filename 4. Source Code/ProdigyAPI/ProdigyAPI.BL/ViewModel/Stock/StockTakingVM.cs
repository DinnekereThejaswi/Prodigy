using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Stock
{
    public class StockTakingVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public Nullable<int> BatchNo { get; set; }
        public string BarcodeNo { get; set; }
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal NetWt { get; set; }
        public string SalCode { get; set; }
        public string CounterCode { get; set; }
        public Nullable<decimal> Dcts { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }

    }

    public class StockTakingHeader
    {
        public string Item { get; set; }
        public string Counter { get; set; }
        public string Salesman { get; set; }
        public int Count { get; set; }
    }
      
}
