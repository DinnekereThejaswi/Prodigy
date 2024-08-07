using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.BatchPosting
{
    public class SchemeInfoVM
    {
        public string BranchCode { get; set; }
        public string SchemeCode { get; set; }
        public string GroupCode { get; set; }
        public int MembershipNo { get; set; }
        public decimal AmountReceived { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal ChitAmount { get; set; }
        public decimal WinnerAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string ClosingMode { get; set; }
    }
}
