using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.OPGProcessing
{
    #region Input object
    public class OPGSeparationInputVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string GSCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string SalesmanCode { get; set; }
        public List<OPGSeparationInputLinesVM> OPGSeparationInputLines { get; set; }
    }

    public class OPGSeparationInputLinesVM
    {
        public string GSCode { get; set; }
        public decimal GrossWeight { get; set; }
        public string StoneGSCode { get; set; }
        public decimal StoneWeight { get; set; }
        public string DiamondGSCode { get; set; }
        public decimal DiamondCaretWeight { get; set; }
        public decimal PurityPercent { get; set; }
        public decimal PureWeight { get; set; }
    }
    #endregion

    #region Output Object
    public class OPGSeparationOutputVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal Dcts { get; set; }
        public decimal ToleranceWt { get; set; }
        public List<OPGSeparationLineVM> LineDetails { get; set; }
    }

    public class OPGSeparationLineVM
    {
        public int BillNo { get; set; }
        public int SlNo { get; set; }
        public string GSCode { get; set; }
        public string ItemCode { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal MeltingPercent { get; set; }
        public decimal MeltingLossWeight { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal DiamondAmount { get; set; }
        public decimal GoldAmount { get; set; }
        public decimal LineAmount { get; set; }
        public List<OPGSeparationStoneDetailVM> StoneDetails { get; set; }
    }

    public class OPGSeparationStoneDetailVM
    {
        public int SlNo { get; set; }
        public string GS { get; set; }
        public string Name { get; set; }
        public int? Qty { get; set; }
        public decimal? Carrat { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
    } 
    #endregion
}
