using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Issues
{    
    public class SRIssueVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string IssueTo { get; set; }
        public string GSCode { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal AmountBeforeTax { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal AmountAfterTax { get; set; }
        public List<SRIssueLineVM> IssueLines { get; set; }
    }

    public class SRIssueLineVM
    {
        public int SlNo { get; set; }
        public int BillNo { get; set; }
        public string BarcodeNo { get; set; }
        public string GSCode { get; set; }
        public string CounterCode { get; set; }
        public string ItemCode { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal AmountBeforeTax { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal AmountAfterTax { get; set; }
        public OtherSRLineAttributes OtherLineAttributes { get; set; }
        public List<SRIssueStoneDetailVM> StoneDetails { get; set; }
    }

    public class SRIssueStoneDetailVM
    {
        public int SlNo { get; set; }
        public int BillNo { get; set; }
        public int ItemSlNo { get; set; }
        public string GS { get; set; }
        public string Name { get; set; }
        public int? Qty { get; set; }
        public decimal? Carrat { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
    }

    public class OtherSRLineAttributes
    {
        public Nullable<decimal> StoneCharges { get; set; }
        public Nullable<decimal> DiamondCharges { get; set; }
        public decimal VAAmount { get; set; }
        public string Supplier { get; set; }
        public string BatchID { get; set; }
        public string DesignCode { get; set; }
        public string ItemSize { get; set; }
        public decimal PurchaseRate { get; set; }
        public int? PurchaseWastageType { get; set; }
        public decimal PurchaseWastageValue { get; set; }
        public int? PurchaseMCType { get; set; }
        public decimal PurchaseMCGram { get; set; }
    }

    public class SRIssueQueryVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string IssueTo { get; set; }
        public string GSCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
