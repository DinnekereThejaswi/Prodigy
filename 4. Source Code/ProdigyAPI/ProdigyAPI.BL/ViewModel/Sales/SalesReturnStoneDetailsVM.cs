using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Sales
{
    public class SalesReturnStoneDetailsVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int SalesBillNo { get; set; }
        public int EstNo { get; set; }
        public int ItemSno { get; set; }
        public int Sno { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal Carrat { get; set; }
        public Nullable<decimal> StoneWt { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<int> FinYear { get; set; }
        public string BarcodeNo { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}
