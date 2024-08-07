using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Issues
{
    public class OPGIssueVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string IssueTo { get; set; }
        public string GSCode { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal Dcts { get; set; }
        public decimal MeltingLossWt { get; set; }
        public decimal AmountBeforeTax { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal AmountAfterTax { get; set; }
        public List<OPGIssueLineVM> IssueLines { get; set; }
    }

    public class OPGIssueLineVM
    {
        public int SlNo { get; set; }
        public int BillNo { get; set; }
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
        public OtherOPGLineAttributes OtherLineAttributes { get; set; }
        public List<OPGIssueStoneDetailVM> StoneDetails { get; set; }
    }

    public class OPGIssueStoneDetailVM
    {
        public int SlNo { get; set; }
        public string GS { get; set; }
        public string Name { get; set; }
        public int? Qty { get; set; }
        public decimal? Carrat { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
    }

    public class OtherOPGLineAttributes
    {
        public decimal? MeltingPercent { get; set; }
        public decimal? MeltingLoss { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal DiamondAmount { get; set; }
        public decimal ItemAmount { get; set; }
        public decimal PurityPercent { get; set; }
        public string ItemType { get; set; }
        public string CategoryType { get; set; }
    }

    public class OPGIssueQueryVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string IssueTo { get; set; }
        public string GSCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
