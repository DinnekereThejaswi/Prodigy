using ProdigyAPI.BL.ViewModel.Payment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Orders
{
    public class OrderMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int OrderNo { get; set; }
        public int CustID { get; set; }
        public string CustName { get; set; }
        public string OrderType { get; set; }
        public string Remarks { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string OperatorCode { get; set; }
        public string SalCode { get; set; }

        [Required]
        public System.DateTime DeliveryDate { get; set; }
        public string OrderRateType { get; set; }
        public string gsCode { get; set; }
        public decimal Rate { get; set; }
        public Nullable<decimal> AdvacnceOrderAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string ObjectStatus { get; set; }
        public int BillNo { get; set; }
        public string CFlag { get; set; }
        public string CancelledBy { get; set; }
        public string BillCounter { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string ClosedBranch { get; set; }
        public string ClosedFlag { get; set; }
        public string ClosedBy { get; set; }
        public Nullable<System.DateTime> ClosedDate { get; set; }
        public string Karat { get; set; }
        public Nullable<decimal> OrderDayRate { get; set; }
        public string NewBillNo { get; set; }
        public Nullable<decimal> TaxPercentage { get; set; }
        public Nullable<decimal> TotalAmountBeforeTax { get; set; }
        public Nullable<decimal> TotalTaxAmount { get; set; }
        public Nullable<int> ShiftID { get; set; }
        public string IsPAN { get; set; }
        public Nullable<int> OldOrderNo { get; set; }
        public System.Guid UniqRowID { get; set; }
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
        public Nullable<int> ESTNo { get; set; }
        public string IDType { get; set; }
        public string IDDetails { get; set; }
        public string BookingType { get; set; }
        public string PhoneNo { get; set; }
        public string EmailID { get; set; }
        public string Salutation { get; set; }
        public string ManagerCode { get; set; }
        public string OrderAdvanceRateType { get; set; }
        public int? RateValidityDays { get; set; }
        public DateTime? RateValidityTill { get; set; }
        public Nullable<decimal> AdvancePercent { get; set; }
        public Nullable<int> CentralRefNo { get; set; }
        public string MarketPlaceOrderNo { get; set; }

        public string CustDocType { get; set; }
        public string CustDocValue { get; set; }

        public string IsLock { get; set; }
        public List<OrderItemDetailsVM> lstOfOrderItemDetailsVM { get; set; }
        public List<PaymentVM> lstOfPayment { get; set; }
    }

    public class OrderPurchasePlanTypeVM
    {
        public string BookingTypeCode { get; set; }
        public string Name { get; set; }
    }

    public class OrderPurchasePlanDetailVM
    {
        public string BookingTypeCode { get; set; }
        public string Description { get; set; }
        public decimal AdvanceAmountPercent { get; set; }
        public int FixedDays { get; set; }
        public Nullable<decimal> MinimumWeightGrams { get; set; }
    }

    public class OrderInfoVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int OrderNo { get; set; }
        public List<int> ReceiptNo { get; set; }
    }
}
