using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Repair
{
    public class RepairIssueDetailsVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int IssueNo { get; set; }
        public int ReceiptNo { get; set; }
        public int SlNo { get; set; }
        public string Item { get; set; }
        public int Units { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> WastageGrams { get; set; }
        public Nullable<decimal> WtAdd { get; set; }
        public Nullable<decimal> GoldRate { get; set; }
        public Nullable<decimal> GoldAmount { get; set; }
        public Nullable<decimal> MiscAmount { get; set; }
        public Nullable<decimal> RepairAmount { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string GSCode { get; set; }
        public Nullable<int> FinYear { get; set; }
        public Nullable<decimal> ItemwiseTaxPercentage { get; set; }
        public Nullable<decimal> ItemwiseTaxAmount { get; set; }
        public Nullable<decimal>  ItemwiseTotalAmountBeforeTax { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string PartyCode { get; set; }
        public string GSTGroupCode { get; set; }
        public Nullable<decimal> SGSTPercent { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> CGSTPercent { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> IGSTPercent { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public string HSN { get; set; }
        public Nullable<decimal> Dcts { get; set; }

        //public string ObjID { get; set; }
        //public string CompanyCode { get; set; }
        //public string BranchCode { get; set; }
        //public int RepaireNo { get; set; }
        //public int SlNo { get; set; }
        //public string Item { get; set; }
        //public int Units { get; set; }
        //public decimal GrossWt { get; set; }
        //public decimal StoneWt { get; set; }
        //public decimal NetWt { get; set; }
        //public string Description { get; set; }
        //public Nullable<System.DateTime> UpdateOn { get; set; }
        //public string GSCode { get; set; }
        //public Nullable<int> FinYear { get; set; }
        //public Nullable<decimal> Rate { get; set; }
        //public string PartyCode { get; set; }
        //public Nullable<decimal> Dcts { get; set; }
    }
}
