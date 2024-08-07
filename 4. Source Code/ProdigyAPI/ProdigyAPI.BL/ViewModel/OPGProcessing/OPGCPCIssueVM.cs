using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.OPGProcessing
{
    public class OPGCPCIssueHeaderVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string IssueTo { get; set; }
        public int DocumentNo { get; set; }
        public string Remarks { get; set; }
        public List<OPGCPCIssueLineVM> OPGIssueLines { get; set; }
    }

    public class OPGCPCIssueLineVM
    {
        public string BatchId { get; set; }
        public string GSCode { get; set; }
        public string HSN { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal KovaWeight { get; set; }
        public decimal AverageRate { get; set; }
        public decimal DiamondCarets { get; set; }
        public decimal WastageWeight { get; set; }
        public decimal PurityPercent { get; set; }
        public decimal Amount { get; set; }
        public string StoneGS { get; set; }
        public string DiamondGS { get; set; }
    }
}
