using System.Web.Http;
using ProdigyAPI.BL.ViewModel.Error;
using System.Web.Http.Description;
using System.Threading.Tasks;
using System.Collections;
using ProdigyAPI.Framework;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.BL.ViewModel.Stock;
using ProdigyAPI.Handlers;
using System;
using System.Linq;
using System.Web.Http.OData.Query;
using ProdigyAPI.BL.ViewModel.Master;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Net;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.BusinessLayer.Order;
using ProdigyAPI.BL.BusinessLayer.Stock;

namespace ProdigyAPI.Controllers.Stock
{
    [Authorize]
    [RoutePrefix("api/Stock/CounterStock")]
    public class CounterStockController : SIBaseApiController<CounterStockVM>, IBaseMasterActionController<CounterStockVM, CounterStockVM>
    {

        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        private int GetAndUpdateNextSeriesNo(string companyCode, string branchCode, string numberSeries)
        {
            var kstsSeqNos = db.KSTS_SEQ_NOS.Where(
                                rs => rs.company_code == companyCode
                                && rs.branch_code == branchCode
                                && rs.obj_id == numberSeries).FirstOrDefault();
            if (kstsSeqNos == null)
                return 0;

            int nextTxnNo = kstsSeqNos.nextno;
            int transactionNoPrefixedwithFinYear = Convert.ToInt32(db.KSTU_ACC_FY_MASTER.FirstOrDefault().fin_year.ToString().Remove(0, 1)
                + nextTxnNo);
            kstsSeqNos.nextno = nextTxnNo + 1;
            db.Entry(kstsSeqNos).State = System.Data.Entity.EntityState.Modified;

            return transactionNoPrefixedwithFinYear;
        }

        #region Controller Methods

        #region Report Methods
        [HttpGet]
        [Route("CounterStockSummaryReport")]
        public IHttpActionResult StockSummaryReport(string companyCode, string branchCode, string gsCode = null, DateTime? asOnDate = null)
        {
            ErrorVM error = new ErrorVM();
            if(string.IsNullOrEmpty(gsCode))
                gsCode = "All";
            string print = new CounterStockBL().GetCounterStockSummaryReport(companyCode, branchCode, gsCode, asOnDate, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        [HttpGet]
        [Route("CounterStockDetailReport")]
        public IHttpActionResult StockDetailedReport(string companyCode, string branchCode, string gsCode = null, string counterCode = null, DateTime? asOnDate = null)
        {
            ErrorVM error = new ErrorVM();
            if(string.IsNullOrEmpty(gsCode))
                gsCode = "All";
            if(string.IsNullOrEmpty(counterCode))
                counterCode = "All";
            string print = new CounterStockBL().GetStockDetailedReport(companyCode, branchCode, gsCode, counterCode, asOnDate, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        [HttpGet]
        [Route("ClosingStockReport")]
        public IHttpActionResult ClosingStockReport(string companyCode, string branchCode, string gsCode = null, string counterCode = null, DateTime? asOnDate = null)
        {
            ErrorVM error = new ErrorVM();
            if(string.IsNullOrEmpty(gsCode))
                gsCode = "All";
            if(string.IsNullOrEmpty(counterCode))
                counterCode = "All";
            var print = new CounterStockBL().GetClosingStockReport(companyCode, branchCode, gsCode, counterCode, asOnDate, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        [HttpGet]
        [Route("CounterSummaryReport")]
        public IHttpActionResult CounterSummaryReport(string companyCode, string branchCode, string gsCode = null, string counterCode = null, DateTime? asOnDate = null)
        {
            ErrorVM error = new ErrorVM();
            if(string.IsNullOrEmpty(gsCode))
                gsCode = "All";
            if(string.IsNullOrEmpty(counterCode))
                counterCode = "All";
            var print = new CounterStockBL().GetCounterSummaryReport(companyCode, branchCode, gsCode, counterCode, asOnDate, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        #endregion

        #region CRUD Methods
        /// <summary>
        /// Get GS Information
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("LoadGS/{companyCode}/{branchCode}")]
        [Route("LoadGS")]
        public IHttpActionResult LoadGS(string companyCode, string branchCode)
        {
            List<OrderGSTypeVM> lstOrderGSType = new OrderBL().GetGSTTypes(companyCode, branchCode);
            lstOrderGSType.Insert(0, new OrderGSTypeVM() { Name = "ALL", gsCode = "ALL" });
            return Ok(lstOrderGSType);
        }

        /// <summary>
        /// Get GS Information
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Counter/{companyCode}/{branchCode}")]
        [Route("Counter")]
        public IHttpActionResult Counter(string companyCode, string branchCode)
        {
            List<OrderGSTypeVM> lstOrderGSType = (from c in db.KSTU_COUNTER_MASTER
                                                  where c.company_code == companyCode && c.branch_code == branchCode && c.obj_status == "O"
                                                  orderby c.counter_code
                                                  select new OrderGSTypeVM()
                                                  {
                                                      gsCode = c.counter_code,
                                                      Name = c.counter_name
                                                  }).ToList();
            lstOrderGSType.Insert(0, new OrderGSTypeVM() { Name = "ALL", gsCode = "ALL" });
            return Ok(lstOrderGSType);
        }

        /// <summary>
        /// Get CounterStockInformation
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="CounterCode"></param>
        /// <param name="ItemCode"></param>
        /// <param name="AsOnDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStockInfo/{companyCode}/{branchCode}/{gsCode}/{CounterCode}/{AsOnDate}")]
        public IHttpActionResult GetStockInfo(string companyCode, string branchCode, string gsCode, string CounterCode, DateTime AsOnDate)
        {
            var stockInfo = from s in db.KTTU_COUNTER_STOCK
                            where s.company_code == companyCode
                            && s.branch_code == branchCode
                            && System.Data.Entity.DbFunctions.TruncateTime(s.date) == AsOnDate.Date
                            && s.gs_code == (gsCode.ToUpper() == "ALL" ? s.gs_code : gsCode)
                            && s.counter_code == (CounterCode.ToUpper() == "ALL" ? s.counter_code : CounterCode)
                            select new
                            {
                                GS = s.gs_code,
                                ItemName = s.item_name,
                                OpnQty = s.op_units,
                                OpnGwt = s.op_gwt,
                                BarQty = s.barcoded_units,
                                BarGwt = s.barcoded_gwt,
                                SalQty = s.sales_units,
                                SalGwt = s.sales_gwt,
                                RecQty = s.receipt_units,
                                RecGwt = s.receipt_gwt,
                                IssQty = s.issues_units,
                                IssGwt = s.issues_gwt,
                                ClsQty = s.closing_units,
                                ClsGwt = s.closing_gwt,
                                Cntr = s.counter_code
                            };
            var q = stockInfo.ToString();

            if (stockInfo == null) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Empty!" });
            }
            #region Code not needed
            //if (((string.IsNullOrEmpty(GsCode) == false) && GsCode != "All") && ((string.IsNullOrEmpty(ItemCode) == false) && ItemCode != "All") && ((string.IsNullOrEmpty(CounterCode) == false) && CounterCode != "All")) {
            //    var stock = stockInfo.Where(s => s.GS == GsCode && s.ItemName == ItemCode && s.Cntr == CounterCode).ToList();
            //    return Ok(stock);
            //}
            //else if ((string.IsNullOrEmpty(GsCode) == false) && GsCode != "All") {
            //    var stock = stockInfo.Where(s => s.GS == GsCode).ToList();
            //    return Ok(stock);
            //}
            //else if ((string.IsNullOrEmpty(ItemCode) == false) && ItemCode != "All") {
            //    var stock = stockInfo.Where(s => s.ItemName == ItemCode).ToList();
            //    return Ok(stock);
            //}
            //else if ((string.IsNullOrEmpty(CounterCode) == false) && CounterCode != "All") {
            //    var stock = stockInfo.Where(s => s.Cntr == CounterCode).ToList();
            //    return Ok(stock);
            //}
            //else if (((string.IsNullOrEmpty(GsCode) == false) && GsCode != "All") && ((string.IsNullOrEmpty(ItemCode) == false) && ItemCode != "All")) {
            //    var stock = stockInfo.Where(s => s.GS == GsCode && s.ItemName == ItemCode).ToList();
            //    return Ok(stock);
            //}
            //else if (((string.IsNullOrEmpty(GsCode) == false) && GsCode != "All") && ((string.IsNullOrEmpty(CounterCode) == false) && CounterCode != "All")) {
            //    var stock = stockInfo.Where(s => s.GS == GsCode && s.Cntr == CounterCode).ToList();
            //    return Ok(stock);
            //}
            //else if (((string.IsNullOrEmpty(ItemCode) == false) && ItemCode != "All") && ((string.IsNullOrEmpty(CounterCode) == false) && CounterCode != "All")) {
            //    var stock = stockInfo.Where(s => s.ItemName == ItemCode && s.Cntr == CounterCode).ToList();
            //    return Ok(stock);
            //} 
            #endregion
            return Ok(stockInfo.ToList());
        }

        /// <summary>
        /// Get CounterStockInformation
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="CounterCode"></param>
        /// <param name="AsOnDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStockSumInfo/{companyCode}/{branchCode}/{gsCode}/{CounterCode}/{AsOnDate}")]
        public IHttpActionResult GetStockSumInfo(string companyCode, string branchCode, string gsCode, string CounterCode, DateTime AsOnDate)
        {
            if (gsCode.ToUpper() == "ALL" && CounterCode.ToUpper() == "ALL") {
                var stockInfo = from s in db.KTTU_COUNTER_STOCK
                                where s.company_code == companyCode
                                && s.branch_code == branchCode
                                && System.Data.Entity.DbFunctions.TruncateTime(s.date) == AsOnDate.Date
                                group s by new { s.branch_code } into stockGroup
                                select new
                                {
                                    OpnQty = stockGroup.Sum(s => s.op_units),
                                    OpnGwt = stockGroup.Sum(s => s.op_gwt),
                                    BarQty = stockGroup.Sum(s => s.barcoded_units),
                                    BarGwt = stockGroup.Sum(s => s.barcoded_gwt),
                                    SalQty = stockGroup.Sum(s => s.sales_units),
                                    SalGwt = stockGroup.Sum(s => s.sales_gwt),
                                    RecQty = stockGroup.Sum(s => s.receipt_units),
                                    RecGwt = stockGroup.Sum(s => s.receipt_gwt),
                                    IssQty = stockGroup.Sum(s => s.issues_units),
                                    IssGwt = stockGroup.Sum(s => s.issues_gwt),
                                    ClsQty = stockGroup.Sum(s => s.closing_units),
                                    ClsGwt = stockGroup.Sum(s => s.closing_gwt)
                                };
                if (stockInfo == null) {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "No stock details" });
                }
                return Ok(stockInfo.ToList());
            }
            else if (gsCode.ToUpper() == "ALL" && CounterCode.ToUpper() != "ALL") {
                var stockInfo = from s in db.KTTU_COUNTER_STOCK
                                where s.company_code == companyCode
                                && s.branch_code == branchCode
                                && System.Data.Entity.DbFunctions.TruncateTime(s.date) == AsOnDate.Date
                                && s.counter_code == CounterCode
                                group s by new { s.branch_code } into stockGroup
                                select new
                                {
                                    OpnQty = stockGroup.Sum(s => s.op_units),
                                    OpnGwt = stockGroup.Sum(s => s.op_gwt),
                                    BarQty = stockGroup.Sum(s => s.barcoded_units),
                                    BarGwt = stockGroup.Sum(s => s.barcoded_gwt),
                                    SalQty = stockGroup.Sum(s => s.sales_units),
                                    SalGwt = stockGroup.Sum(s => s.sales_gwt),
                                    RecQty = stockGroup.Sum(s => s.receipt_units),
                                    RecGwt = stockGroup.Sum(s => s.receipt_gwt),
                                    IssQty = stockGroup.Sum(s => s.issues_units),
                                    IssGwt = stockGroup.Sum(s => s.issues_gwt),
                                    ClsQty = stockGroup.Sum(s => s.closing_units),
                                    ClsGwt = stockGroup.Sum(s => s.closing_gwt)
                                };
                if (stockInfo == null) {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "No stock details" });
                }
                return Ok(stockInfo.ToList());
            }
            else if (gsCode.ToUpper() != "ALL" && CounterCode.ToUpper() == "ALL") {
                var stockInfo = from s in db.KTTU_COUNTER_STOCK
                                where s.company_code == companyCode
                                && s.branch_code == branchCode
                                && System.Data.Entity.DbFunctions.TruncateTime(s.date) == AsOnDate.Date
                                && s.gs_code == gsCode
                                group s by new { s.branch_code } into stockGroup
                                select new
                                {
                                    OpnQty = stockGroup.Sum(s => s.op_units),
                                    OpnGwt = stockGroup.Sum(s => s.op_gwt),
                                    BarQty = stockGroup.Sum(s => s.barcoded_units),
                                    BarGwt = stockGroup.Sum(s => s.barcoded_gwt),
                                    SalQty = stockGroup.Sum(s => s.sales_units),
                                    SalGwt = stockGroup.Sum(s => s.sales_gwt),
                                    RecQty = stockGroup.Sum(s => s.receipt_units),
                                    RecGwt = stockGroup.Sum(s => s.receipt_gwt),
                                    IssQty = stockGroup.Sum(s => s.issues_units),
                                    IssGwt = stockGroup.Sum(s => s.issues_gwt),
                                    ClsQty = stockGroup.Sum(s => s.closing_units),
                                    ClsGwt = stockGroup.Sum(s => s.closing_gwt)
                                };
                if (stockInfo == null) {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "No stock details" });
                }
                return Ok(stockInfo.ToList());
            }
            else {
                var stockInfo = from s in db.KTTU_COUNTER_STOCK
                                where s.company_code == companyCode
                                && s.branch_code == branchCode
                                && System.Data.Entity.DbFunctions.TruncateTime(s.date) == AsOnDate.Date
                                && s.gs_code == gsCode
                                && s.counter_code == CounterCode
                                group s by new { s.branch_code } into stockGroup
                                select new
                                {
                                    OpnQty = stockGroup.Sum(s => s.op_units),
                                    OpnGwt = stockGroup.Sum(s => s.op_gwt),
                                    BarQty = stockGroup.Sum(s => s.barcoded_units),
                                    BarGwt = stockGroup.Sum(s => s.barcoded_gwt),
                                    SalQty = stockGroup.Sum(s => s.sales_units),
                                    SalGwt = stockGroup.Sum(s => s.sales_gwt),
                                    RecQty = stockGroup.Sum(s => s.receipt_units),
                                    RecGwt = stockGroup.Sum(s => s.receipt_gwt),
                                    IssQty = stockGroup.Sum(s => s.issues_units),
                                    IssGwt = stockGroup.Sum(s => s.issues_gwt),
                                    ClsQty = stockGroup.Sum(s => s.closing_units),
                                    ClsGwt = stockGroup.Sum(s => s.closing_gwt)
                                };
                if (stockInfo == null) {
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Empty!" });
                }
                return Ok(stockInfo.ToList());
            }
        }

        /// <summary>
        /// Positive Adjustment (Excess) for counter stock
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("PositiveAdjustment")]
        [ResponseType(typeof(CounterStockAdjuststmentVM))]
        public IHttpActionResult PositiveAdjustment(CounterStockAdjuststmentVM cs)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            KTTU_COUNTER_STOCK kcs = db.KTTU_COUNTER_STOCK.Where(d => d.company_code == cs.CompanyCode && d.branch_code == cs.BranchCode &&
                 d.gs_code == cs.GSCode && d.item_name == cs.ItemCode && d.counter_code == cs.CounterCode).FirstOrDefault();
            if (kcs == null) {
                return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = "Stock detail is not found." });
            }

            #region Calculation
            int closingQuantity = kcs.closing_units + cs.Qty;
            cs.StoneWt = Convert.ToDecimal(cs.StoneWt);
            cs.NetWt = cs.GrossWt - (decimal)cs.StoneWt;
            decimal closingGrossWt = Convert.ToDecimal(kcs.closing_gwt + cs.GrossWt);
            decimal closingStoneWt = Convert.ToDecimal(kcs.closing_swt + cs.StoneWt);
            decimal closingNetWt = Convert.ToDecimal(kcs.closing_nwt + cs.NetWt);
            #endregion

            try {
                #region Add Ledger Line
                int transactionNo = GetAndUpdateNextSeriesNo(cs.CompanyCode, cs.BranchCode, "33");

                KTTU_COUNTER_RECEIPT kcr = new KTTU_COUNTER_RECEIPT();
                kcr.obj_id = Common.GetNewGUID();
                kcr.receipt_no = transactionNo;
                kcr.company_code = cs.CompanyCode;
                kcr.branch_code = cs.BranchCode;
                kcr.gs_code = cs.GSCode;
                kcr.item_name = cs.ItemCode;
                kcr.counter_code = cs.CounterCode;
                kcr.operator_code = cs.SalCode;
                kcr.receipt_gwt = cs.GrossWt;
                kcr.receipt_units = cs.Qty;
                kcr.receipt_nwt = cs.NetWt;
                kcr.receipt_swt = cs.StoneWt;
                kcr.receipt_date = Common.GetDateTime();
                kcr.remarks = cs.Remarks;
                kcr.UniqRowID = Guid.NewGuid();
                kcr.UpdateOn = Common.GetCountryDateTime(); //Get transaction date
                db.KTTU_COUNTER_RECEIPT.Add(kcr);
                #endregion

                #region Update Summary Row
                kcs.date = SIGlobals.Globals.GetApplicationDate(cs.CompanyCode, cs.BranchCode);
                kcs.receipt_units = kcs.receipt_units + cs.Qty;
                kcs.receipt_gwt = kcs.receipt_gwt + cs.GrossWt;
                kcs.receipt_swt = kcs.receipt_swt + cs.StoneWt;
                kcs.receipt_nwt = kcs.receipt_nwt + cs.NetWt;
                kcs.closing_units = closingQuantity;
                kcs.closing_gwt = closingGrossWt;
                kcs.closing_swt = closingStoneWt;
                kcs.closing_nwt = closingNetWt;
                kcs.UpdateOn = Common.GetCountryDateTime(); //Get transaction date               
                db.Entry(kcs).State = System.Data.Entity.EntityState.Modified;
                #endregion

                db.SaveChanges();
            }

            catch (Exception exp) {
                string errorMessage = string.Empty;
                if (exp.InnerException.InnerException != null)
                    errorMessage = exp.InnerException.InnerException.Message;
                else
                    errorMessage = exp.Message;
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Exception occurred: " + errorMessage });
            }

            return Ok();
        }
        /// <summary>
        /// Negative Adjustment (Short) for counter stock
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("NegativeAdjustment")]
        [ResponseType(typeof(CounterStockAdjuststmentVM))]
        public IHttpActionResult NegativeAdjustment(CounterStockAdjuststmentVM cs)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            KTTU_COUNTER_STOCK kcs = db.KTTU_COUNTER_STOCK.Where(d => d.company_code == cs.CompanyCode && d.branch_code == cs.BranchCode &&
                 d.gs_code == cs.GSCode && d.item_name == cs.ItemCode && d.counter_code == cs.CounterCode).FirstOrDefault();
            if (kcs == null) {
                return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = "Stock detail is not found." });
            }

            #region Calculation
            cs.StoneWt = Convert.ToDecimal(cs.StoneWt);
            cs.NetWt = cs.GrossWt - (decimal)cs.StoneWt;
            int closingQuantity = Convert.ToInt32(kcs.closing_units - cs.Qty);
            decimal closingGrossWt = Convert.ToDecimal(kcs.closing_gwt - cs.GrossWt);
            decimal closingStoneWt = Convert.ToDecimal(kcs.closing_swt - cs.StoneWt);
            decimal closingNetWt = Convert.ToDecimal(kcs.closing_nwt - cs.NetWt);

            #endregion

            #region Add Ledger Line
            int transactionNo = GetAndUpdateNextSeriesNo(cs.CompanyCode, cs.BranchCode, "26");

            KTTU_COUNTER_ISSUE kci = new KTTU_COUNTER_ISSUE();
            kci.obj_id = Common.GetNewGUID();
            kci.issue_no = transactionNo;
            kci.company_code = cs.CompanyCode;
            kci.branch_code = cs.BranchCode;
            kci.gs_code = cs.GSCode;
            kci.item_name = cs.ItemCode;
            kci.counter_code = cs.CounterCode;
            kci.operator_code = cs.SalCode;
            kci.issues_gwt = cs.GrossWt;
            kci.issues_units = cs.Qty;
            kci.issues_nwt = cs.NetWt;
            kci.issues_swt = cs.StoneWt;
            kci.issued_date = Common.GetDateTime();
            kci.remarks = cs.Remarks;
            kci.UniqRowID = Guid.NewGuid();
            kci.UpdateOn = Common.GetCountryDateTime(); //Get transaction date
            db.KTTU_COUNTER_ISSUE.Add(kci);
            #endregion

            #region Update Summary Row
            kcs.date = SIGlobals.Globals.GetApplicationDate(cs.CompanyCode, cs.BranchCode);
            kcs.issues_units = kcs.issues_units + cs.Qty;
            kcs.issues_gwt = kcs.issues_gwt + cs.GrossWt;
            kcs.issues_swt = kcs.issues_swt + cs.StoneWt;
            kcs.issues_nwt = kcs.issues_nwt + cs.NetWt;
            kcs.closing_units = closingQuantity;
            kcs.closing_gwt = closingGrossWt;
            kcs.closing_swt = closingStoneWt;
            kcs.closing_nwt = closingNetWt;
            kcs.UpdateOn = Common.GetCountryDateTime(); //Get transaction date               
            db.Entry(kcs).State = System.Data.Entity.EntityState.Modified;
            #endregion

            try {
                db.SaveChanges();
            }

            catch (Exception exp) {
                string errorMessage = string.Empty;
                if (exp.InnerException.InnerException != null)
                    errorMessage = exp.InnerException.InnerException.Message;
                else
                    errorMessage = exp.Message;
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Exception occurred: " + errorMessage });
            }

            return Ok();
        }
        #endregion

        public IHttpActionResult Post(CounterStockVM t)
        {
            throw new NotImplementedException();
        }
        public IHttpActionResult Count(ODataQueryOptions<CounterStockVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<CounterStockVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, CounterStockVM t)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
