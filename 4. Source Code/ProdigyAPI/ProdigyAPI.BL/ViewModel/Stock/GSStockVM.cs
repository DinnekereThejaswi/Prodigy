using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Stock
{
    public class GSStockVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ItemName { get; set; }
        public string GSCode { get; set; }
        public DateTime Date { get; set; }
        public decimal OpeningUnits { get; set; }
        public decimal OpeningNetWt { get; set; }
        public decimal OpeningGrossWt { get; set; }
        public decimal ReceiptUnits { get; set; }
        public decimal ClosingUnits { get; set; }
        public decimal ReceiptNetWt { get; set; }
        public decimal ReceiptGrossWt { get; set; }
        public decimal ClosingNetWt { get; set; }
        public decimal ClosingGrossWt { get; set; }
        public DateTime? UpdateOn { get; set; }
       
    }
}
