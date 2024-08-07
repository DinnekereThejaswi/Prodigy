using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.OldPurchase
{
    public class PurchaseEstDetailsVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int BillNo { get; set; }
        public int EstNo { get; set; }
        public int SlNo { get; set; }
        public string ItemName { get; set; }
        public int ItemNo { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal MeltingPercent { get; set; }
        public Nullable<decimal> MeltingLoss { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal DiamondAmount { get; set; }
        public decimal GoldAmount { get; set; }
        public decimal ItemAmount { get; set; }
        public string SalCode { get; set; }
        public string GSCode { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<decimal> PurityPercent { get; set; }
        public Nullable<decimal> ConvertionWt { get; set; }
        public Nullable<int> FinYear { get; set; }
        public string ItemDescription { get; set; }
        public Nullable<decimal> ItemwiseTaxPercentage { get; set; }
        public Nullable<decimal> ItemwiseTaxAmount { get; set; }
        public Nullable<decimal> ItemwisePurchaseAmount { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<decimal> RateDeduct { get; set; }
        public string GSTGroupCode { get; set; }
        public Nullable<decimal> SGSTPercent { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> CGSTPercent { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> IGSTPercent { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public string HSN { get; set; }
        public Nullable<decimal> Dcts { get; set; }
        public string RefNo { get; set; }
        public List<PurchaseEstStoneDetailsVM> lstPurchaseEstStoneDetailsVM { get; set; }
        public List<PurchaseEstStoneDetailsVM> lstPurchaseEstDiamondDetailsVM { get; set; }
    }

    public class PurchaseBillingDetailsVM
    {
        public string ObjId { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int BillNo { get; set; }
        public int SlNo { get; set; }
        public int EstNo { get; set; }
        public string ItemName { get; set; }
        public int ItemNo { get; set; }
        public decimal Gwt { get; set; }
        public decimal Swt { get; set; }
        public decimal Nwt { get; set; }
        public decimal MeltingPercent { get; set; }
        public Nullable<decimal> MeltingLoss { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal DiamondAmount { get; set; }
        public decimal GoldAmount { get; set; }
        public decimal ItemAmount { get; set; }
        public string SalCode { get; set; }
        public string GsCode { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<decimal> VAAamount { get; set; }
        public Nullable<decimal> PurityPer { get; set; }
        public Nullable<decimal> ConvertionWt { get; set; }
        public string ItemDescription { get; set; }
        public Nullable<int> FinYear { get; set; }
        public Nullable<decimal> ItemwisTaxPercentage { get; set; }
        public Nullable<decimal> ItemwiseTaxAmount { get; set; }
        public Nullable<decimal> ItemwisePurchaseAmount { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<decimal> ItemwiseTotalAmount { get; set; }
        public Nullable<decimal> ItemwiseFinalAmount { get; set; }
        public Nullable<decimal> RateDeduct { get; set; }
        public string GSTGroupCode { get; set; }
        public Nullable<decimal> SGSTPercent { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> CGSTPercent { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> IGSTPercent { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public string HSN { get; set; }
        public System.Guid UniqRowID { get; set; }
    }
}
