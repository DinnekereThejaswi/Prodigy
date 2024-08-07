using ProdigyAPI.BL.ViewModel.Payment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Sales
{
    public class SalesEstMasterVM
    {
        public string ObjID { get; set; }

        [Required(ErrorMessage = "Company code is required.")]
        public string CompanyCode { get; set; }

        [Required(ErrorMessage = "Branch code is required.")]
        public string BranchCode { get; set; }
        public int EstNo { get; set; }
        public int OrderNo { get; set; }
        public int CustID { get; set; }
        public string CustName { get; set; }
        public System.DateTime OrderDate { get; set; }
        public System.DateTime EstDate { get; set; }

        [Required(ErrorMessage = "Operator code is required.")]
        public string OperatorCode { get; set; }

        [Required(ErrorMessage = "Karat is required.")]
        public string Karat { get; set; }
        public decimal Rate { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalEstAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal SPurchaseAmount { get; set; }
        public string SType { get; set; }
        public int BillNo { get; set; }
        public string GSType { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> OfferDiscountAmount { get; set; }
        public string OfferCode { get; set; }
        public string ApprovedBy { get; set; }
        public string IsIns { get; set; }
        public Nullable<int> SrBillNo { get; set; }
        public string ItemSet { get; set; }
        public string Remarks { get; set; }
        public string ApproveFlag { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<decimal> EdCess { get; set; }
        public Nullable<decimal> HedCEss { get; set; }
        public Nullable<decimal> EdCessPercent { get; set; }
        public Nullable<decimal> HedCessPercent { get; set; }
        public Nullable<decimal> ExciseDutyPercent { get; set; }
        public Nullable<decimal> ExciseDutyAmount { get; set; }
        public Nullable<decimal> HiSchemeAmount { get; set; }
        public Nullable<int> HiSchemeNumber { get; set; }
        public Nullable<decimal> HiBonusAmount { get; set; }
        public string Salutation { get; set; }
        public string NewBillNo { get; set; }
        public string IsPAN { get; set; }
        public Nullable<decimal> RoundOff { get; set; }
        public Nullable<int> StateCode { get; set; }
        public Nullable<int> Pos { get; set; }
        public string CorparateID { get; set; }
        public string CorporateBranchID { get; set; }
        public string EmployeeId { get; set; }
        public string RegisteredMN { get; set; }
        public string ProfessionID { get; set; }
        public string EmpcorpEmailID { get; set; }
        public string PhoneNo { get; set; }
        public string EmailID { get; set; }
        public string IdType { get; set; }
        public string IdDetails { get; set; }
        public string PANNo { get; set; }
        public string TIN { get; set; }
        public string Pincode { get; set; }
        public string MobileNo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address1 { get; set; }
        public bool IsMergeEst { get; set; }
        public int MergedEstNo { get; set; }
        public bool IsOfferApplied { get; set; }
        public bool IsOfferSkipped { get; set; }
        public List<SalesEstDetailsVM> salesEstimatonVM { get; set; }
        public List<PaymentVM> paymentVM { get; set; }
        public SalesInvoiceAttributeVM offerDiscount { get; set; }
        public string RowRevisionString { get; set; }

        public bool Overwrite { get; set; }

    }
    public class MergeEstMaster
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public List<MergeEstDet> EstDet { get; set; }
    }
    public class MergeEstDet
    {
        public int EstNo { get; set; }
        public List<MegeEstLineItem> salesEstimatonVM { get; set; }
    }
    public class MegeEstLineItem
    {
        public int SlNo { get; set; }
        public string BarcodeNo { get; set; }

        public string GSCode { get; set; }
    }

    public class SalesEstimateRowVersion
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int EstNo { get; set; }
        public bool Invoiced { get; set; }
        public string RowRevisionString { get; set; }
    }

    public class DiscountOutput
    {
        public string OfferCode { get; set; }
        public string OfferName { get; set; }
        public bool DiscountAfterTax { get; set; }
        public decimal DiscountAmount { get; set; }
        public string CalculationLogic { get; set; }
    }
    public class SalesEstModel
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int EstimationNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
        public List<SalesEstDetailModel> SalesEstDetail { get; set; }
    }

    public class SalesEstDetailModel
    {
        public int SlNo { get; set; }
        public string GSCode { get; set; }
        public string BarcodeNo { get; set; }
        public string ItemCode { get; set; }
        public string SalesmanCode { get; set; }
        public string CounterCode { get; set; }
        public bool MRPItem { get; set; }
        public int Qty { get; set; }
        public decimal Grosswt { get; set; }
        public decimal Stonewt { get; set; }
        public decimal Netwt { get; set; }      
        public decimal MetalValue { get; set; }
        public decimal VAAmount { get; set; }
        public decimal StoneCharges { get; set; }
        public decimal DiamondCharges { get; set; }
        public decimal Dcts { get; set; }
        public decimal ItemAmount { get; set; }
    }
}
