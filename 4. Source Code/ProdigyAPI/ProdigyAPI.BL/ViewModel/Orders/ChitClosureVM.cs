using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Orders
{
    public class ChitClosureVM
    {
        public string objID { get; set; }
        public string SchemeCode { get; set; }
        public string GroupCode { get; set; }
        public int ChitMembShipNo { get; set; }
        public decimal AmtReceived { get; set; }
        public decimal BonusAmt { get; set; }
        public decimal InterestPercentage { get; set; }
        public decimal InterestAmt { get; set; }
        public decimal GiftAmount { get; set; }
        public decimal DeductionAmt { get; set; }
        public string ClosingMode { get; set; }
        public System.DateTime ClosingDate { get; set; }
        public string BillNumber { get; set; }
        public Nullable<System.DateTime> BillDate { get; set; }
        public decimal ChitAmt { get; set; }
        public string ObjectStatus { get; set; }
        public Nullable<decimal> TotalWtAcc { get; set; }
        public Nullable<int> ChitTransNo { get; set; }
        public Nullable<decimal> WinAmt { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public Nullable<System.DateTime> ChqIssueDate { get; set; }
        public Nullable<int> ChqNo { get; set; }
        public Nullable<int> AccCode { get; set; }
        public Nullable<decimal> AmtCollected { get; set; }
        public string ClosedAt { get; set; }
        public string ClosedBy { get; set; }
        public Nullable<int> OldBillNumber { get; set; }
        public Nullable<bool> IsUpdated { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }
        public Nullable<int> ClosureUniqId { get; set; }
    }
}
