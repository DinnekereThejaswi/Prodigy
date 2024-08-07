using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Sales
{
    public class SalesReturnDetailsVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int SalesBillNo { get; set; }
        public int SlNo { get; set; }
        public int EstNo { get; set; }
        public string SalCode { get; set; }
        public string ItemName { get; set; }
        public string CounterCode { get; set; }
        public int Quantity { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public Nullable<decimal> WastePercent { get; set; }
        public Nullable<decimal> MakingChargePerRs { get; set; }
        public decimal VAAmount { get; set; }
        public decimal SRAmount { get; set; }
        public Nullable<decimal> StoneCharges { get; set; }
        public Nullable<decimal> DiamondCharges { get; set; }
        public decimal NetAmount { get; set; }
        public Nullable<int> AddQty { get; set; }
        public Nullable<int> DeductQty { get; set; }
        public Nullable<decimal> AddWt { get; set; }
        public Nullable<decimal> DeductWt { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<decimal> VAPecent { get; set; }
        public string GSCode { get; set; }
        public Nullable<int> FinYear { get; set; }
        public string BarcodeNo { get; set; }
        public string SupplierCode { get; set; }
        public Nullable<decimal> ItemAdditionalDiscount { get; set; }
        public Nullable<decimal> ItemTotalAfterDiscount { get; set; }
        public Nullable<decimal> TaxPercentage { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public string ItemSize { get; set; }
        public string ImageID { get; set; }
        public string DesignCode { get; set; }
        public string DesignName { get; set; }
        public string BatchID { get; set; }
        public string RFID { get; set; }
        public Nullable<decimal> MCPerPiece { get; set; }
        public Nullable<decimal> OfferValue { get; set; }
        public Nullable<decimal> ItemFinalAmount { get; set; }
        public Nullable<int> MCType { get; set; }
        public Nullable<decimal> RoundOff { get; set; }
        public Nullable<decimal> ItemFinalAmountAfterRoundOff { get; set; }
        public string OriginalSalesBillNo { get; set; }
        public string ItemType { get; set; }
        public Nullable<decimal> CSTAmount { get; set; }
        public Nullable<decimal> Dcts { get; set; }
        public string GSTGroupCode { get; set; }
        public Nullable<decimal> SGSTPercent { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> CGSTPercent { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> IGSTPercent { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public string HSN { get; set; }
        public Nullable<decimal> DiscountMc { get; set; }
        public Nullable<decimal> McDiscountAmt { get; set; }
        public Nullable<decimal> CessPercent { get; set; }
        public Nullable<decimal> CessAmount { get; set; }
        public string IsBarcoded { get; set; }
        public decimal DiscVoucherAmt { get; set; }
        public List<SalesReturnStoneDetailsVM> lstOfStoneDetails { get; set; }
    }
}
