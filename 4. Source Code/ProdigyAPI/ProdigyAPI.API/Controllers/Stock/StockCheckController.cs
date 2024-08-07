using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ProdigyAPI.BL.ViewModel.Stock;

namespace ProdigyAPI.Controllers.Stock
{
    [Authorize]
    [RoutePrefix("api/Stock/StockCheck")]
    public class StockCheckController : ApiController
    {

        MagnaDbEntities db = new MagnaDbEntities();

        [HttpGet]
        [Route("TagSummary/{companyCode}/{branchCode}/{gSCode}/{counterCode}/{itemCode}")]
        [Route("TagSummary")]
        [ResponseType(typeof(StockCheckerVM))]
        public IHttpActionResult GetTagSummary(string companyCode, string branchCode, string gSCode, string counterCode, string itemCode)
        {
            #region Though this is valid query, this would not give correct result due to NULL in the join. So this is ignored and kept for knowledge purpose
            //var tags = from b in db.KTTU_BARCODE_MASTER
            //           join s in db.KTTU_BARCODE_STONE_DETAILS
            //           on new { CompanyCode = b.company_code, BranchCode = b.branch_code, BarcodeNo = b.barcode_no }
            //           equals new { CompanyCode = s.company_code, BranchCode = s.branch_code, BarcodeNo = s.barcode_no } into leftOuterJoin
            //           from s in leftOuterJoin.DefaultIfEmpty()
            //           where b.company_code == companyCode && b.branch_code == branchCode
            //              && b.gs_code == gSCode && b.counter_code == counterCode && b.item_name == itemCode
            //              && b.sold_flag != "Y" && b.isConfirmed == "Y"
            //              && s.type == "D"
            //           group new { b, s } by new
            //           {
            //               CompanyCode = b.company_code,
            //               BranchCode = b.branch_code,
            //               GsCode = b.gs_code,
            //               CounterCode = b.counter_code,
            //               ItemCode = b.item_name
            //           } into g
            //           select new TagSummary
            //           {
            //               Count = g.Count(),
            //               Qty = (int)g.Sum(e => e.b.qty),
            //               GrossWt = (decimal)g.Sum(e => e.b.gwt),
            //               StoneWt = (decimal)g.Sum(e => e.b.swt),
            //               NetWt = (decimal)g.Sum(e => e.b.nwt),
            //               Dcts = (decimal)g.Sum(e => e.s.carrat)
            //           }; 
            #endregion
            try {
                db.Configuration.UseDatabaseNullSemantics = true; //Use SQL Server NULL Sematics to avoid NULL checking. This can be dangerous in LEFT & RIGHT joins please note.
                if (itemCode != "ALL") {
                    var existingStockQuery = from b in db.BarcodeInclusiveStones
                                             where b.company_code == companyCode && b.branch_code == branchCode
                                                   && b.gs_code == gSCode && b.counter_code == counterCode && b.item_name == itemCode
                                                   && b.sold_flag != "Y" && b.isConfirmed == "Y"
                                             group new { b } by new
                                             {
                                                 CompanyCode = b.company_code,
                                                 BranchCode = b.branch_code,
                                                 GsCode = b.gs_code,
                                                 CounterCode = b.counter_code,
                                                 ItemCode = b.item_name
                                             } into g
                                             select new TagSummary
                                             {
                                                 StockType = "Existing Stock",
                                                 Count = g.Count(),
                                                 Qty = (int)g.Sum(e => e.b.qty),
                                                 GrossWt = (decimal)g.Sum(e => e.b.gwt),
                                                 StoneWt = (decimal)g.Sum(e => e.b.swt),
                                                 NetWt = (decimal)g.Sum(e => e.b.nwt),
                                                 Dcts = (decimal)g.Sum(e => e.b.DiaCaret)
                                             };

                    var scannedStockQuery = from st in db.KTTU_STOCK_TAKING
                                            where st.company_code == companyCode && st.branch_code == branchCode
                                               && st.item_name == itemCode && st.counter_code == counterCode
                                            //&& st.gs == gSCode //Unfortunately this is not found in Magna
                                            group st by new
                                            {
                                                CompanyCode = st.company_code,
                                                BranchCode = st.branch_code,
                                                ItemCode = st.item_name,
                                                CounterCode = st.counter_code
                                            } into g
                                            select new TagSummary
                                            {
                                                StockType = "Scanned Stock",
                                                Count = g.Count(),
                                                Qty = g.Sum(e => e.units),
                                                GrossWt = g.Sum(e => e.gwt),
                                                StoneWt = 0M,
                                                NetWt = g.Sum(e => e.nwt),
                                                Dcts = (decimal)g.Sum(e => e.D_cts)
                                            };
                    var existing = existingStockQuery.FirstOrDefault();
                    var scanned = scannedStockQuery.FirstOrDefault();
                    var remaining = new TagSummary();
                    remaining.StockType = "Remaining Tags";
                    if (existing != null) {
                        if (scanned != null) {
                            remaining.Count = existing.Count - scanned.Count;
                            remaining.Qty = existing.Qty - scanned.Qty;
                            remaining.GrossWt = existing.GrossWt - scanned.GrossWt;
                            remaining.StoneWt = existing.StoneWt - scanned.StoneWt;
                            remaining.NetWt = existing.NetWt - scanned.NetWt;
                            remaining.Dcts = existing.Dcts - scanned.Dcts;
                        }
                        else {
                            remaining.Count = existing.Count;
                            remaining.Qty = existing.Qty;
                            remaining.GrossWt = existing.GrossWt;
                            remaining.StoneWt = existing.StoneWt;
                            remaining.NetWt = existing.NetWt;
                            remaining.Dcts = existing.Dcts;
                        }
                    }

                    #region Added this on request from API Team
                    if (existing == null)
                        existing = new TagSummary
                        {
                            StockType = "Existing Stock",
                            Count = 0,
                            Qty = 0,
                            GrossWt = 0.00M,
                            StoneWt = 0.00M,
                            NetWt = 0.00M,
                            Dcts = 0.00M
                        };
                    if (scanned == null)
                        scanned = new TagSummary()
                        {
                            StockType = "Scanned Stock",
                            Count = 0,
                            Qty = 0,
                            GrossWt = 0.00M,
                            StoneWt = 0.00M,
                            NetWt = 0.00M,
                            Dcts = 0.00M
                        };
                    #endregion

                    #region This query too will work but consumes hell lot of query time. I'm a performance geek and I don't want to show too much Linq knowledge and therefore it is commented.
                    //var remainingItems1 = from t in existingStockQuery
                    //                      from s in scannedStockQuery
                    //                      select new TagSummary
                    //                     {
                    //                         StockType = "Remaining Stock",
                    //                         Count = t.Count - s.Count,
                    //                         Qty = t.Qty - s.Qty,
                    //                         GrossWt = t.GrossWt - s.GrossWt,
                    //                         StoneWt = t.StoneWt - s.StoneWt,
                    //                         NetWt = t.NetWt - s.NetWt,
                    //                         Dcts = t.Dcts - s.Dcts
                    //                     };
                    //var remaining1 = remainingItems1.FirstOrDefault();
                    #endregion

                    StockCheckerVM stockChecker = new StockCheckerVM
                    {
                        CompanyCode = companyCode,
                        BranchCode = branchCode,
                        GS = gSCode,
                        Counter = counterCode,
                        Item = itemCode,
                        BarcodedTags = existing,
                        ScannedTags = scanned,
                        RemainingTags = remaining
                    };

                    return Ok(stockChecker);
                }
                else {
                    var existingStockQuery = from b in db.BarcodeInclusiveStones
                                             where b.company_code == companyCode && b.branch_code == branchCode
                                                   && b.gs_code == gSCode && b.counter_code == counterCode //Note: && b.item_name == itemCode is removed for ALL condition
                                                   && b.sold_flag != "Y" && b.isConfirmed == "Y"
                                             group new { b } by new
                                             {
                                                 CompanyCode = b.company_code,
                                                 BranchCode = b.branch_code,
                                                 GsCode = b.gs_code,
                                                 CounterCode = b.counter_code
                                                 //,ItemCode = b.item_name  //this is also removed
                                             } into g
                                             select new TagSummary
                                             {
                                                 StockType = "Existing Stock",
                                                 Count = g.Count(),
                                                 Qty = (int)g.Sum(e => e.b.qty),
                                                 GrossWt = (decimal)g.Sum(e => e.b.gwt),
                                                 StoneWt = (decimal)g.Sum(e => e.b.swt),
                                                 NetWt = (decimal)g.Sum(e => e.b.nwt),
                                                 Dcts = (decimal)g.Sum(e => e.b.DiaCaret)
                                             };

                    var scannedStockQuery = from st in db.KTTU_STOCK_TAKING
                                            where st.company_code == companyCode && st.branch_code == branchCode
                                               && st.counter_code == counterCode //Note: && st.item_name == itemCode  is removed
                                                                                 //&& st.gs == gSCode //Unfortunately this is not found in Magna
                                            group st by new
                                            {
                                                CompanyCode = st.company_code,
                                                BranchCode = st.branch_code,
                                                CounterCode = st.counter_code
                                                //,ItemCode = st.item_name //this is also removed
                                            } into g
                                            select new TagSummary
                                            {
                                                StockType = "Scanned Stock",
                                                Count = g.Count(),
                                                Qty = g.Sum(e => e.units),
                                                GrossWt = g.Sum(e => e.gwt),
                                                StoneWt = 0M,
                                                NetWt = g.Sum(e => e.nwt),
                                                Dcts = (decimal)g.Sum(e => e.D_cts)
                                            };
                    var existing = existingStockQuery.FirstOrDefault();
                    var scanned = scannedStockQuery.FirstOrDefault();
                    var remaining = new TagSummary();
                    remaining.StockType = "Remaining Tags";
                    if (existing != null) {
                        if (scanned != null) {
                            remaining.Count = existing.Count - scanned.Count;
                            remaining.Qty = existing.Qty - scanned.Qty;
                            remaining.GrossWt = existing.GrossWt - scanned.GrossWt;
                            remaining.StoneWt = existing.StoneWt - scanned.StoneWt;
                            remaining.NetWt = existing.NetWt - scanned.NetWt;
                            remaining.Dcts = existing.Dcts - scanned.Dcts;
                        }
                        else {
                            remaining.Count = existing.Count;
                            remaining.Qty = existing.Qty;
                            remaining.GrossWt = existing.GrossWt;
                            remaining.StoneWt = existing.StoneWt;
                            remaining.NetWt = existing.NetWt;
                            remaining.Dcts = existing.Dcts;
                        }
                    }

                    #region Added this on request from API Team
                    if (existing == null)
                        existing = new TagSummary
                        {
                            StockType = "Existing Stock",
                            Count = 0,
                            Qty = 0,
                            GrossWt = 0.00M,
                            StoneWt = 0.00M,
                            NetWt = 0.00M,
                            Dcts = 0.00M
                        };
                    if (scanned == null)
                        scanned = new TagSummary()
                        {
                            StockType = "Scanned Stock",
                            Count = 0,
                            Qty = 0,
                            GrossWt = 0.00M,
                            StoneWt = 0.00M,
                            NetWt = 0.00M,
                            Dcts = 0.00M
                        };
                    #endregion


                    StockCheckerVM stockChecker = new StockCheckerVM
                    {
                        CompanyCode = companyCode,
                        BranchCode = branchCode,
                        GS = gSCode,
                        Counter = counterCode,
                        Item = itemCode,
                        BarcodedTags = existing,
                        ScannedTags = scanned,
                        RemainingTags = remaining
                    };

                    return Ok(stockChecker);
                }
            }
            catch (Exception ex) {
                ErrorVM error = new ErrorVM().GetErrorDetails(ex);
                throw;
            }
        }

        [HttpGet]
        [Route("ScannedTags/{companyCode}/{branchCode}/{gSCode}/{counterCode}/{itemCode}")]
        [Route("ScannedTags")]
        [ResponseType(typeof(usp_stockCheckScannedStockReport_Result))]
        public IHttpActionResult GetScannedTags(string companyCode, string branchCode, string gSCode, string counterCode, string itemCode)
        {
            try {
                var scannedTags = db.usp_stockCheckScannedStockReport(companyCode, branchCode, gSCode, counterCode, itemCode);
                return Ok(scannedTags.ToList());
            }
            catch (Exception exp) {
                string errorMessage = string.Empty;
                if (exp.InnerException.InnerException != null)
                    errorMessage = exp.InnerException.InnerException.Message;
                else
                    errorMessage = exp.Message;
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Exception occurred: " + errorMessage });
            }
        }

        [HttpGet]
        [Route("RemainingTags/{companyCode}/{branchCode}/{gSCode}/{counterCode}/{itemCode}")]
        [Route("RemainingTags")]
        [ResponseType(typeof(usp_stockCheckRemainingTagReport_Result))]
        public IHttpActionResult GetRemainingTags(string companyCode, string branchCode, string gSCode, string counterCode, string itemCode)
        {
            try {
                var remainingTags = db.usp_stockCheckRemainingTagReport(companyCode, branchCode, gSCode, counterCode, itemCode);
                return Ok(remainingTags.ToList());
            }
            catch (Exception exp) {
                string errorMessage = string.Empty;
                if (exp.InnerException.InnerException != null)
                    errorMessage = exp.InnerException.InnerException.Message;
                else
                    errorMessage = exp.Message;
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Exception occurred: " + errorMessage });
            }
        }

        [HttpDelete]
        [Route("ClearStockTaking/{companyCode}/{branchCode}/{gSCode}/{counterCode}/{itemCode}")]
        [Route("ClearStockTaking")]
        public IHttpActionResult ClearStockTaking(string companyCode, string branchCode, string gSCode, string counterCode, string itemCode)
        {
            try {
                int result = db.usp_stockCheckClearStockTakingByItemAttributes(companyCode, branchCode, gSCode, counterCode, itemCode);
                if (result == 0)
                    return Ok();
                else
                    return BadRequest();
            }
            catch (Exception exp) {
                string errorMessage = string.Empty;
                if (exp.InnerException.InnerException != null)
                    errorMessage = exp.InnerException.InnerException.Message;
                else
                    errorMessage = exp.Message;
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Exception occurred: " + errorMessage });
            }
        }


        /// <summary>
        /// Get the counters corresponding to the respective GS
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Counter/{companyCode}/{branchCode}/{gsCode}")]
        [Route("Counter")]
        public IHttpActionResult GetCounters(string companyCode, string branchCode, string gsCode)
        {
            var counterList = from c in db.KSTU_COUNTER_MASTER
                              join cs in db.KTTU_COUNTER_STOCK
                              on new { CC = c.company_code, CB = c.branch_code, CCD = c.counter_code } equals
                              new { CC = cs.company_code, CB = cs.branch_code, CCD = cs.counter_code }
                              where c.company_code == companyCode && c.branch_code == branchCode
                              && c.obj_status == "O" && cs.gs_code == gsCode
                              orderby c.counter_code
                              select new
                              {
                                  CounterCode = c.counter_code,
                                  CounterName = c.counter_name
                              };

            return Ok(counterList.Distinct().ToList());
        }

        /// <summary>
        /// Get Items for the selected GS and counters
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="counterCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Item/{companyCode}/{branchCode}/{gsCode}/{counterCode}")]
        [Route("Item")]
        public IHttpActionResult GetItems(string companyCode, string branchCode, string gsCode, string counterCode)
        {
            var itemList = from i in db.KTTU_COUNTER_STOCK
                           where i.company_code == companyCode && i.branch_code == branchCode
                           && i.counter_code == counterCode && i.gs_code == gsCode
                           && (i.op_gwt > 0 || i.op_units > 0
                           || i.barcoded_units > 0 || i.barcoded_gwt > 0 || i.sales_units > 0 || i.sales_gwt > 0
                           || i.issues_units > 0 || i.issues_gwt > 0 || i.receipt_units > 0 || i.receipt_gwt > 0
                           || i.closing_gwt > 0 || i.closing_units > 0)
                           orderby i.item_name
                           select new
                           {
                               ItemName = i.item_name,
                               ItemCode = i.item_name
                           };
            return Ok(itemList.ToList());
        }

    }
}
