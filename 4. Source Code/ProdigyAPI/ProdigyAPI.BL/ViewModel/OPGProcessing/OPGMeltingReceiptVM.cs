using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.OPGProcessing
{
    #region Input Objects - used in Posting
    public class OPGMeltingReceiptHeaderVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ReceiptFrom { get; set; }
        public int IssueNo { get; set; }
        public string Remarks { get; set; }
        public List<OPGMeltingLineVM> OPGReceiptLines { get; set; }
    }

    public class OPGMeltingLineVM
    {
        public string BatchId { get; set; }
        public string GSCode { get; set; }
        public decimal IssueGrossWt { get; set; }
        public decimal ReceiptGrossWt { get; set; }
        public decimal KovaWeight { get; set; }
        public decimal MeltingLossWeight { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal CalculatedMeltingLoss { get { return IssueGrossWt - ReceiptGrossWt - KovaWeight; } }
        public decimal CalculatedTotalReceiptWeight { get { return ReceiptGrossWt + KovaWeight + MeltingLossWeight; } }
        public decimal CalculatedAmount { get { return IssueGrossWt * Rate; } }
    }
    #endregion

    #region Ouput Object - used to get the details
    public class OPGMeltingReceiptBatchDetailVM
    {
        public int IssueNo { get; set; }
        public string BatchId { get; set; }
        public string GSCode { get; set; }
        public string ItemCode { get; set; }
        public decimal Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal WastageWt { get; set; }
        public decimal PurityPercent { get; set; }
        public decimal AlloyWt { get; set; }
        public decimal Amount { get; set; }
        public decimal Rate { get; set; }
    } 
    #endregion
}
