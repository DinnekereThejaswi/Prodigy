using ProdigyAPI.BL.ViewModel.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Credit
{
    public class CreditReceiptVM
    {
        public int BillNo { get; set; }
        public int LastReceiptNo { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string CustomerName { get; set; }
        public int CustID { get; set; }
        public int? FinancialYear { get; set; }
        public decimal? SalesAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public string EmailID { get; set; }
        public string PANNo { get; set; }
        public string IDType { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<PaymentVM> lstOfPayment { get; set; }
    }

    public class CancelCreditReceiptVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int ReceiptNo { get; set; }
        public int FinYear { get; set; }
        public int BillNo { get; set; }
        public DateTime BilledDate { get; set; }
        public string CFlag { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string CustomerName { get; set; }
        public int CustID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public decimal? CreditBalance { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public List<PaymentVM> lstOfPayment { get; set; }
    }
}
