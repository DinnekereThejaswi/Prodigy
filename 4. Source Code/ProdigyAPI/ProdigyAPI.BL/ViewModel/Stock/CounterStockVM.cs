using ProdigyAPI.BL.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Stock
{
    public class CounterStockVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ItemName { get; set; }
        public string GSCode { get; set; }
        public DateTime Date { get; set; }
        public int OpeningUnits { get; set; }
        public decimal? OpeningNetWt { get; set; }
        public decimal OpeningGrossWt { get; set; }
        public decimal OpeningSWt { get; set; }
        public int ReceiptUnits { get; set; }
        public decimal ReceiptNetWt { get; set; }
        public decimal? ReceiptGrossWt { get; set; }
        public decimal? ReceiptSWt { get; set; }
        public decimal ClosingNetWt { get; set; }
        public decimal? ClosingSWt { get; set; }
        public decimal? ClosingGrossWt { get; set; }
        public int ClosingUnits { get; set; }
        public DateTime? UpdateOn { get; set; }
        public int BarcodedUnits { get; set; }
        public decimal BarcodedGrossWt { get; set; }
        public decimal? BarcodedSWt { get; set; }
        public decimal? BarcodedNetWt { get; set; }
        public string CounterCode { get; set; }
        public int SalesUnits { get; set; }
        public decimal? SalesSWt { get; set; }
        public decimal SalesNetWt { get; set; }
        public decimal? SalesGrossWt { get; set; }
        public int IssuesUnits { get; set; }
        public decimal IssuesNetWt { get; set; }
        public decimal? IssuesSWt { get; set; }
        public decimal? IssuesGrossWt { get; set; }
        public int? Version { get; set; }
        public System.Guid UniqRowID { get; set; }
        public ItemListGroupVM itemlisgtgrpvm { get; set; }

    }
    public class CounterStockAdjuststmentVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int TranaactionNo { get; set; }
        public string GSCode { get; set; }
        public string CounterCode { get; set; }
        public string ItemCode { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal? StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public string SalCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Remarks { get; set; }

    }
}
