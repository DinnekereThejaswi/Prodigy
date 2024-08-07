using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.OldPurchase
{
    public class PurchaseEstMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int EstNo { get; set; }
        public int BillNo { get; set; }
        public string PurItem { get; set; }
        public Nullable<int> CustID { get; set; }
        public string CustName { get; set; }
        public System.DateTime PDate { get; set; }
        public decimal Tax { get; set; }
        public Nullable<decimal> TodayRate { get; set; }
        public string OperatorCode { get; set; }
        public decimal GrandTotal { get; set; }
        public string PType { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<int> SchemeDuration { get; set; }
        public Nullable<decimal> TotalTaxAmount { get; set; }
        public Nullable<decimal> TotalPurchaseAmount { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public string Salutation { get; set; }
        public string CFlag { get; set; }
        public Nullable<decimal> RateDeduct { get; set; }
        public string IsPAN { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public string MobileNo { get; set; }
        public string State { get; set; }
        public Nullable<int> StateCode { get; set; }
        public string TIN { get; set; }
        public string PANNo { get; set; }
        public string IDType { get; set; }
        public string IDDetails { get; set; }
        public string BookingType { get; set; }
        public string PhoneNo { get; set; }
        public string EmailID { get; set; }
        public List<PurchaseEstDetailsVM> lstPurchaseEstDetailsVM { get; set; }
    }

    public class PurchaseBillMasterVM
    {
        public string ObjID { get; set; }
        public string Companycode { get; set; }
        public string BranchCode { get; set; }
        public int BillNo { get; set; }
        public int EstNo { get; set; }
        public int CustID { get; set; }
        public string CustName { get; set; }
        public System.DateTime PDate { get; set; }
        public decimal Tax { get; set; }
        public string OperatorCode { get; set; }
        public decimal GrandTotal { get; set; }
        public string PType { get; set; }
        public decimal TodayRate { get; set; }
        public string CFlag { get; set; }
        public string CancelledBy { get; set; }
        public string BillCounter { get; set; }
        public Nullable<decimal> BalanceAmt { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string CancelledRemarks { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerAddress2 { get; set; }
        public string PurItem { get; set; }
        public Nullable<int> SchemeDuration { get; set; }
        public string salutation { get; set; }
        public Nullable<decimal> RoundOff { get; set; }
        public Nullable<decimal> TotalTaxAmount { get; set; }
        public Nullable<decimal> TotalPurchaseAmount { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public string NewBillNo { get; set; }
        public Nullable<int> ShiftID { get; set; }
        public string IsPAN { get; set; }
        public Nullable<decimal> PurchaseAmount { get; set; }
        public System.Guid UniqRowID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string Pin_code { get; set; }
        public string Mobile_no { get; set; }
        public string State { get; set; }
        public Nullable<int> State_code { get; set; }
        public string Tin { get; set; }
        public string PanNo { get; set; }
    }
}
