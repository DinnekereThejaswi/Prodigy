using ProdigyAPI.BL.ViewModel.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Sales
{
    public class SalesMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int BillNo { get; set; }
        public int EstimationNo { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderAmount { get; set; }
        public DateTime BillDate { get; set; }
        public string OperatorCode { get; set; }
        public string Karat { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public decimal Tax { get; set; }
        public Nullable<decimal> CessTax { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> TotalCessAmount { get; set; }
        public Nullable<decimal> TotalTaxAmount { get; set; }
        public decimal TotalBillAmount { get; set; }
        public Nullable<decimal> GrandTotal { get; set; }
        public Nullable<decimal> PurchaseAmount { get; set; }
        public Nullable<decimal> SpurchaseAmount { get; set; }
        public string Stype { get; set; }
        public string Cflag { get; set; }
        public string CancelledBy { get; set; }
        public string DiscountAutharity { get; set; }
        public Nullable<int> PurchaseNo { get; set; }
        public Nullable<int> SpurchaseNo { get; set; }
        public string GStype { get; set; }
        public string IsCredit { get; set; }
        public string BillCounter { get; set; }
        public string Is_Ins { get; set; }
        public string CancelledRemarks { get; set; }
        public string GuranteeName { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<decimal> BalanceAmt { get; set; }
        public string NewCust { get; set; }
        public Nullable<int> SalesBillPK { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string ItemSet { get; set; }
        public Nullable<decimal> TotalSaleAmount { get; set; }
        public Nullable<decimal> EdCess { get; set; }
        public Nullable<decimal> HedCess { get; set; }
        public Nullable<decimal> EdCessPercent { get; set; }
        public Nullable<decimal> HedCessPercent { get; set; }
        public Nullable<decimal> ExciseDutyPercent { get; set; }
        public Nullable<decimal> ExciseDutyAmount { get; set; }
        public Nullable<int> FinalYear { get; set; }
        public Nullable<decimal> HISchemeAmount { get; set; }
        public Nullable<int> HISchemeNo { get; set; }
        public Nullable<decimal> HIBonusAmount { get; set; }
        public string Salutation { get; set; }
        public string NewBillNo { get; set; }
        public Nullable<decimal> RoundOff { get; set; }
        public string ReferedBy { get; set; }
        public Nullable<int> ShiftID { get; set; }
        public string BookNo { get; set; }
        public string RefInvoiceNo { get; set; }
        public string FlightNo { get; set; }
        public string Race { get; set; }
        public string MangerCode { get; set; }
        public string BillType { get; set; }
        public string IsPAN { get; set; }
        public System.Guid UniqRowID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public string MobileNo { get; set; }
        public string State { get; set; }
        public Nullable<int> StateCode { get; set; }
        public string Tin { get; set; }
        public string PANNo { get; set; }
        public Nullable<int> pos { get; set; }
        public string CorparateID { get; set; }
        public string CorporateBranchID { get; set; }
        public string EmployeeID { get; set; }
        public string RegisteredMN { get; set; }
        public string ProfessionID { get; set; }
        public string EmpcorpEmailID { get; set; }
        public List<SalesDetailsVM> salesDetailsVM { get; set; }
        public List<PaymentVM> PaymentVM { get; set; }
        public decimal BillAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }

        public SalesInvoiceAttributeVM salesInvoiceAttribute { get; set; }
    }

    public class SalesBillingVM
    {
        public int CustID { get; set; }
        public string MobileNo { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int EstNo { get; set; }
        public decimal ReceivableAmt { get; set; }
        public decimal BalanceAmt { get; set; }
        public decimal DifferenceDiscountAmt { get; set; }
        public decimal PayableAmt { get; set; }
        public decimal TCSAmt { get; set; }
        public string OperatorCode { get; set; }
        public string BillCounter { get; set; }
        public string GuaranteeName { get; set; }
        public string GuaranteeMobileNo { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsOrder { get; set; }
        public bool IsCreditNote { get; set; }
        public bool IsTCSBill { get; set; }
        public string SalesManCode { get; set; }
        public string PANNo { get; set; }
        public string BillType { get; set; }
        public string RowRevision { get; set; }
        public List<PaymentVM> lstOfPayment { get; set; }
        public List<PaymentVM> lstOfPayToCustomer { get; set; }
        public SalesInvoiceAttributeVM salesInvoiceAttribute { get; set; }
        public List<OrderOTPAttributes> orderOTPAttributes { get; set; }
        public OrderAttributes orderAttributes { get; set; }
    }

    public class SalesInvoiceAttributeVM
    {
        public decimal SalesAmountExclTax { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal GSTCessAmount { get; set; }
        public decimal GSTAmountInclCess { get; set; }
        public decimal SalesAmountInclTax { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal OldPurchaseAmount { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal OrderRateDiscount { get; set; }
        public decimal OfferDiscount { get; set; }
        public decimal MCDiscount { get; set; }
        public decimal AdditionalDiscount { get; set; }
        public decimal SalesAmountInclTaxWithoutRoundOff { get; set; }

    }

    public class OrderOTPAttributes
    {
        public int OrderNo { get; set; }
        public string MobileNo { get; set; }
        public int SMSID { get; set; }

        [System.ComponentModel.DefaultValue(false)]
        public bool IsValidated { get; set; }
    }

    public class OrderOTPPaymentVM
    {
        public string MobileNo { get; set; }
        public int OrderNo { get; set; }
    }

    public class SalesBillInfo
    {
        public string Companycode { get; set; }
        public string BranchCode { get; set; }
        public int BillNo { get; set; }
        public List<int> Purchase { get; set; }
        public List<int> Orders { get; set; }
        public List<int> SalesReturn { get; set; }
        public CreditNoteInfo CreditNote { get; set; }
    }

    public class BarcodeAgeInfo
    {
        public string BarcodeNo { get; set; }
        public int AgeInDays { get; set; }
        public string AgeText { get; set; }
        public string BackgroundColour { get; set; }
        public string ForegroundColour { get; set; }
        public string ItemAge { get; set; }
        public string AgeInBranch { get; set; }
    }


    public class CreditNoteInfo
    {
        public int OrderNo { get; set; }
        public int ReceiptNo { get; set; }
    }

    public class OrderAttributes
    {
        public string OrderRateType { get; set; }
        public decimal Rate { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}
