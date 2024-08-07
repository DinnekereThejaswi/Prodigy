using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProdigyAPI.BL.BusinessLayer.Stock
{
    public class CounterStockBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Methods
        public string GetCounterStockSummaryReport(string companyCode, string branchCode, string gsCode, DateTime? asOnDate, out ErrorVM error)
        {
            error = null;
            try {
                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                applicationDate = applicationDate.Date;

                bool historyData = false;
                if (asOnDate == null)
                    asOnDate = applicationDate;
                else
                    asOnDate = asOnDate.Value.Date;

                if (asOnDate != applicationDate)
                    historyData = true;

                if (historyData == false) {
                    var query = from cs in db.KTTU_COUNTER_STOCK //For present stock refer to KTTU_COUNTER_STOCK.
                                join gs in db.KSTS_GS_ITEM_ENTRY on
                                new { cs.company_code, cs.branch_code, cs.gs_code } equals new { gs.company_code, gs.branch_code, gs.gs_code }
                                where cs.company_code == companyCode && cs.branch_code == branchCode
                                && cs.gs_code == (gsCode.ToUpper() == "ALL" ? cs.gs_code : gsCode)
                                && (
                                   cs.closing_gwt > 0
                                   || cs.op_gwt > 0
                                   || cs.sales_gwt > 0
                                   || cs.receipt_gwt > 0
                                   || cs.issues_gwt > 0
                               )
                                group new { cs } by new { cs.company_code, cs.branch_code, cs.gs_code, gs.item_level1_name } into g
                                select new StockSummaryReport()
                                {
                                    GSCode = g.Key.gs_code,
                                    GSName = g.Key.item_level1_name,
                                    OpeningWt = g.Sum(f => f.cs.op_gwt),
                                    OpeningPcs = g.Sum(f => f.cs.op_units),
                                    IssueWt = g.Sum(f => f.cs.issues_gwt),
                                    ReceiptWt = g.Sum(f => f.cs.receipt_gwt),
                                    SalesWt = g.Sum(f => f.cs.sales_gwt),
                                    ClosingWt = g.Sum(f => f.cs.closing_gwt),
                                    ClosingPcs = g.Sum(f => f.cs.closing_units)
                                };
                    var result = query.ToList();
                    return GenerateDotMatrixReport(result, companyCode, branchCode, applicationDate);
                }
                else {
                    var query = from cs in db.KTTU_COUNTER_STOCK_HIS //For history we will refer to KTTU_COUNTER_STOCK_HIS, rest all query is the same. Pls suggest a good alternative to this.
                                join gs in db.KSTS_GS_ITEM_ENTRY on
                                new { cs.company_code, cs.branch_code, cs.gs_code } equals new { gs.company_code, gs.branch_code, gs.gs_code }
                                where cs.company_code == companyCode && cs.branch_code == branchCode
                                && cs.gs_code == (gsCode.ToUpper() == "ALL" ? cs.gs_code : gsCode)
                                && System.Data.Entity.DbFunctions.TruncateTime(cs.date) == asOnDate
                                && (
                                   cs.closing_gwt > 0
                                   || cs.op_gwt > 0
                                   || cs.sales_gwt > 0
                                   || cs.receipt_gwt > 0
                                   || cs.issues_gwt > 0
                               )
                                group new { cs } by new { cs.company_code, cs.branch_code, cs.gs_code, gs.item_level1_name } into g
                                select new StockSummaryReport
                                {
                                    GSCode = g.Key.gs_code,
                                    GSName = g.Key.item_level1_name,
                                    OpeningWt = g.Sum(f => f.cs.op_gwt),
                                    OpeningPcs = g.Sum(f => f.cs.op_units),
                                    IssueWt = g.Sum(f => f.cs.issues_gwt),
                                    ReceiptWt = g.Sum(f => f.cs.receipt_gwt),
                                    SalesWt = g.Sum(f => f.cs.sales_gwt),
                                    ClosingWt = g.Sum(f => f.cs.closing_gwt),
                                    ClosingPcs = g.Sum(f => f.cs.closing_units)
                                };
                    return GenerateDotMatrixReport(query.ToList(), companyCode, branchCode, applicationDate);
                }
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public string GetStockDetailedReport(string companyCode, string branchCode, string gsCode, string counterCode, DateTime? asOnDate, out ErrorVM error)
        {
            error = null;
            try {
                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                applicationDate = applicationDate.Date;

                bool historyData = false;
                if (asOnDate == null)
                    asOnDate = applicationDate;
                else
                    asOnDate = asOnDate.Value.Date;

                if (asOnDate != applicationDate)
                    historyData = true;

                if(historyData == false) {
                    var stockInfo = from cs in db.KTTU_COUNTER_STOCK
                                    join im in db.ITEM_MASTER
                                    on new { CD = cs.company_code, BC = cs.branch_code, GS = cs.gs_code, IC = cs.item_name }
                                        equals new { CD = im.company_code, BC = im.branch_code, GS = im.gs_code, IC = im.Item_code } into leftOuterJoin
                                    from im in leftOuterJoin.DefaultIfEmpty()
                                    where cs.company_code == companyCode
                                    && cs.branch_code == branchCode
                                    && System.Data.Entity.DbFunctions.TruncateTime(cs.date) == asOnDate
                                    && cs.gs_code == (gsCode.ToUpper() == "ALL" ? cs.gs_code : gsCode)
                                    && cs.counter_code == (counterCode.ToUpper() == "ALL" ? cs.counter_code : counterCode)
                                    select new StockDetaildReport {
                                        GS = cs.gs_code,
                                        ItemName = im.Item_Name,
                                        OpnQty = cs.op_units,
                                        OpnGwt = cs.op_gwt,
                                        BarQty = cs.barcoded_units,
                                        BarGwt = cs.barcoded_gwt,
                                        SalQty = cs.sales_units,
                                        SalGwt = cs.sales_gwt,
                                        RecQty = cs.receipt_units,
                                        RecGwt = cs.receipt_gwt,
                                        IssQty = cs.issues_units,
                                        IssGwt = cs.issues_gwt,
                                        ClsQty = cs.closing_units,
                                        ClsGwt = cs.closing_gwt,
                                        Cntr = cs.counter_code
                                    };

                    return GenerateDotMatrixReport(stockInfo.ToList(), companyCode, branchCode, applicationDate);
                }
                else {
                    var stockInfo = from cs in db.KTTU_COUNTER_STOCK_HIS
                                    join im in db.ITEM_MASTER
                                    on new { CD = cs.company_code, BC = cs.branch_code, GS = cs.gs_code, IC = cs.item_name }
                                        equals new { CD = im.company_code, BC = im.branch_code, GS = im.gs_code, IC = im.Item_code } into leftOuterJoin
                                    from im in leftOuterJoin.DefaultIfEmpty()
                                    where cs.company_code == companyCode
                                    && cs.branch_code == branchCode
                                    && System.Data.Entity.DbFunctions.TruncateTime(cs.date) == asOnDate
                                    && cs.gs_code == (gsCode.ToUpper() == "ALL" ? cs.gs_code : gsCode)
                                    && cs.counter_code == (counterCode.ToUpper() == "ALL" ? cs.counter_code : counterCode)
                                    select new StockDetaildReport {
                                        GS = cs.gs_code,
                                        ItemCode = cs.item_name,
                                        ItemName = im.Item_Name,
                                        OpnQty = cs.op_units,
                                        OpnGwt = cs.op_gwt,
                                        BarQty = cs.barcoded_units,
                                        BarGwt = cs.barcoded_gwt,
                                        SalQty = cs.sales_units,
                                        SalGwt = cs.sales_gwt,
                                        RecQty = cs.receipt_units,
                                        RecGwt = cs.receipt_gwt,
                                        IssQty = cs.issues_units,
                                        IssGwt = cs.issues_gwt,
                                        ClsQty = cs.closing_units,
                                        ClsGwt = cs.closing_gwt,
                                        Cntr = cs.counter_code
                                    };

                    return GenerateDotMatrixReport(stockInfo.ToList(), companyCode, branchCode, applicationDate);

                }
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public string GetClosingStockReport(string companyCode, string branchCode, string gsCode, string counterCode, DateTime? asOnDate, out ErrorVM error)
        {
            error = null;
            try {
                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                applicationDate = applicationDate.Date;

                bool historyData = false;
                if (asOnDate == null)
                    asOnDate = applicationDate;
                else
                    asOnDate = asOnDate.Value.Date;

                if (asOnDate != applicationDate)
                    historyData = true;

                if (historyData == false) {
                    var totalStock = from cs in db.KTTU_COUNTER_STOCK
                                     where cs.company_code == companyCode && cs.branch_code == branchCode
                                     && cs.gs_code == (gsCode.ToUpper() == "ALL" ? cs.gs_code : gsCode)
                                     && cs.counter_code == (counterCode.ToUpper() == "ALL" ? cs.counter_code : counterCode)
                                     group new { cs } by new { cs.company_code, cs.branch_code, cs.gs_code } into g
                                     select new ClosingStockReport
                                     {
                                         CompanyCode = g.Key.company_code,
                                         BranchCode = g.Key.branch_code,
                                         GS = g.Key.gs_code,
                                         TotalGrossWt = g.Sum(x => x.cs.closing_gwt),
                                         TotalQty = g.Sum(x => x.cs.closing_units),
                                         TotalNetWt = g.Sum(x => x.cs.closing_nwt)
                                     };
                    var query = from cs in db.KTTU_COUNTER_STOCK //For present stock refer to KTTU_COUNTER_STOCK.
                                join gs in totalStock on
                                new { CC = cs.company_code, BC = cs.branch_code, GC = cs.gs_code } equals
                                new { CC = gs.CompanyCode, BC = gs.BranchCode, GC = gs.GS }
                                where cs.company_code == companyCode && cs.branch_code == branchCode
                                && cs.gs_code == (gsCode.ToUpper() == "ALL" ? cs.gs_code : gsCode)
                                && cs.counter_code == (counterCode.ToUpper() == "ALL" ? cs.counter_code : counterCode)
                                && (cs.closing_gwt > 0 || cs.closing_units > 0)
                                select new ClosingStockReportHistory
                                {
                                    GSCode = cs.gs_code,
                                    ItemCode = cs.item_name,
                                    Counter = cs.counter_code,
                                    ClosingGrossWt = cs.closing_gwt,
                                    ClosingNetWt = cs.closing_nwt,
                                    ClosingUnit = cs.closing_units,
                                    GrossWtPercentage = gs.TotalGrossWt > 0 ? cs.closing_gwt / gs.TotalGrossWt * 100 : 0,
                                    NetWtPercentage = gs.TotalNetWt > 0 ? cs.closing_nwt / gs.TotalNetWt * 100 : 0
                                };
                    return GenerateDotMatrixReport(query.ToList(), companyCode, branchCode, counterCode, applicationDate);
                    //return query.ToList();
                }
                else {
                    var totalStock = from cs in db.KTTU_COUNTER_STOCK_HIS
                                     where cs.company_code == companyCode && cs.branch_code == branchCode
                                     && cs.gs_code == (gsCode.ToUpper() == "ALL" ? cs.gs_code : gsCode)
                                     && cs.counter_code == (counterCode.ToUpper() == "ALL" ? cs.counter_code : counterCode)
                                     && System.Data.Entity.DbFunctions.TruncateTime(cs.date) == asOnDate
                                     group new { cs } by new { cs.company_code, cs.branch_code, cs.gs_code } into g
                                     select new ClosingStockReport
                                     {
                                         CompanyCode = g.Key.company_code,
                                         BranchCode = g.Key.branch_code,
                                         GS = g.Key.gs_code,
                                         TotalGrossWt = g.Sum(x => x.cs.closing_gwt),
                                         TotalQty = g.Sum(x => x.cs.closing_units),
                                         TotalNetWt = g.Sum(x => x.cs.closing_nwt)
                                     };
                    var query = from cs in db.KTTU_COUNTER_STOCK_HIS //For present stock refer to KTTU_COUNTER_STOCK_HIS.
                                join gs in totalStock on
                                new { CC = cs.company_code, BC = cs.branch_code, GC = cs.gs_code } equals
                                new { CC = gs.CompanyCode, BC = gs.BranchCode, GC = gs.GS }
                                where cs.company_code == companyCode && cs.branch_code == branchCode
                                && cs.gs_code == (gsCode.ToUpper() == "ALL" ? cs.gs_code : gsCode)
                                && cs.counter_code == (counterCode.ToUpper() == "ALL" ? cs.counter_code : counterCode)
                                && System.Data.Entity.DbFunctions.TruncateTime(cs.date) == asOnDate
                                && (cs.closing_gwt > 0 || cs.closing_units > 0)
                                select new ClosingStockReportHistory
                                {
                                    GSCode = cs.gs_code,
                                    ItemCode = cs.item_name,
                                    Counter = cs.counter_code,
                                    ClosingGrossWt = cs.closing_gwt,
                                    ClosingNetWt = cs.closing_nwt,
                                    ClosingUnit = cs.closing_units,
                                    GrossWtPercentage = gs.TotalGrossWt > 0 ? cs.closing_gwt / gs.TotalGrossWt * 100 : 0,
                                    NetWtPercentage = gs.TotalNetWt > 0 ? cs.closing_nwt / gs.TotalNetWt * 100 : 0
                                };
                    return GenerateDotMatrixReport(query.ToList(), companyCode, branchCode, counterCode, applicationDate);
                    //return query.ToList();
                }
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetCounterSummaryReport(string companyCode, string branchCode, string gsCode, string counterCode, DateTime? asOnDate, out ErrorVM error)
        {
            error = null;
            List<CounterSummaryReport> lstOfStockSummaryReport = new List<CounterSummaryReport>();

            try {
                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                applicationDate = applicationDate.Date;

                bool historyData = false;
                if (asOnDate == null)
                    asOnDate = applicationDate;
                else
                    asOnDate = asOnDate.Value.Date;

                if (asOnDate != applicationDate)
                    historyData = true;

                if (historyData == false) {
                    var query = from cs in db.KTTU_COUNTER_STOCK //For present stock refer to KTTU_COUNTER_STOCK.
                                join gs in db.KSTS_GS_ITEM_ENTRY on
                                new { cs.company_code, cs.branch_code, cs.gs_code } equals new { gs.company_code, gs.branch_code, gs.gs_code }
                                join ct in db.KSTU_COUNTER_MASTER on
                                new { cs.company_code, cs.branch_code, cs.counter_code } equals new { ct.company_code, ct.branch_code, ct.counter_code }
                                where cs.company_code == companyCode && cs.branch_code == branchCode
                                && cs.gs_code == (gsCode.ToUpper() == "ALL" ? cs.gs_code : gsCode)
                                && cs.counter_code == (counterCode.ToUpper() == "ALL" ? cs.counter_code : counterCode)
                                && (
                                   cs.op_gwt > 0
                                   || cs.op_units > 0
                                   //|| cs.sales_gwt > 0
                                   //|| cs.receipt_gwt > 0
                                   //|| cs.issues_gwt > 0
                                   || cs.closing_gwt > 0
                                   || cs.closing_units > 0
                                )
                                group new { cs } by new { cs.company_code, cs.branch_code, cs.gs_code, gs.item_level1_name, cs.counter_code, ct.counter_name } into g
                                select new CounterSummaryReport
                                {
                                    GSCode = g.Key.gs_code,
                                    GSName = g.Key.item_level1_name,
                                    CounterCode = g.Key.counter_code,
                                    CounterName = g.Key.counter_name,

                                    OpeningUnits = g.Sum(f => f.cs.op_units),
                                    OpeningGWt = g.Sum(f => f.cs.op_gwt),
                                    OpeningSwt = g.Sum(f => f.cs.op_swt),
                                    OpeningNwt = g.Sum(f => f.cs.op_nwt),

                                    ReceiptUnits = g.Sum(f => f.cs.receipt_units),
                                    ReceiptGWt = g.Sum(f => f.cs.receipt_gwt),
                                    ReceiptSwt = g.Sum(f => f.cs.receipt_swt),
                                    ReceiptNwt = g.Sum(f => f.cs.receipt_nwt),

                                    IssueUnits = g.Sum(f => f.cs.issues_units),
                                    IssueGwt = g.Sum(f => f.cs.issues_gwt),
                                    IssueSwt = g.Sum(f => f.cs.issues_swt),
                                    IssueNwt = g.Sum(f => f.cs.issues_nwt),

                                    ClosingUnits = g.Sum(f => f.cs.closing_units),
                                    ClosingGwt = g.Sum(f => f.cs.closing_gwt),
                                    ClosingNwt = g.Sum(f => f.cs.closing_nwt)
                                };
                    lstOfStockSummaryReport = query.ToList();
                    return GenerateGetCounterSummaryHTMLReport(lstOfStockSummaryReport, companyCode, branchCode, counterCode, applicationDate);
                }
                else {
                    var query = from cs in db.KTTU_COUNTER_STOCK_HIS //For history we will refer to KTTU_COUNTER_STOCK_HIS, rest all query is the same. Pls suggest a good alternative to this.
                                join gs in db.KSTS_GS_ITEM_ENTRY on
                                new { cs.company_code, cs.branch_code, cs.gs_code } equals new { gs.company_code, gs.branch_code, gs.gs_code }
                                join ct in db.KSTU_COUNTER_MASTER on
                                new { cs.company_code, cs.branch_code, cs.counter_code } equals new { ct.company_code, ct.branch_code, ct.counter_code }
                                where cs.company_code == companyCode && cs.branch_code == branchCode
                                && cs.gs_code == (gsCode.ToUpper() == "ALL" ? cs.gs_code : gsCode)
                                && cs.counter_code == (counterCode.ToUpper() == "ALL" ? cs.counter_code : counterCode)
                                && System.Data.Entity.DbFunctions.TruncateTime(cs.date) == asOnDate
                                && (
                                   cs.op_gwt > 0
                                   || cs.op_units > 0
                                   //|| cs.sales_gwt > 0
                                   //|| cs.receipt_gwt > 0
                                   //|| cs.issues_gwt > 0
                                   || cs.closing_gwt > 0
                                   || cs.closing_units > 0
                                )
                                group new { cs } by new { cs.company_code, cs.branch_code, cs.gs_code, gs.item_level1_name, cs.counter_code, ct.counter_name } into g
                                select new CounterSummaryReport
                                {
                                    GSCode = g.Key.gs_code,
                                    GSName = g.Key.item_level1_name,
                                    CounterCode = g.Key.counter_code,
                                    CounterName = g.Key.counter_name,

                                    OpeningUnits = g.Sum(f => f.cs.op_units),
                                    OpeningGWt = g.Sum(f => f.cs.op_gwt),
                                    OpeningSwt = g.Sum(f => f.cs.op_swt),
                                    OpeningNwt = g.Sum(f => f.cs.op_nwt),

                                    ReceiptUnits = g.Sum(f => f.cs.receipt_units),
                                    ReceiptGWt = g.Sum(f => f.cs.receipt_gwt),
                                    ReceiptSwt = g.Sum(f => f.cs.receipt_swt),
                                    ReceiptNwt = g.Sum(f => f.cs.receipt_nwt),

                                    IssueUnits = g.Sum(f => f.cs.issues_units),
                                    IssueGwt = g.Sum(f => f.cs.issues_gwt),
                                    IssueSwt = g.Sum(f => f.cs.issues_swt),
                                    IssueNwt = g.Sum(f => f.cs.issues_nwt),

                                    ClosingUnits = g.Sum(f => f.cs.closing_units),
                                    ClosingGwt = g.Sum(f => f.cs.closing_gwt),
                                    ClosingSwt = g.Sum(f => f.cs.closing_gwt),
                                    ClosingNwt = g.Sum(f => f.cs.closing_nwt)
                                };
                    lstOfStockSummaryReport = query.ToList();
                    return GenerateGetCounterSummaryHTMLReport(lstOfStockSummaryReport, companyCode, branchCode, counterCode, applicationDate);
                }
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }
        #endregion

        #region Private Methods
        private string GenerateDotMatrixReport(List<StockSummaryReport> result, string compayCode, string branchCode, DateTime applicationDate)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder strLine = new StringBuilder();
            strLine.Append('-', 80);

            sb.Append(Convert.ToChar(18).ToString());
            sb.AppendLine(string.Format("{0,30}{1,0}{2,10} ", branchCode, "-", "Counter Stock Summary Report"));
            sb.AppendLine(string.Format("Date:{0,-20}{1,55}", applicationDate.ToString("dd-MM-yyyy"), "Time:" + DateTime.Now.TimeOfDay));
            sb.AppendLine(string.Format("{0}{1}", 0, strLine.ToString()));
            int count = result.Count() - 1;
            string itemName = string.Empty;
            sb.AppendLine(string.Format("{0,-20}{1,-15}{2,-10}{3,-10}{4,1}{5,9}{6,15}",
                   "GS Name", "Opn.Gwt", "Sale Gwt", "Exc.Gwt", "", "Short Gwt", "Cls.Gwt"));
            sb.AppendLine(string.Format("{0}{1}", 0, strLine.ToString()));
            for (int i = 0; i < result.Count(); i++) {
                sb.Append(string.Format("{0,-20}", result[i].GSName));
                sb.Append(string.Format("{0,-15}", result[i].OpeningWt));
                sb.Append(string.Format("{0,-10}", result[i].SalesWt));
                sb.Append(string.Format("{0,-10}", result[i].ReceiptWt));
                sb.Append(string.Format("{0,1}", ""));
                sb.Append(string.Format("{0,9}", result[i].IssueWt));
                sb.Append(string.Format("{0,15}", result[i].ClosingWt));
                sb.AppendLine();
            }
            sb.AppendLine(string.Format("{0}{1}", 0, strLine.ToString()));
            return sb.ToString();
        }

        private string GenerateDotMatrixReport(List<StockDetaildReport> result, string companyCode, string branchCode, DateTime applicationDate)
        {
            DataTable tblData = SIGlobals.Globals.ConvertListToDataTable<StockDetaildReport>(result);

            int tempWidth = 160;
            int pageNo = 1;

            StringBuilder strLine = new StringBuilder();
            strLine.Append('-', tempWidth);
            StringBuilder strTransLine = new StringBuilder();
            strTransLine.Append('-', tempWidth);
            decimal Spaces;//= 0M;
            StringBuilder strSpaces = new StringBuilder();
            Spaces = ((tempWidth - strTransLine.Length) / 3);
            strSpaces.Append(' ', Convert.ToInt32(Spaces));

            string esc = Convert.ToChar(27).ToString();
            string tempString = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append(esc + Convert.ToChar(64).ToString());
            sb.Append(esc + Convert.ToChar(15).ToString());
            sb.Append(GetHeader(tempWidth, companyCode, branchCode));
            sb.AppendLine();


            sb.Append(esc + Convert.ToChar(77).ToString());
            sb.AppendLine(string.Format("{0,60}{1,0}{2,10} ", branchCode, "-", "Counter Stock Details Report"));
            sb.AppendLine(string.Format("{0,152}{1}", "Page No:", pageNo));
            sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
            sb.AppendLine(string.Format("{0,-4}{1,-8}{2,-21}{3,10}{4,11}{5,10}{6,11}{7,10}{8,11}{9,10}{10,11}{11,10}{12,11}{13,10}{14,11}"
                , "SNo", "Gs Code", "Item", "Opn.Qty", "Opn.Gwt", "Bar.Qty", "Bar.Gwt", "Sal.Qty", "Sal.Gwt", "Rec.Qty", "Rec.Gwt", "Isu.Qty", "Iss.Gwt", "Cls.Qty", "Cls.Gwt"));
            sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

            int p = 0;
            int count = result.Count - 1;
            string itemName = string.Empty;
            List<string> lstItemName = null;

            for (int i = 0; i < result.Count; i++) {
                sb.Append(string.Format("{0,-4}", i + 1));
                sb.Append(string.Format("{0,-8}", result[i].GS.ToString()));

                itemName = result[i].ItemName == null ? "" : result[i].ItemName;
                if (itemName.Length <= 20)
                    sb.Append(string.Format("{0,-21}", itemName));
                else {
                    lstItemName = SIGlobals.Globals.SplitStringAt(20, itemName);
                    sb.Append(string.Format("{0,-21}", lstItemName[0]));
                }
                sb.Append(string.Format("{0,10}", result[i].OpnQty.ToString()));
                sb.Append(string.Format("{0,11}", result[i].OpnGwt.ToString()));
                sb.Append(string.Format("{0,10}", result[i].BarQty.ToString()));
                sb.Append(string.Format("{0,11}", result[i].BarGwt.ToString()));
                sb.Append(string.Format("{0,10}", result[i].SalQty.ToString()));
                sb.Append(string.Format("{0,11}", result[i].SalGwt.ToString()));
                sb.Append(string.Format("{0,10}", result[i].RecQty.ToString()));
                sb.Append(string.Format("{0,11}", result[i].RecGwt.ToString()));
                sb.Append(string.Format("{0,10}", result[i].IssQty.ToString()));
                sb.Append(string.Format("{0,11}", result[i].IssGwt.ToString()));
                sb.Append(string.Format("{0,10}", result[i].ClsQty.ToString()));
                sb.Append(string.Format("{0,11}", result[i].ClsGwt.ToString()));
                sb.AppendLine();
                if (lstItemName != null && lstItemName.Count > 0) {
                    for (int x = 1; x < lstItemName.Count; x++)
                        sb.AppendLine(string.Format("{0,-12}-{1,-20}", "", lstItemName[x]));
                    itemName = string.Empty;
                    lstItemName = null;
                }
                if (p > 55) {
                    ++pageNo;
                    sb.Append(strLine);
                    sb.AppendLine();
                    sb.AppendLine(string.Format("{0,148}", "Continued..."));
                    sb.Append(Convert.ToChar(12).ToString());
                    sb.AppendLine();
                    sb.AppendLine(string.Format("{0,152}{1}", "Page No:", pageNo));
                    sb.AppendLine();
                    sb.Append(string.Format("{0}{1}", strSpaces, strTransLine));
                    sb.AppendLine();
                    sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                    sb.AppendLine(string.Format("{0,-4}{1,-8}{2,-21}{3,10}{4,11}{5,10}{6,11}{7,10}{8,11}{9,10}{10,11}{11,10}{12,11}{13,10}{14,11}", "SNo", "Gs Code", "Item", "Opn.Qty", "Opn.Gwt", "Bar.Qty", "Bar.Gwt", "Sal.Qty", "Sal.Gwt", "Rec.Qty", "Rec.Gwt", "Isu.Qty", "Iss.Gwt", "Cls.Qty", "Cls.Gwt"));
                    sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                    p = 0;
                }
                p++;
            }
            sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
            sb.AppendLine(string.Format("{0,-4}{1,-9}{2,-20}{3,10}{4,11}{5,10}{6,11}{7,10}{8,11}{9,10}{10,11}{11,10}{12,11}{13,10}{14,11}",
             "", "", "Total",
                result.Sum(s => s.OpnQty).ToString(), result.Sum(s => s.OpnGwt).ToString(),
            result.Sum(s => s.BarQty).ToString(), result.Sum(s => s.BarGwt).ToString(),
             result.Sum(s => s.SalQty).ToString(), result.Sum(s => s.SalGwt).ToString(),
             result.Sum(s => s.RecQty).ToString(), result.Sum(s => s.RecGwt).ToString(),
             result.Sum(s => s.IssQty).ToString(), result.Sum(s => s.IssGwt).ToString(),
             result.Sum(s => s.ClsQty).ToString(), result.Sum(s => s.ClsGwt).ToString()));

            sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
            sb.AppendLine(string.Format("{0,143}{1}", "", "End of the Report"));
            sb.AppendLine(Convert.ToChar(12).ToString());
            sb.Append(esc + Convert.ToChar(64).ToString());
            return sb.ToString();
        }

        private string GenerateDotMatrixReport(List<ClosingStockReportHistory> result, string companyCode, string branchCode, string counterCode, DateTime applicationDate)
        {
            int width = 60;
            StringBuilder strLine = new StringBuilder();
            strLine.Append('-', width);
            StringBuilder strTransLine = new StringBuilder();
            strTransLine.Append('-', width);
            decimal Spaces = 0M;
            StringBuilder strSpaces = new StringBuilder();
            Spaces = ((width - strTransLine.Length) / 3);
            strSpaces.Append(' ', Convert.ToInt32(Spaces));

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0}", GetHeader(width, companyCode, branchCode)));
            DateTime StartDate = SIGlobals.Globals.GetApplicationDate();
            sb.AppendLine(string.Format("Item Stock as On {0}", StartDate.ToString("dd/MM/yyyy")));

            #region Print GS Wise
            sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

            sb.AppendLine(string.Format("{0,-30}{1,12}{2,9}{3,8}", "Item", "Cls.Gwt", "Qty", "Stk.%"));
            sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

            string counter = string.Empty;
            string GS = string.Empty;
            decimal subTotalGwt = 0;
            int subTotalUnits = 0;
            DataTable dtGStype = new DataTable();
            for (int i = 0; i < result.Count; i++) {
                if (string.Compare(counter, result[i].Counter.ToString()) != 0) {
                    if (!string.IsNullOrEmpty(counter) || string.Equals(counter, " ") || string.Equals(counter, "NULL")) {
                        sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                        sb.Append(string.Format("{0,-30}", counter));
                        sb.Append(string.Format("{0,12}", subTotalGwt));
                        sb.Append(string.Format("{0,8}", subTotalUnits));
                        sb.AppendLine();
                        sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                        sb.AppendLine();
                        subTotalGwt = 0; subTotalUnits = 0;
                    }
                    counter = result[i].Counter.ToString();
                    GS = result[i].GSCode.ToString();
                }
                string ItemName = string.Empty;
                List<string> lstItemName = null;
                ItemName = result[i].ItemCode.ToString();
                string[] item = ItemName.Split('-');
                ItemName = item[0];
                if (ItemName.Length <= 30)
                    sb.Append(string.Format("{0,-30}", ItemName));
                else {
                    lstItemName = SIGlobals.Globals.SplitStringAt(30, ItemName);
                    sb.Append(string.Format("{0,-30}", lstItemName[0]));
                }
                sb.Append(string.Format("{0,12}", result[i].ClosingGrossWt.ToString()));
                sb.Append(string.Format("{0,8}", result[i].ClosingUnit.ToString()));
                sb.Append(string.Format("{0,10}", result[i].NetWtPercentage.ToString()));
                sb.AppendLine();

                if (string.Compare(counterCode == null ? "ALL" : counterCode, "ALL") != 0) {
                    //object tempGwt = result.Compute("sum(closing_gwt)", string.Format("Counter_Code = '{0}'  ", counter));
                    object tempGwt = result.Where(r => r.Counter == counter).Sum(r => r.ClosingGrossWt);
                    if (tempGwt != null && tempGwt != DBNull.Value)
                        subTotalGwt = Math.Round(Convert.ToDecimal(tempGwt), 2);

                    //object tempQty = dtCounterReport.Compute("sum(closing_units)", string.Format("Counter_Code = '{0}'  ", counter));
                    object tempQty = result.Where(r => r.Counter == counter).Sum(r => r.ClosingUnit);
                    if (tempQty != null && tempQty != DBNull.Value)
                        subTotalUnits = Convert.ToInt32(tempQty);
                }
                else {
                    //object tempGwt = dtCounterReport.Compute("sum(closing_gwt)", string.Format("Counter_Code = '{0}' and gs_code='{1}' ", counter, GS));
                    object tempGwt = result.Where(r => r.Counter == counter && r.GSCode == GS).Sum(r => r.ClosingGrossWt);
                    if (tempGwt != null && tempGwt != DBNull.Value)
                        subTotalGwt = Convert.ToDecimal(tempGwt);

                    //object tempQty = dtCounterReport.Compute("sum(closing_units)", string.Format("Counter_Code = '{0}' and gs_code='{1}' ", counter, GS));
                    object tempQty = result.Where(r => r.Counter == counter && r.GSCode == GS).Sum(r => r.ClosingUnit);
                    if (tempQty != null && tempQty != DBNull.Value)
                        subTotalUnits = Convert.ToInt32(tempQty);
                }
                if (lstItemName != null && lstItemName.Count > 1) {
                    for (int x = 1; x < lstItemName.Count; x++)
                        sb.AppendLine(string.Format("{0,30}-{1,-10}", "", lstItemName[x].Trim()));
                }
            }
            if (result.Count >= 1) {
                //  sb.AppendLine();
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.Append(string.Format("{0,-30}", counter));
                sb.Append(string.Format("{0,12}", subTotalGwt));
                sb.Append(string.Format("{0,8}", subTotalUnits));
                sb.AppendLine();
                sb.AppendLine();
                subTotalGwt = 0; subTotalUnits = 0;

            }
            //Query = string.Format("EXEC [usp_CounterStockDetails] '{0}','{1}','{2}','{3}','{4}','{5}'", gs_code, "", CGlobals.BranchCode, CGlobals.CompanyCode, "C", "T");
            //DataTable dtGroup = CGlobals.GetDataTable(Query);
            //sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
            //foreach (DataRow dr in dtGroup.Rows) {
            //    object objGwt = dtCounterReport.Compute("SUM(closing_gwt)", string.Format("gs_code='{0}'", dr["gs_code"]));
            //    object objUnit = dtCounterReport.Compute("SUM(closing_units)", string.Format("gs_code='{0}'", dr["gs_code"]));
            //    sb.AppendLine(string.Format("{0,-30}{1,10}{2,10}", CGlobals.GetGsName(dr["gs_code"].ToString()), objGwt, objUnit));
            //}
            //sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

            string esc = Convert.ToChar(27).ToString();
            sb.Append(esc + Convert.ToChar(100).ToString() + Convert.ToChar(9).ToString());
            sb.Append(esc + Convert.ToChar(105).ToString());
            #endregion
            return sb.ToString();
        }

        private string GenerateGetCounterSummaryHTMLReport(List<CounterSummaryReport> result, string companyCode, string branchCode, string counterCode, DateTime applicationDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<HTML>");
            sb.AppendLine("<HEAD>");
            sb.AppendLine(GetStyle());
            sb.AppendLine("</HEAD>");
            sb.AppendLine("<BODY>");

            DateTime StartDate = SIGlobals.Globals.GetApplicationDate();

            sb.AppendLine("<table bgcolor=\"#FDFDF0\" class=\"boldText\" frame=\"border\" border=\"2\" width=\"900\" align=\"LEFT\" >");  //FRAME=BOX RULES=NONE
            sb.AppendLine("<TR height=\"3px\">");
            sb.AppendLine(string.Format("<th  class=\"noborder\" colspan=23 align=\"CENTER\" bgcolor=\"EBE7C6\" ><b style=\"color:'maroon'\" \"height:'100px'\">{0}</b></th>", SIGlobals.Globals.FillCompanyDetails("Company_Name", companyCode, branchCode)));
            sb.AppendLine("</TR>");

            sb.AppendLine("<TR>");
            sb.AppendLine(string.Format("<TD class=\"noborder\" colspan=23 align=\"CENTER\" ><b> {0}  {1}</b> </TD>", "Counter Stock Summary On ", StartDate.ToString("dd/MM/yyyy")));
            sb.AppendLine("</TR>");

            sb.AppendLine("<tr bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\" class = \"fullborderStyle\">");
            sb.AppendLine("<td colspan=1 align=\"LEFT\" ><b>Sales Counter </b></td>");
            sb.AppendLine("<td colspan=3 align=\"CENTRE\" ><b><----- OPENING -----></b></td>");
            sb.AppendLine("<td colspan=3 align=\"CENTRE\" ><b><----- (+) -----></b></td>");
            sb.AppendLine("<td colspan=3 align=\"CENTRE\" ><b><----- (-) -----></b></td>");
            sb.AppendLine("<td colspan=3 align=\"CENTRE\" ><b><----- CLOSING -----></b></td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\" class = \"fullborderStyle\">");
            sb.AppendLine("<td align=\"LEFT\" ><b> </b></td>");
            sb.AppendLine("<td align=\"CENTRE\" ><b>Pcs.</b></td>");
            sb.AppendLine("<td align=\"CENTRE\" ><b>Wt.</b></td>");
            sb.AppendLine("<td align=\"CENTRE\" ><b>NWt.</b></td>");

            sb.AppendLine("<td align=\"CENTRE\" ><b>Pcs.</b></td>");
            sb.AppendLine("<td align=\"CENTRE\" ><b>Wt.</b></td>");
            sb.AppendLine("<td align=\"CENTRE\" ><b>NWt.</b></td>");

            sb.AppendLine("<td align=\"CENTRE\" ><b>Pcs.</b></td>");
            sb.AppendLine("<td align=\"CENTRE\" ><b>Wt.</b></td>");
            sb.AppendLine("<td align=\"CENTRE\" ><b>NWt.</b></td>");

            sb.AppendLine("<td align=\"CENTRE\" ><b>Pcs.</b></td>");
            sb.AppendLine("<td align=\"CENTRE\" ><b>Wt.</b></td>");
            sb.AppendLine("<td align=\"CENTRE\" ><b>NWt.</b></td>");
            sb.AppendLine("</tr>");

            decimal ONetWt = 0, INetWt = 0, RNetWt = 0, CNetWt = 0, OGwt = 0, IGwt = 0, RGwt = 0, CGwt = 0;
            int OQty = 0, IQty = 0, RQty = 0, CQty = 0;

            var resultByGs = result.Select(e => e.GSCode).Distinct().ToList<string>();
            for (int s = 0; s < resultByGs.Count; s++) {

                string gsCode = resultByGs[s].ToString();
                var gsName = result.Where(gs => gs.GSCode == gsCode).FirstOrDefault();
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD bgcolor='#E4ECC7' style=\"color:black; class=\"noborder\" colspan=23 align=\"LEFT\" ><b>GS : {0}</b></TD>", gsName.GSName));
                sb.AppendLine("</TR>");

                string gsCodeFor = resultByGs[s].ToString();
                var resultSet = result.Where(r => r.GSCode == gsCodeFor).ToList();

                for (int i = 0; i < resultSet.Count; i++) {
                    if (Convert.ToInt32(resultSet[i].OpeningUnits) > 0 || Convert.ToDecimal(resultSet[i].OpeningGWt) > 0 || Convert.ToInt32(resultSet[i].ClosingUnits) > 0 || Convert.ToDecimal(resultSet[i].ClosingGwt) > 0) {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD ALIGN = \"LEFT\">{0}</TD>", resultSet[i].CounterName.Trim()));

                        sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", resultSet[i].OpeningUnits.ToString().Trim()));
                        sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", resultSet[i].OpeningGWt.ToString().Trim()));
                        sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", resultSet[i].OpeningNwt.ToString().Trim()));

                        if (Convert.ToInt32(resultSet[i].ReceiptUnits) > 0) {
                            sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", resultSet[i].ReceiptUnits.ToString().Trim()));
                        }
                        else {
                            sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", " "));
                        }

                        if (Convert.ToInt32(resultSet[i].ReceiptGWt) > 0) {
                            sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", resultSet[i].ReceiptGWt.ToString().Trim()));
                        }
                        else {
                            sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", " "));
                        }

                        if (Convert.ToInt32(resultSet[i].ReceiptNwt) > 0) {
                            sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", resultSet[i].ReceiptNwt.ToString().Trim()));
                        }
                        else {
                            sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", " "));
                        }

                        if (Convert.ToInt32(resultSet[i].IssueUnits) > 0) {
                            sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", resultSet[i].IssueUnits.ToString().Trim()));
                        }
                        else {
                            sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", " "));
                        }
                        if (Convert.ToInt32(resultSet[i].IssueGwt) > 0) {
                            sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", resultSet[i].IssueGwt.ToString().Trim()));
                        }
                        else {
                            sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", " "));
                        }
                        if (Convert.ToInt32(resultSet[i].IssueNwt) > 0) {
                            sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", resultSet[i].IssueNwt.ToString().Trim()));
                        }
                        else {
                            sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", " "));
                        }
                        sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", resultSet[i].ClosingUnits.ToString().Trim()));
                        sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", resultSet[i].ClosingGwt.ToString().Trim()));
                        sb.AppendLine(string.Format("<TD ALIGN = \"RIGHT\">{0}</TD>", resultSet[i].ClosingNwt.ToString().Trim()));
                        sb.AppendLine("</TR>");
                    }
                    OQty += Convert.ToInt32(resultSet[i].OpeningUnits);
                    OGwt += Convert.ToDecimal(resultSet[i].OpeningGWt);
                    ONetWt += Convert.ToDecimal(resultSet[i].OpeningNwt);

                    RQty += Convert.ToInt32(resultSet[i].ReceiptUnits);
                    RGwt += Convert.ToDecimal(resultSet[i].ReceiptGWt);
                    RNetWt += Convert.ToDecimal(resultSet[i].ReceiptNwt);

                    IQty += Convert.ToInt32(resultSet[i].IssueUnits);
                    IGwt += Convert.ToDecimal(resultSet[i].IssueGwt);
                    INetWt += Convert.ToDecimal(resultSet[i].IssueNwt);

                    CQty += Convert.ToInt32(resultSet[i].ClosingUnits);
                    CGwt += Convert.ToDecimal(resultSet[i].ClosingGwt);
                    CNetWt += Convert.ToDecimal(resultSet[i].ClosingNwt);
                }

                if (resultSet.Count > 0)
                    sb.AppendLine(string.Format("<TR  bgcolor='FFFACD'style=\"color:black; text-decoration:bold\" class = \"tableStyle\" > <td COLSPAN=1 class=\"fullborderStyle\" align=\"LEFT\"><b>{0}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{1}</b></td> <td class=\"fullborderStyle\" align=\"RIGHT\"><b>{2}</b></td> <td class=\"fullborderStyle\" align=\"RIGHT\"><b>{3}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{4}</b></td> <td class=\"fullborderStyle\" align=\"RIGHT\"><b>{5}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{6}</b></td>  <td class=\"fullborderStyle\" align=\"RIGHT\"><b>{7}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{8}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{9}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{10}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{11}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{12}</b></td> ",
                   "Total : ", OQty, OGwt, ONetWt, RQty, RGwt, RNetWt, IQty, IGwt, INetWt, CQty, CGwt, CNetWt));
                OQty = 0; OGwt = 0; ONetWt = 0; RQty = 0; RGwt = 0; RNetWt = 0; IQty = 0; IGwt = 0; INetWt = 0; CQty = 0; CGwt = 0; CNetWt = 0;
            }

            object tempOQty = result.Sum(s => s.OpeningUnits);
            if (tempOQty != null && tempOQty != DBNull.Value)
                OQty = Convert.ToInt32(tempOQty);

            object tempOGwt = result.Sum(s => s.OpeningGWt);
            if (tempOGwt != null && tempOGwt != DBNull.Value)
                OGwt = Convert.ToDecimal(tempOGwt);

            object tempONetWt = result.Sum(s => s.OpeningNwt);
            if (tempONetWt != null && tempONetWt != DBNull.Value)
                ONetWt = Convert.ToDecimal(tempONetWt);


            object tempRQty = result.Sum(s => s.ReceiptUnits);
            if (tempRQty != null && tempRQty != DBNull.Value)
                RQty = Convert.ToInt32(tempRQty);

            object tempRGwt = result.Sum(s => s.ReceiptGWt);
            if (tempRGwt != null && tempRGwt != DBNull.Value)
                RGwt = Convert.ToDecimal(tempRGwt);

            object tempRNetWt = result.Sum(s => s.ReceiptNwt);
            if (tempRNetWt != null && tempRNetWt != DBNull.Value)
                RNetWt = Convert.ToDecimal(tempRNetWt);


            object tempIQty = result.Sum(s => s.IssueUnits);
            if (tempIQty != null && tempIQty != DBNull.Value)
                IQty = Convert.ToInt32(tempIQty);

            object tempIGwt = result.Sum(s => s.IssueGwt);
            if (tempIGwt != null && tempIGwt != DBNull.Value)
                IGwt = Convert.ToDecimal(tempIGwt);

            object tempiNetWt = result.Sum(s => s.IssueNwt);
            if (tempiNetWt != null && tempiNetWt != DBNull.Value)
                INetWt = Convert.ToDecimal(tempiNetWt);


            object tempCQty = result.Sum(s => s.ClosingUnits);
            if (tempCQty != null && tempCQty != DBNull.Value)
                CQty = Convert.ToInt32(tempCQty);

            object tempCGwt = result.Sum(s => s.ClosingGwt);
            if (tempCGwt != null && tempCGwt != DBNull.Value)
                CGwt = Convert.ToDecimal(tempCGwt);

            object tempCNetWt = result.Sum(s => s.ClosingNwt);
            if (tempCNetWt != null && tempCNetWt != DBNull.Value)
                CNetWt = Convert.ToDecimal(tempCNetWt);

            if (resultByGs.Count > 1) {
                sb.AppendLine(string.Format("<TR  bgcolor='FFFACD'style=\"color:black; text-decoration:bold\" class = \"tableStyle\" > <td COLSPAN=1 class=\"fullborderStyle\" align=\"LEFT\"><b>{0}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{1}</b></td> <td class=\"fullborderStyle\" align=\"RIGHT\"><b>{2}</b></td> <td class=\"fullborderStyle\" align=\"RIGHT\"><b>{3}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{4}</b></td> <td class=\"fullborderStyle\" align=\"RIGHT\"><b>{5}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{6}</b></td>  <td class=\"fullborderStyle\" align=\"RIGHT\"><b>{7}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{8}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{9}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{10}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{11}</b></td><td class=\"fullborderStyle\" align=\"RIGHT\"><b>{12}</b></td>",
               " Grand Total : ", OQty, OGwt, ONetWt, RQty, RGwt, RNetWt, IQty, IGwt, INetWt, CQty, CGwt, CNetWt));
            }

            sb.AppendLine("</TABLE>");
            sb.AppendLine("</BODY>");
            sb.AppendLine("</HTML>");
            return sb.ToString();
        }

        private StringBuilder GetHeader(int maxWidth, string companyCode, string branchCode)
        {
            string tempString = string.Empty;
            StringBuilder sb = new StringBuilder();
            string esc = Convert.ToChar(27).ToString();
            sb.Append(Convert.ToChar(18).ToString());
            sb.Append(esc + Convert.ToChar(77).ToString());
            sb.Append(esc + Convert.ToChar(120).ToString() + Convert.ToChar(48).ToString());
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode && com.branch_code == branchCode).FirstOrDefault();
            if (company != null) {
                sb.Append(esc + Convert.ToChar(69).ToString());
                sb.Append(esc + Convert.ToChar(14).ToString() + Convert.ToChar(14).ToString());
                sb.Append(esc + Convert.ToChar(52).ToString());
                sb.Append(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.company_name.ToString()));
                sb.Append(esc + Convert.ToChar(53).ToString());
                sb.Append(Convert.ToChar(20).ToString());
                sb.Append(esc + Convert.ToChar(70).ToString());
                sb.AppendLine();
                if (!string.IsNullOrEmpty(company.address1.ToString()))
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.address1.ToString()));
                if (!string.IsNullOrEmpty(company.address2.ToString()))
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.address2.ToString()));
                if (!string.IsNullOrEmpty(company.address3.ToString()))
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.address3.ToString()));
            }
            return sb;
        }

        private string GetStyle()
        {
            return "<style = 'text/css'>\n\r\n.boldText, .boldtable TD, .boldtable TH\n\r\n{\n\r\nfont-family: 'Times New Roman';\n\r\nfont-size:12pt;\n\r\n}\n\r\n.tableStyle\n\r\n{\n\r\nborder-left: 1px solid black;\n\r\nborder-right: 1px solid black;\n\r\nborder-top-style:none;\n\r\nborder-bottom-style:none;\n\r\nborder-collapse: collapse;\n\r\n}\n\r\n.table,TD,TH\n\r\n{\n\r\nborder-left-style:none;\n\r\nborder-right: 1px solid black;\n\r\nborder-bottom-style:none;\n\r\nborder-top-style:1px solid;\n\r\nborder-collapse: collapse;\n\r\n}\n\r\n.noborder\n\r\n{\n\r\nborder-left-style:none;\n\r\nborder-right-style:none;\n\r\nborder-top-style:none;;\n\r\nborder-bottom-style:none;\n\r\nborder-collapse: collapse;\n\r\n}\n\r\n.fullborder\n\r\n{\n\r\nborder-left-style:none;;\n\r\nborder-right: 1px solid black;\n\r\nborder-bottom: 1px solid black;\n\r\nborder-top: 1px solid black;\n\r\nborder-collapse: collapse;\n\r\n}\n\r\n</style>\n\r\n";
        }
        #endregion
    }

    #region Reporting Related Classes
    public class StockSummaryReport
    {
        public string GSCode { get; set; }
        public string GSName { get; set; }

        public string CounterCode { get; set; }
        public string CounterName { get; set; }

        public decimal OpeningWt { get; set; }
        public int OpeningPcs { get; set; }
        public decimal? IssueWt { get; set; }
        public decimal? ReceiptWt { get; set; }
        public decimal? SalesWt { get; set; }
        public decimal? ClosingWt { get; set; }
        public int ClosingPcs { get; set; }
    }

    public class StockDetaildReport
    {
        public string GS { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal OpnQty { get; set; }
        public decimal OpnGwt { get; set; }
        public decimal BarQty { get; set; }
        public decimal BarGwt { get; set; }
        public decimal SalQty { get; set; }
        public decimal? SalGwt { get; set; }
        public decimal RecQty { get; set; }
        public decimal? RecGwt { get; set; }
        public decimal IssQty { get; set; }
        public decimal? IssGwt { get; set; }
        public decimal ClsQty { get; set; }
        public decimal? ClsGwt { get; set; }
        public string Cntr { get; set; }
    }

    public class ClosingStockReport
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string GS { get; set; }
        public decimal? TotalGrossWt { get; set; }
        public int TotalQty { get; set; }
        public decimal TotalNetWt { get; set; }
    }

    public class ClosingStockReportHistory
    {
        public string GSCode { get; set; }
        public string ItemCode { get; set; }
        public string Counter { get; set; }
        public decimal? ClosingGrossWt { get; set; }
        public decimal ClosingNetWt { get; set; }
        public int ClosingUnit { get; set; }
        public decimal? GrossWtPercentage { get; set; }
        public decimal NetWtPercentage { get; set; }
    }

    public class CounterSummaryReport
    {
        public string GSCode { get; set; }
        public string GSName { get; set; }
        public string CounterCode { get; set; }
        public string CounterName { get; set; }
        public int OpeningUnits { get; set; }
        public decimal OpeningGWt { get; set; }
        public decimal? OpeningSwt { get; set; }
        public decimal? OpeningNwt { get; set; }

        public int IssueUnits { get; set; }
        public decimal? IssueGwt { get; set; }
        public decimal? IssueSwt { get; set; }
        public decimal? IssueNwt { get; set; }

        public int ReceiptUnits { get; set; }
        public decimal? ReceiptGWt { get; set; }
        public decimal? ReceiptSwt { get; set; }
        public decimal? ReceiptNwt { get; set; }
        public int ClosingUnits { get; set; }
        public decimal? ClosingGwt { get; set; }
        public decimal? ClosingSwt { get; set; }
        public decimal ClosingNwt { get; set; }
    }
    #endregion
}
