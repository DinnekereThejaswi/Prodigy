using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Sales
{
    public class SalesStoneVM
    {

        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int BillNo { get; set; }
        public int SlNo { get; set; }
        public int EstNo { get; set; }
        public int EstSrNo { get; set; }
        public string BarcodeNo { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal Carrat { get; set; }
        public Nullable<decimal> StoneWt { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string BillType { get; set; }
        public Nullable<int> DealerSalesNo { get; set; }
        public Nullable<int> BillDet11PK { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<int> FinYear { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Shape { get; set; }
        public string Cut { get; set; }
        public string Polish { get; set; }
        public string Symmetry { get; set; }
        public string Fluorescence { get; set; }
        public string Certificate { get; set; }

    }
}
