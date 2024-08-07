using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Sales
{
    public class TagVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BarcodeNo { get; set; }
        public Nullable<int> BatchNo { get; set; }
        public string SalCode { get; set; }
        public string OperatorCode { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string CounterCode { get; set; }
        public string GSCode { get; set; }
        public string ItemName { get; set; }
        public Nullable<decimal> Gwt { get; set; }
        public Nullable<decimal> Swt { get; set; }
        public Nullable<decimal> Nwt { get; set; }
        public string Grade { get; set; }
        public string CatalogID { get; set; }
        public Nullable<decimal> MakingChargePerRs { get; set; }
        public Nullable<decimal> WastePercent { get; set; }
        public Nullable<int> Qty { get; set; }
        public string ItemSize { get; set; }
        public string DesignNo { get; set; }
        public Nullable<decimal> PieceRate { get; set; }
        public Nullable<decimal> DiamondAmount { get; set; }
        public Nullable<decimal> StoneAmount { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public string SoldFlag { get; set; }
        public string ProductCode { get; set; }
        public Nullable<decimal> HallmarkCharges { get; set; }
        public string Remarks { get; set; }
        public string SupplierCode { get; set; }
        public string OrderedCompanyCode { get; set; }
        public string OrderedBranchCode { get; set; }
        public string Karat { get; set; }
        public Nullable<decimal> McAmount { get; set; }
        public Nullable<decimal> WastageGrams { get; set; }
        public Nullable<decimal> McPercent { get; set; }
        public string McType { get; set; }
        public string OldBarcodeNo { get; set; }
        public string ProdIda { get; set; }
        public string ProdTagno { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<int> LotNo { get; set; }
        public Nullable<decimal> TagWt { get; set; }
        public string IsConfirmed { get; set; }
        public string ConfirmedBy { get; set; }
        public Nullable<System.DateTime> ConfirmedDate { get; set; }
        public Nullable<decimal> CurrentWt { get; set; }
        public string MCFor { get; set; }
        public Nullable<int> DiamondNo { get; set; }
        public string BatchId { get; set; }
        public Nullable<decimal> AddWt { get; set; }
        public string WeightRead { get; set; }
        public string ConfirmedweightRead { get; set; }
        public string PartyName { get; set; }
        public string DesignName { get; set; }
        public string ItemSizeName { get; set; }
        public string MasterDesignCode { get; set; }
        public string MasterDesignName { get; set; }
        public string VendorModelNo { get; set; }
        public Nullable<decimal> PurMcGram { get; set; }
        public Nullable<decimal> McPerPiece { get; set; }
        public string TaggingType { get; set; }
        public string BReceiptNo { get; set; }
        public Nullable<int> BSNo { get; set; }
        public string IssueTo { get; set; }
        public Nullable<decimal> PurMcAmount { get; set; }
        public Nullable<int> PurMcType { get; set; }
        public Nullable<decimal> PurRate { get; set; }
        public string SRBatchID { get; set; }
        public Nullable<decimal> TotalSellingMC { get; set; }
        public Nullable<decimal> PurDiamondAmount { get; set; }
        public Nullable<decimal> TotalPurchaseMc { get; set; }
        public Nullable<decimal> PurStoneAmount { get; set; }
        public decimal PurPurityPercentage { get; set; }
        public int PurWastageType { get; set; }
        public decimal PurWastageTypeValue { get; set; }
        public System.Guid UniqRowID { get; set; }
        public string CertificationNo { get; set; }
        public string RefNo { get; set; }
        public string ReceiptType { get; set; }
        public string EntryDocType { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public string EntryDocNo { get; set; }
        public string ExitDocType { get; set; }
        public Nullable<System.DateTime> ExitDate { get; set; }
        public string ExitDocNo { get; set; }
        public string OnlineStock { get; set; }
        public string IsShuffled { get; set; }
        public Nullable<System.DateTime> Shuffled_date { get; set; }
        public string Collections { get; set; }
        public List<SalesEstStoneVM> lstOfStone { get; set; }
    }
}
