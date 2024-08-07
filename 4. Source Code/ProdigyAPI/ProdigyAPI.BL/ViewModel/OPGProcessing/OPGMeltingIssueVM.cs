using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.OPGProcessing
{

    public class OPGMeltingIssueHeaderVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string IssueTo { get; set; }
        public string Remarks { get; set; }
        public List<OPGMeltingIssueBatchDetailVM> OPGBatchLines { get; set; }
    }

    public class OPGMeltingIssueBatchDetailVM
    {
        public string GSCode { get; set; }
        public string BatchId { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
    }
}
