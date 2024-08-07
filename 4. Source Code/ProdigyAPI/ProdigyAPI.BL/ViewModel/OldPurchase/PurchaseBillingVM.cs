using ProdigyAPI.BL.ViewModel.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.OldPurchase
{
    public class PurchaseBillingVM
    {
        public string ObjID { get; set; }

        public int SlNo { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int EstNo { get; set; }
        public string OperatorCode { get; set; }
        public string BillCounter { get; set; }
        public string Type { get; set; }
        public int BillNo { get; set; }

        public int CustNo { get; set; }
        public string CustName { get; set; }
        public decimal PaidAmount { get; set; }
        public string CancelRemarks { get; set; }
        public List<PaymentVM> lstOfPayment { get; set; }
    }
}
