using ProdigyAPI.BL.ViewModel.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Sales
{
    public class SalesReturnMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int VotureBillNo { get; set; }
        public int SalesBillNo { get; set; }
        public int EstNo { get; set; }
        public int CustomerID { get; set; }
        public System.DateTime BillDate { get; set; }
        public Nullable<System.DateTime> SalesReturnDate { get; set; }
        public string GSType { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public decimal TotalSRAmount { get; set; }
        public string OperatorCode { get; set; }
        public string SalCode { get; set; }
        public string BillCounter { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string ISIns { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string BilledBranch { get; set; }
        public string CFlag { get; set; }
        public Nullable<decimal> ExciseDutyPercent { get; set; }
        public Nullable<decimal> EDCessPercent { get; set; }
        public Nullable<decimal> HEDCessPercent { get; set; }
        public Nullable<decimal> ExciseDutyAmount { get; set; }
        public Nullable<decimal> EDCessAmount { get; set; }
        public Nullable<decimal> HEDCessAmount { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public string IsAdjusted { get; set; }
        public string CancelledBy { get; set; }
        public string CancelledRemarks { get; set; }
        public string NewBillNo { get; set; }
        public Nullable<decimal> RoundOff { get; set; }
        public Nullable<int> ShiftID { get; set; }
        public Nullable<int> InvoiceTypeID { get; set; }
        public Nullable<decimal> TotalCSTAmount { get; set; }
        public Nullable<decimal> VPAmount { get; set; }
        public Nullable<decimal> PayableAmount { get; set; }
        public string CustomerName { get; set; }
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
        public string Type { get; set; }
        public bool IsCreditBill { get; set; }
        public decimal PartialPaidAmount { get; set; }
        public bool IsAdjust { get; set; }
        public bool IsPayable { get; set; }
        public bool IsOrder { get; set; }
        public decimal TotalDiscVoucherAmt { get; set; }
        public List<SalesReturnDetailsVM> lstOfSalesReturnDetails { get; set; }
        public List<PaymentVM> lstOfPayment { get; set; }
    }

    public class ConfirmSalesReturnVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }

        public string ObjectID { get; set; }

        public int SlNo { get; set; }
        public int EstNo { get; set; }
        public string OperatorCode { get; set; }
        public string BillCounter { get; set; }
        public string Type { get; set; }
        public int BillNo { get; set; }
        public decimal PaidAmount { get; set; }
        public string CancleRemarks { get; set; }
        public bool IsCreditBill { get; set; }
        public decimal VPAmount { get; set; }
        public bool IsOrder { get; set; }

        public bool IsAdjust { get; set; }

        public bool IsPayable { get; set; }

        public decimal PartialPaidAmount { get; set; }

        public decimal TotalSRAmount { get; set; }
        public decimal TotalDiscVoucherAmt { get; set; }
        public List<PaymentVM> lstOfPayment { get; set; }
    }

    public class ConfirmVM
    {
        public int SalesBillNo { get; set; }

        public int OrderNo { get; set; }

        public int ReceiptNo { get; set; }

        public int CreditReceiptNo { get; set; }
    }
}
