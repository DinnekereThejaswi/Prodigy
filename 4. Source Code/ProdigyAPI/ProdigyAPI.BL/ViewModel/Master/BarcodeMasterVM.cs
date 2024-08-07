using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class BarcodeMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BarcodeNo { get; set; }
        public int? BatchNo { get; set; }
        public string SalCode { get; set; }
        public string OperatorCode { get; set; }
        public System.DateTime? Date { get; set; }
        public string CounterCode { get; set; }
        public string GSCode { get; set; }
        public string ItemName { get; set; }
        public decimal? Gwt { get; set; }
        public decimal? Swt { get; set; }
        public decimal? Nwt { get; set; }
        public string Grade { get; set; }
        public string CatalogID { get; set; }
        public decimal? MakingChargePerRs { get; set; }
        public decimal? WastPercent { get; set; }
        public int? Qty { get; set; }
        public string ItemSize { get; set; }
        public string DesignNo { get; set; }
        public decimal? PieceRate { get; set; }
        public decimal? DaimondAmount { get; set; }
        public decimal? StoneAmount { get; set; }
        public int? OrderNo { get; set; }
        public string SoldFlag { get; set; }
        public string ProductCode { get; set; }
        public decimal? HallmarkCharges { get; set; }
        public string Remarks { get; set; }
        public string SupplierCode { get; set; }
        public string OrderedCompanyCode { get; set; }
        public string OrderedBranchCode { get; set; }
        public string Karat { get; set; }
        public decimal? McAmount { get; set; }
        public decimal? WastageGrams { get; set; }
        public decimal? McPercent { get; set; }
        public string McType { get; set; }
        public string OldBarcodeNo { get; set; }
        public string ProdIda { get; set; }
        public string ProdTagNo { get; set; }
        public System.DateTime? UpdateOn { get; set; }
        public int? LotNo { get; set; }
        public decimal? TagWt { get; set; }
        public string IsConfirmed { get; set; }
        public string ConfirmedBy { get; set; }
        public System.DateTime? ConfirmedDate { get; set; }
        public decimal? CurrentWt { get; set; }
        public string MCFor { get; set; }
        public int? DiamondNo { get; set; }
        public string BatchID { get; set; }
        public decimal? AddWt { get; set; }
        public string WeightRead { get; set; }
        public string ConfirmedWeightRead { get; set; }
        public string PartyName { get; set; }
        public string DesignName { get; set; }
        public string ItemSizeName { get; set; }
        public string MasterDesignCode { get; set; }
        public string MasterDesignName { get; set; }
        public string VendorModelNo { get; set; }
        public decimal? PurMcGram { get; set; }
        public decimal? McPerPiece { get; set; }
        public string TaggingType { get; set; }
        public string BReceiptNo { get; set; }
        public int? BSNo { get; set; }
        public string IssueTo { get; set; }
        public decimal? PurMcAmount { get; set; }
        public int? PurMcType { get; set; }
        public decimal? PurRate { get; set; }
        public string SrBatchId { get; set; }
        public decimal? TotalSellingMc { get; set; }
        public decimal? PurDiamondAmount { get; set; }
        public decimal? TotalPurchaseMc { get; set; }
        public decimal? PurStoneAmount { get; set; }
        public decimal PurPurityPercentage { get; set; }
        public int PurWastageType { get; set; }
        public decimal PurWastageTypeValue { get; set; }
        public string CertificationNo { get; set; }
        public string RefNo { get; set; }
        public string ReceiptType { get; set; }
        public List<BarcodeStoneVM> BarcodeStoneDetails { get; set; }

        public List<BarcodeTransactionDet> BarcodeTransactionDetails { get; set; }
    }

    public class BarcodeStoneVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int SlNo { get; set; }
        public string BarcodeNo { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> Carrat { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Clarity { get; set; }
        public string Color { get; set; }
        public string ProdIDA { get; set; }
        public string ProdTagNo { get; set; }
        public string OldBarcodeNo { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public string StoneType { get; set; }
        public string StoneGSType { get; set; }
        public Nullable<int> Fin_Year { get; set; }
        public string UOM { get; set; }
        public Nullable<decimal> PurCost { get; set; }
        public string StoneCode { get; set; }
        public string Shape { get; set; }
        public string Cut { get; set; }
        public string Polish { get; set; }
        public string Symmetry { get; set; }
        public string Fluorescence { get; set; }
        public string Certificate { get; set; }
        public Nullable<decimal> PurRate { get; set; }
        public string Size { get; set; }
    }

    public class BarcodeTransactionDet
    {
        public DateTime RefDate { get; set; }
        public string RefNo { get; set; }
        public string Description { get; set; }
        public string Staff { get; set; }
    }
}
