using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Sales
{
    #region Input Object
    public class SalesInfoQueryVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int EstNo { get; set; }
        public bool CalculateFinalAmount { get; set; }
        public decimal TotalReceiptAmount { get; set; }
        public decimal TotalPaymentAmount { get; set; }
        public decimal DifferenceDiscountAmt { get; set; }
        public decimal ReceivableAmount { get; set; }
        public string RowVersion { get; set; }
    } 
    #endregion

    #region Output Object
    public class SalesDerivationVM
    {
        public CustomerVM CustomerInfo { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int EstNo { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal LineAmountBeforeTax { get; set; }
        public decimal OfferDiscount { get; set; }
        public decimal OtherDiscount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineAmountAfterTax { get; set; }
        public List<SalesLineVM> SalesLines { get; set; }
        public InlineOrderVM InlineOrder { get; set; }
        public InlineOldPurchaseVM InlinePurchase { get; set; }
        //public List<SalesAttachmentVM> AttachedOrders { get; set; }
        //public List<SalesAttachmentVM> AttachedPurchases { get; set; }
        //public List<SalesAttachmentVM> AttachedSRs { get; set; }
        public SalesAttachmentInfoVM AttachedOldPurchase { get; set; }
        public SalesAttachmentInfoVM AttachedSalesReturn { get; set; }
        public SalesAttachmentInfoVM AttachedCustomerOrder { get; set; }
        public SalesAttachmentInfoVM AttachedOthers { get; set; }
        public SalesInvoiceAttributeVM SalesInvoiceAttribute { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal RoundedOffAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
        public string RowVersion { get; set; }
    }

    public class CustomerVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int? CustId { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public int? RepoCustId { get; set; }
        public int? StateCode { get; set; }
    }

    public class InlineOrderVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int OrderNo { get; set; }
        public decimal Amount { get; set; }
    }

    public class InlineOldPurchaseVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int EstNo { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal LineAmountBeforeTax { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineAmountAfterTax { get; set; }
        public List<PurchaseLineVM> PurchaseLines { get; set; }
    }

    public class SalesAttachmentVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int DocumentNo { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal Amount { get; set; }
    }

    public class SalesAttachmentInfoVM
    {
        public decimal TotalGrossWt { get; set; }
        public decimal TotalStoneWt { get; set; }
        public decimal TotalNetWt { get; set; }
        public decimal TotalAmount { get; set; }
        public string DocumentInfo { get; set; }
        public List<SalesAttachmentLinesVM> AttachmentLines { get; set; }
    }

    public class SalesAttachmentLinesVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int DocumentNo { get; set; }
        public string DocumentDetail { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal Amount { get; set; }
    }

    public class SalesLineVM
    {
        public int EstNo { get; set; }
        public int SerialNo { get; set; }
        public string BarcodeNo { get; set; }
        public string Item { get; set; }
        public string Counter { get; set; }
        public decimal RatePerGram { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal MetalAmount { get; set; }
        public decimal StoneAmount { get; set; }
        public decimal DiamondAmount { get; set; }
        public decimal MCPercent { get; set; }
        public decimal MCAmount { get; set; }
        public decimal LineAmountBeforeTax { get; set; }
        public decimal OfferDiscount { get; set; }
        public decimal OtherDiscount { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal TotalGSTAmount { get { return this.CGSTAmount + this.SGSTAmount + this.IGSTAmount; } }
        public decimal LineAmountAfterTax { get; set; }
    }

    public class PurchaseLineVM
    {
        public int EstNo { get; set; }
        public int SerialNo { get; set; }
        public string Item { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal DiamondWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal PurityPercent { get; set; }
        public decimal MeltingPercent { get; set; }
        public decimal MeltingLoss { get; set; }
        public decimal PurhaseRate { get; set; }
        public decimal StoneDiamondAmount { get; set; }
        public decimal MetalAmount { get; set; }
        public decimal LineAmountBeforeTax { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal TotalGSTAmount { get { return this.CGSTAmount + this.SGSTAmount + this.IGSTAmount; } }
        public decimal LineAmountAfterTax { get; set; }
    } 
    #endregion
}
