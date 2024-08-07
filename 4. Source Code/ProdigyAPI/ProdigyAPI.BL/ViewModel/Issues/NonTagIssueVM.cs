using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Issues
{
    public class NonTagIssueVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string IssueTo { get; set; }
        public string Remarks { get; set; }
        public decimal BoardRate { get; set; }
        public decimal WastageGrams { get; set; }
        public decimal MakingChargesRs { get; set; }
        public decimal HallmarkingChargesRs { get; set; }
        public List<NonTagIssueLineVM> IssueLines { get; set; }
    }

    public class NonTagIssueLineVM
    {
        public int SlNo { get; set; }
        public string CounterCode { get; set; }
        public string GSCode { get; set; }
        public string ItemCode { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal Dcts { get; set; }
        public decimal Rate { get; set; }
        public decimal PurityPercent { get; set; }
        public decimal PureWeight { get; set; }
        public decimal AmountBeforeTax { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal AmountAfterTax { get; set; }
        public List<NonTagIssueStoneDetailVM> StoneDetails { get; set; }
    }

    public class NonTagIssueStoneDetailVM
    {
        public int SlNo { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal Carrat { get; set; }
        public decimal Weight { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }

    public class ClosingStockVM
    {
        public int Qty { get; set; }
        public decimal? GrossWt { get; set; }
        public decimal NetWt { get; set; }
    }
}
