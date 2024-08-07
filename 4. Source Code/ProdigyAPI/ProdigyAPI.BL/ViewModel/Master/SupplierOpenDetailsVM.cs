using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class SupplierOpenDetailsVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string PartyCode { get; set; }
        public string VoucherCode { get; set; }
        public decimal OpnBal { get; set; }
        public string BalType { get; set; }
        public decimal OpnWeight { get; set; }
        public string WeightType { get; set; }
        public string ObjectStatus { get; set; }
        public DateTime UpdateOn { get; set; }
        public string MetalCode { get; set; }
        public decimal? OpnPureWeight { get; set; }
        public decimal? OpnNetWeight { get; set; }
        public int? FinYear { get; set; }
        public string GSCode { get; set; }
        public int? RefNo { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}
