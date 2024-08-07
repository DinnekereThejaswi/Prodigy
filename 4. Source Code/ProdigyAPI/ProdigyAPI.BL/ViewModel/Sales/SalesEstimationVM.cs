using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Sales
{
    public class SalesEstDetailsVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int EstNo { get; set; }
        public int SlNo { get; set; }
        public Nullable<int> BillNo { get; set; }
        public string BarcodeNo { get; set; }
        public string SalCode { get; set; }
        public string CounterCode { get; set; }
        public string ItemName { get; set; }
        public int ItemNo { get; set; }
        public int? ItemQty { get; set; }
        public decimal Grosswt { get; set; }
        public decimal Stonewt { get; set; }
        public decimal Netwt { get; set; }
        public Nullable<decimal> AddWt { get; set; }
        public Nullable<decimal> DeductWt { get; set; }
        public decimal MakingChargePerRs { get; set; }
        public decimal WastPercent { get; set; }
        public decimal GoldValue { get; set; }
        public decimal VaAmount { get; set; }
        public decimal StoneCharges { get; set; }
        public Nullable<decimal> DiamondCharges { get; set; }
        public decimal TotalAmount { get; set; }
        public Nullable<decimal> Hallmarkarges { get; set; }
        public Nullable<decimal> McAmount { get; set; }
        public Nullable<decimal> WastageGrms { get; set; }
        public Nullable<decimal> McPercent { get; set; }
        public Nullable<int> AddQty { get; set; }
        public Nullable<int> DeductQty { get; set; }
        public Nullable<decimal> OfferValue { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string GsCode { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string Karat { get; set; }
        public string AdBarcode { get; set; }
        public string AdCounter { get; set; }
        public string AdItem { get; set; }
        public string IsEDApplicable { get; set; }
        public int? McType { get; set; }
        public Nullable<int> Fin_Year { get; set; }
        public string NewBillNo { get; set; }
        public Nullable<decimal> ItemTotalAfterDiscount { get; set; }
        public Nullable<decimal> ItemAdditionalDiscount { get; set; }
        public Nullable<decimal> TaxPercentage { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<decimal> ItemFinalAmount { get; set; }
        public string SupplierCode { get; set; }
        public string ItemSize { get; set; }
        public string ImgID { get; set; }
        public string DesignCode { get; set; }
        public string DesignName { get; set; }
        public string BatchID { get; set; }
        public string Rf_ID { get; set; }
        public Nullable<decimal> McPerPiece { get; set; }
        public Nullable<decimal> DiscountMc { get; set; }
        public Nullable<decimal> TotalSalesMc { get; set; }
        public Nullable<decimal> McDiscountAmt { get; set; }
        public Nullable<decimal> purchaseMc { get; set; }
        public string GSTGroupCode { get; set; }
        public Nullable<decimal> SGSTPercent { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> CGSTPercent { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> IGSTPercent { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public string HSN { get; set; }
        public string PieceRate { get; set; }
        public Nullable<decimal> DeductSWt { get; set; }
        public Nullable<decimal> OrdDiscountAmt { get; set; }
        public string DedCounter { get; set; }
        public string DedItem { get; set; }
        public int OrderNo { get; set; }
        public int BillQty { get; set; }
        public int RoundingOfAmount { get; set; }
        public decimal RoundingOfValue { get; set; }
        public decimal? CESSPercent { get; set; }
        public decimal? CESSAmount { get; set; }
        public int isInterstate { get; set; }

        public List<SalesEstPrintTotals> lstOfSalesEstPrintTotals = new List<SalesEstPrintTotals>();
        public List<SalesEstStoneVM> salesEstStoneVM { get; set; }

        public string TaggingType { get; set; }

        public int Quantity { get; set; }
        public decimal BillingGrossWt { get; set; }
        public decimal BillingStoneWt { get; set; }
        public decimal BillingNetWt { get; set; }
        public decimal? VaporLossWeight { get; set; }
        public decimal? VaporLossAmount { get; set; }
        public string Huid { get; set; }
    }

    public class SalesEstPrintTotals
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
