using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Accounts
{
    public class CashInHandVM
    {
        public string ObjID { get; set; }
        public string BranchCode { get; set; }
        public string CompanyCode { get; set; }
        public int SlNo { get; set; }
        public Nullable<decimal> CashBalance { get; set; }
        public Nullable<decimal> CashInHand { get; set; }
        public System.DateTime BillDate { get; set; }
        public Nullable<int> FinYear { get; set; }
    }
}
