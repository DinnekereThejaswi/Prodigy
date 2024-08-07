using ProdigyAPI.BL.ViewModel.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Sales
{
    public class SalesDetailsVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int BillNo { get; set; }
        public int SlNo { get; set; }
        public int EstimationNo { get; set; }
        public string BarcodeNo { get; set; }
        public string SalCode { get; set; }
        public string ItemName { get; set; }
        public string CounterCode { get; set; }
        public int ItemNo { get; set; }
        public Nullable<decimal> GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal WastpPercent { get; set; }
        public decimal MakingChargePerRS { get; set; }
        public decimal VaAmount { get; set; }
        public decimal StoneCharges { get; set; }
        public decimal TotalAmount { get; set; }
        public Nullable< decimal> OfferValue { get; set; }
        public decimal GoldValue { get; set; }
        public Nullable<decimal> DiamondCharges { get; set; }
        public Nullable<decimal> AddWt { get; set; }
        public Nullable<decimal> DeductWt { get; set; }
        public Nullable<decimal> HallMarCharges { get; set; }
        public Nullable<decimal> McAmount { get; set; }
        public Nullable<decimal> WastageGrms { get; set; }
        public Nullable<decimal> McPercent { get; set; }
        public Nullable<int> AddQty { get; set; }
        public Nullable<int> DeductQty { get; set; }
        public Nullable<decimal> OofferValue { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string GsCode { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string karat { get; set; }
        public string AdBarcode { get; set; }
        public string AdCounter { get; set; }
        public string AdItem { get; set; }
        public string isEDApplicable { get; set; }
        public Nullable<int> McType { get; set; }
        public Nullable<int> FinalYear { get; set; }
        public string NewBillNo { get; set; }
        public Nullable<decimal> ItemAdditionalDiscount { get; set; }
        public Nullable<decimal> ItemTotalAfterDiscount { get; set; }
        public Nullable<decimal> TaxPercentage { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<decimal> ItemFinalAmount { get; set; }
        public string SupplierCode { get; set; }
        public string ItemSize { get; set; }
        public string ImgId { get; set; }
        public string DesignCode { get; set; }
        public string DesignName { get; set; }
        public string BatchId { get; set; }
        public string RfID { get; set; }
        public Nullable<decimal> McPerPiece { get; set; }
        public Nullable<decimal> RoundOff { get; set; }
        public Nullable<decimal> ItemFinalAmountAfterRoundoff { get; set; }
        public Nullable<decimal> PurityPer { get; set; }
        public Nullable<decimal> MeltingPercent { get; set; }
        public Nullable<decimal> MeltingLoss { get; set; }
        public string ItemType { get; set; }
        public Nullable<decimal> DiscountMc { get; set; }
        public Nullable<decimal> TotalSalesMc { get; set; }
        public Nullable<decimal> McDiscountAmt { get; set; }
        public Nullable<decimal> PurchaseMc { get; set; }
        public string GSTGroupCode { get; set; }
        public Nullable<decimal> SGSTPercent { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> CGSTPercent { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> IGSTPercent { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public string HSN { get; set; }
        public string PieceRate { get; set; }
        public System.Guid UniqRowID { get; set; }
        public Nullable<decimal> DeductSWt { get; set; }
        public Nullable<decimal> OrdDiscountAmt { get; set; }
        public string DedCounter { get; set; }
        public string DedItem { get; set; }
        public List<SalesStoneVM> salesstoneVM { get; set; }

    }
}
