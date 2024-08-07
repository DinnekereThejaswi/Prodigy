using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Stock;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;


namespace ProdigyAPI.Controllers.Stock
{
    /// <summary>
    ///  Provides API's for Stock Taking Module.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Stock/StockTaking")]
    public class StockTakingController : SIBaseApiController<StockTakingVM>
    {
        #region Declaration

        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region all controller methods
        /// <summary>
        /// Get GS for stock check/stock taking
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GS")]
        public IHttpActionResult GetGS(string companyCode, string branchCode)
        {
            var counterList = from c in db.KSTS_GS_ITEM_ENTRY
                              where c.company_code == companyCode && c.branch_code == branchCode && c.bill_type == "S"
                              && (c.measure_type == "P" || c.measure_type == "W")
                              orderby c.display_order
                              select new
                              {
                                  GSCode = c.gs_code,
                                  GSName = c.item_level1_name
                              };

            return Ok(counterList.ToList());
        }

        /// <summary>
        /// Gets all the counters related to the company and branch
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns>Anonumouns object with CounterCode & CounterName values</returns>
        [HttpGet]
        [Route("Counter")]
        public IHttpActionResult GetCounter(string companyCode, string branchCode)
        {
            #region Junk code Not Used
            //var counter = db.KSTU_COUNTER_MASTER
            //             .Select(c => new CounterMasterVM()
            //             {
            //                 CompanyCode = c.company_code,
            //                 BranchCode = c.branch_code,
            //                 CounterCode = c.counter_code,
            //                 CounterName = c.counter_name,
            //             }).OrderBy(s => s.CounterName)
            //     .ToList(); 
            #endregion
            var counterList = from c in db.KSTU_COUNTER_MASTER
                              where c.company_code == companyCode && c.branch_code == branchCode && c.obj_status == "O"
                              orderby c.counter_code
                              select new
                              {
                                  CounterCode = c.counter_code,
                                  CounterName = c.counter_name
                              };

            return Ok(counterList.ToList());
        }

        /// <summary>
        /// Gets all items related to the company, branch and the counter in question
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="counterCode"></param>
        /// <returns>Anonumouns object with ItemCode & ItemName values</returns>
        [HttpGet]
        [Route("Item")]
        public IHttpActionResult GetItem(string companyCode, string branchCode, string counterCode)
        {
            #region Junk code Not Used
            //var item = db.ITEM_MASTER
            //    .Where( s=> s.counter_code == counterCode)
            //         .Select(c => new ItemVM()
            //         {
            //             CompanyCode = c.company_code,
            //             BranchCode = c.branch_code,
            //             ItemName = c.Item_Name,
            //             ItemCode = c.Item_code,
            //         }).OrderBy(s => s.ItemCode)
            // .ToList(); 
            #endregion
            var itemList = from i in db.ITEM_MASTER
                           where i.company_code == companyCode && i.branch_code == branchCode && i.counter_code == counterCode
                           orderby i.Item_code
                           select new
                           {
                               ItemName = i.Item_Name,
                               ItemCode = i.Item_code
                           };
            return Ok(itemList.ToList());
        }

        /// <summary>
        /// Get Salesman detail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Salesman")]
        public IHttpActionResult Salesman(string companyCode, string branchCode)
        {
            return Ok(db.usp_Salesman(companyCode, branchCode));
        }

        /// <summary>
        /// Gets the detail of all stock taking rows
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="batchNo"></param>
        /// <returns>Object of type StockTakingVM</returns>
        [HttpGet]
        [Route("Detail/{companyCode}/{branchCode}/{batchNo}")]
        [Route("Detail")]
        public IHttpActionResult GetStockTakingDetail(string companyCode, string branchCode, int batchNo)
        {
            var query = db.KTTU_STOCK_TAKING
                            .Where(s => s.batch_no == batchNo && s.company_code == companyCode && s.branch_code == branchCode)
                            .Select(c => new StockTakingReport
                            {
                                BatchNo = c.batch_no,
                                BranchCode = c.branch_code,
                                CompanyCode = c.company_code,
                                CounterCode = c.counter_code,
                                SalesmanCode = c.sal_code,
                                Qty = c.units,
                                UpdateOn = c.UpdateOn,
                                BarcodeNo = c.barcode_no,
                                Dcts = c.D_cts,
                                GrossWt = c.gwt,
                                ItemName = c.item_name,
                                NetWt = c.nwt
                            }).OrderByDescending(c => c.UpdateOn);
            return Ok(query.ToList());
        }

        [HttpGet]
        [Route("DetailReport/{companyCode}/{branchCode}/{batchNo}")]
        [Route("DetailReport")]
        public IHttpActionResult GetStockTakingDetailReport(string companyCode, string branchCode, int batchNo)
        {
            ErrorVM error = new ErrorVM();
            var print = new StockTakingBL().GetStockTakingDetaildReport(companyCode, branchCode, batchNo, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Gets the summary of quantity, gross weight, net weight and Dcts for a batch No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("BatchSummary/{companyCode}/{branchCode}/{batchNo}")]
        [Route("BatchSummary")]
        public IHttpActionResult GetBatchSummary(string companyCode, string branchCode, int batchNo)
        {
            var result = (from st in db.KTTU_STOCK_TAKING
                          where st.batch_no == batchNo && st.company_code == companyCode && st.branch_code == branchCode
                          group st by new
                          {
                              CompanyCode = st.company_code,
                              BranchCode = st.branch_code,
                              BatchNo = st.batch_no
                          } into g
                          select new
                          {
                              CompanyCode = g.Key.CompanyCode,
                              BranchCode = g.Key.BranchCode,
                              BatchNo = g.Key.BatchNo,
                              Qty = g.Sum(e => e.units),
                              GrossWt = g.Sum(e => e.gwt),
                              NetWt = g.Sum(e => e.nwt),
                              Dcts = g.Sum(e => e.D_cts)
                          }).ToList();
            return Ok(result);

            #region Joining with Stone details is not giving proper result, this is for educational purposes only
            //decimal caratStandardWt = Convert.ToDecimal(0.2);
            //var result = from st in db.KTTU_STOCK_TAKING
            //             join bc in db.KTTU_BARCODE_STONE_DETAILS
            //             on new { Company = st.company_code, Branch = st.branch_code, Barcode = st.barcode_no }
            //             equals new { Company = bc.company_code, Branch = bc.branch_code, Barcode = bc.barcode_no } into leftOuterJoin
            //             from bc in leftOuterJoin.DefaultIfEmpty()
            //             where st.batch_no == batchNo && st.company_code == companyCode && st.branch_code == branchCode
            //             group new { st, bc } by new
            //             {
            //                 CompanyCode = st.company_code,
            //                 BranchCode = st.branch_code,
            //                 BatchNo = st.batch_no
            //             } into g
            //             select new
            //             {
            //                 CompanyCode = g.Key.CompanyCode,
            //                 BranchCode = g.Key.BranchCode,
            //                 BatchNo = g.Key.BatchNo,
            //                 Qty = g.Sum(x => x.st.units),
            //                 GrossWt = g.Sum(x => x.st.gwt),
            //                 NetWt = g.Sum(x => x.st.nwt),
            //                 Swt = g.Sum(e => e.bc.carrat * caratStandardWt),
            //                 //Dcts = g.Where(bsm => bsm.bc.stone_gs_type == "DMD").Sum(bsm => bsm.bc.carrat) == null ? 0 : g.Where(bsm => bsm.bc.stone_gs_type == "DMD").Sum(bsm => bsm.bc.carrat),
            //                 Dcts = g.Sum(d => d.st.D_cts)
            //             }; 
            //return Ok(result.ToList());
            #endregion
        }

        /// <summary>
        /// Gets the batch header for the first item line so that the header details can be loaded in the UI
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("BatchHeader/{companyCode}/{branchCode}/{batchNo}")]
        [Route("BatchHeader")]
        public IHttpActionResult GetBatchHeader(string companyCode, string branchCode, int batchNo)
        {
            var query = (from st in db.KTTU_STOCK_TAKING
                         where st.batch_no == batchNo && st.company_code == companyCode && st.branch_code == branchCode
                         group st by new
                         {
                             Item = st.item_name,
                             Counter = st.counter_code,
                             Salesman = st.sal_code
                         } into g
                         select new StockTakingHeader
                         {
                             Item = g.Key.Item,
                             Counter = g.Key.Counter,
                             Salesman = g.Key.Salesman,
                             Count = g.Select(x => x.item_name).Count()
                         });
            var result = query.ToList();
            if (result != null && result.Count > 0) {
                if (result.Count() > 1) {
                    var row = result[0];
                    row.Item = "ALL";
                    return Ok(row);
                }
                else
                    return Ok(result[0]);
            }
            else
                return NotFound();
        }

        /// <summary>
        /// Post barcode 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(StockTakingVM))]
        public IHttpActionResult Post([FromBody] StockTakingVM c)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(c.CounterCode)) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Counter is not found." });
            }
            if (string.IsNullOrEmpty(c.ItemName)) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Item is not found." });
            }
            if (string.IsNullOrEmpty(c.SalCode)) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Salesman is not found." });
            }

            if (c.BatchNo != null && c.BatchNo > 0) {
                var ks = db.KTTU_STOCK_TAKING.Where(m => m.company_code == c.CompanyCode && m.branch_code == c.BranchCode && m.batch_no == c.BatchNo).FirstOrDefault();
                if (ks == null)
                    return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = "Invalid Batch No." });

                //Multiple item stock taking should be permitted, so this is commented
                //if (ks.item_name != c.ItemName)
                //    return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = "The item code for this batch No. should be " + ks.item_name });

                if (ks.counter_code != c.CounterCode)
                    return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = "The counter code for this batch No. should be " + ks.counter_code });
            }

            var st = db.KTTU_STOCK_TAKING.Where(m => m.company_code == c.CompanyCode && m.branch_code == c.BranchCode && m.barcode_no == c.BarcodeNo).FirstOrDefault();
            if (st != null) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Tag No. is already scanned. This tag is already scanned for batch No. " + st.batch_no.ToString() });
            }

            var ktms = db.KTTU_BARCODE_MASTER.Where(b => b.barcode_no == c.BarcodeNo && b.company_code == c.CompanyCode
                            && b.branch_code == c.BranchCode
                            //&& b.counter_code == c.CounterCode && b.item_name == c.ItemName
                            && b.sold_flag != "Y" && b.isConfirmed == "Y").FirstOrDefault();
            if (ktms == null) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Invalid Tag No." });
            }
            if (ktms.counter_code != c.CounterCode)
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Invalid Tag No. The counter code for this tag is " + ktms.counter_code });
            if (c.ItemName != "ALL") {
                if (ktms.item_name != c.ItemName)
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Invalid Tag No. The item code for this tag is " + ktms.item_name });
            }

            KTTU_STOCK_TAKING kst = new KTTU_STOCK_TAKING();
            kst.obj_id = Common.GetNewGUID();
            kst.company_code = c.CompanyCode;
            kst.branch_code = c.BranchCode;
            kst.counter_code = c.CounterCode;
            kst.barcode_no = c.BarcodeNo;
            kst.item_name = ktms.item_name;
            var totalCarets = db.KTTU_BARCODE_STONE_DETAILS.Where(k => k.barcode_no == kst.barcode_no &&
                        k.company_code == c.CompanyCode && k.branch_code == c.BranchCode && k.type == "D")
                .Sum(x => x.carrat);
            kst.D_cts = totalCarets == null ? 0.00M : totalCarets;

            kst.gwt = Convert.ToDecimal(ktms.gwt);
            kst.nwt = Convert.ToDecimal(ktms.nwt);
            kst.sal_code = c.SalCode;
            kst.units = Convert.ToInt32(ktms.qty);
            kst.UpdateOn = SIGlobals.Globals.GetDateTime();

            #region With Explicit Transaction
            //using (var transaction = db.Database.BeginTransaction()) {
            //    if (c.BatchNo == null || c.BatchNo <= 0) {
            //        db.SetCommandTimeOut(5);
            //        kst.batch_no = GetAndUpdateNextBatchNo(c.CompanyCode, c.BranchCode);
            //    }
            //    else
            //        kst.batch_no = c.BatchNo;

            //    db.KTTU_STOCK_TAKING.Add(kst);
            //    try {
            //        db.SaveChanges();
            //        transaction.Commit();
            //    }
            //    catch (Exception exp) {
            //        transaction.Rollback();
            //        string errorMessage = string.Empty;
            //        if (exp.InnerException.InnerException != null)
            //            errorMessage = exp.InnerException.InnerException.Message;
            //        else
            //            errorMessage = exp.Message;
            //        return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Exception occurred: " + errorMessage });
            //    }
            //}
            #endregion

            if (c.BatchNo == null || c.BatchNo <= 0) {
                db.SetCommandTimeOut(5);
                kst.batch_no = GetAndUpdateNextBatchNo(c.CompanyCode, c.BranchCode);
            }
            else
                kst.batch_no = c.BatchNo;

            db.KTTU_STOCK_TAKING.Add(kst);

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

            StockTakingVM stockTakingVM = new StockTakingVM();
            if (kst != null) {
                stockTakingVM.BatchNo = kst.batch_no;
                stockTakingVM.BranchCode = kst.branch_code;
                stockTakingVM.CompanyCode = kst.company_code;
                stockTakingVM.CounterCode = kst.counter_code;
                stockTakingVM.SalCode = kst.sal_code;
                stockTakingVM.Qty = kst.units;
                stockTakingVM.UpdateOn = kst.UpdateOn;
                stockTakingVM.BarcodeNo = kst.barcode_no;
                stockTakingVM.Dcts = kst.D_cts;
                stockTakingVM.GrossWt = kst.gwt;
                stockTakingVM.ItemName = kst.item_name;
                stockTakingVM.NetWt = kst.nwt;
            }
            return Ok(stockTakingVM);
        }

        [HttpDelete]
        [Route("Delete/{companyCode}/{branchCode}/{batchNo}")]
        [Route("Delete")]
        public IHttpActionResult Delete(string companyCode, string branchCode, int batchNo)
        {
            try {
                #region Ideal but not preferred for resource intensive queries since each delete is sent as a query.
                //var stockRecords = db.KTTU_STOCK_TAKING.Where(s => s.company_code == companyCode
                //                            && branchCode == s.branch_code
                //                            && s.batch_no == batchNo);
                //db.KTTU_STOCK_TAKING.RemoveRange(stockRecords);
                //db.SaveChanges(); 
                #endregion
                string sql = "DELETE  \n"
                           + "FROM   KTTU_STOCK_TAKING \n"
                           + "WHERE  company_code = @p0 \n"
                           + "       AND branch_code = @p1 \n"
                           + "       AND batch_no = @p2";
                List<object> parameterList = new List<object>();
                parameterList.Add(companyCode);
                parameterList.Add(branchCode);
                parameterList.Add(batchNo);
                object[] parametersArray = parameterList.ToArray();
                int recordsAffected = SIGlobals.Globals.ExecuteSQL(sql, db, parametersArray);
                return Ok(new ErrorVM { index = 0, field = "", description = recordsAffected.ToString() + " records affected." });
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
        [Route("DeleteBarcode/{companyCode}/{branchCode}/{batchNo}/{barcodeNo}")]
        [Route("DeleteBarcode")]
        public IHttpActionResult DeleteBarcode(string companyCode, string branchCode, int batchNo, string barcodeNo)
        {
            try {
                var barcodeToDelete = db.KTTU_STOCK_TAKING.Where(s => s.company_code == companyCode
                                            && branchCode == s.branch_code
                                            && s.batch_no == batchNo
                                            && s.barcode_no == barcodeNo).FirstOrDefault();
                if (barcodeToDelete == null) {
                    return Content(HttpStatusCode.NotFound, new ErrorVM { index = 0, field = "", description = "Barcode number is not found." });
                }
                db.KTTU_STOCK_TAKING.Remove(barcodeToDelete);
                db.SaveChanges();

                return Ok(new ErrorVM { index = 0, field = "", description = "Successfully removed." });
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
        [Route("BatchNumbers/{companyCode}/{branchCode}/{sortBy}")]
        [Route("BatchNumbers")]
        public IHttpActionResult GetBatchNumbers(string companyCode, string branchCode, string sortBy)
        {
            var batchNos = from st in db.KTTU_STOCK_TAKING
                           where st.company_code == companyCode && st.branch_code == branchCode
                           group new { st } by new
                           {
                               st.batch_no,
                               st.item_name,
                               st.counter_code,
                               st.sal_code
                           } into g
                           select new
                           {
                               BatchNo = g.Key.batch_no,
                               Item = g.Key.item_name,
                               Counter = g.Key.counter_code,
                               Salesman = g.Key.sal_code
                           };
            switch (sortBy) {
                case "Batch":
                    return Ok(batchNos.ToList().OrderBy(p => p.BatchNo));
                case "Counter":
                    return Ok(batchNos.ToList().OrderBy(p => p.Counter));
                case "Item":
                    return Ok(batchNos.ToList().OrderBy(p => p.Item));
                case "Salesman":
                    return Ok(batchNos.ToList().OrderBy(p => p.Salesman));
                default:
                    return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "sortBy should be any of these values: Batch, Item, Counter, Saleman" });
            }
        }

        /// <summary>
        /// Print Stone Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="batchNo"></param>
        /// <returns>Object of type StockTakingVM</returns>
        [HttpGet]
        [Route("StoneDetail/{companyCode}/{branchCode}/{batchNo}")]
        [Route("StoneDetail")]
        public IHttpActionResult GetStockTakingStoneDetail(string companyCode, string branchCode, int batchNo)
        {
            ErrorVM error = new ErrorVM();
            var print = new StockTakingBL().GetStockStoneDetaledReport(companyCode, branchCode, batchNo, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Print Stone  Details Total.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="batchNo"></param>
        /// <returns>Object of type StockTakingVM</returns>
        [HttpGet]
        [Route("StoneDetailTotal/{companyCode}/{branchCode}/{batchNo}")]
        [Route("StoneDetailTotal")]
        public dynamic GetStockTakingStoneDetailTotal(string companyCode, string branchCode, int batchNo)
        {
            decimal caratStandardWt = Convert.ToDecimal(0.2);
            try {
                var data = (from kst in db.KTTU_STOCK_TAKING
                            join bcm in db.KTTU_BARCODE_MASTER
                            on new
                            {
                                Company = kst.company_code,
                                Branch = kst.branch_code,
                                Barcode = kst.barcode_no
                            }
                            equals new
                            {
                                Company = bcm.company_code,
                                Branch = bcm.branch_code,
                                Barcode = bcm.barcode_no
                            }
                            join bsm in db.KTTU_BARCODE_STONE_DETAILS
                            on new
                            {
                                Company = kst.company_code,
                                Branch = kst.branch_code,
                                Barcode = kst.barcode_no
                            }
                            equals new
                            {
                                Company = bsm.company_code,
                                Branch = bsm.branch_code,
                                Barcode = bsm.barcode_no
                            } into barcodeStoneLeftJoin
                            from bsm in barcodeStoneLeftJoin.DefaultIfEmpty()
                            where kst.company_code == companyCode && kst.branch_code == branchCode && kst.batch_no == batchNo
                            group new { bsm } by new { bsm.company_code } into g
                            select new
                            {
                                TotalStoneWt = g.Where(bsm => bsm.bsm.stone_gs_type == "STN").Sum(bsm => bsm.bsm.carrat * caratStandardWt) == null ? 0 : g.Where(bsm => bsm.bsm.stone_gs_type == "STN").Sum(bsm => bsm.bsm.carrat * caratStandardWt),
                                TotalStoneCarat = g.Where(bsm => bsm.bsm.stone_gs_type == "STN").Sum(bsm => bsm.bsm.carrat) == null ? 0 : g.Where(bsm => bsm.bsm.stone_gs_type == "STN").Sum(bsm => bsm.bsm.carrat),
                                TotalDiamondWt = g.Where(bsm => bsm.bsm.stone_gs_type == "DMD").Sum(bsm => bsm.bsm.carrat * caratStandardWt) == null ? 0 : g.Where(bsm => bsm.bsm.stone_gs_type == "DMD").Sum(bsm => bsm.bsm.carrat * caratStandardWt),
                                TotalDiamondCarat = g.Where(bsm => bsm.bsm.stone_gs_type == "DMD").Sum(bsm => bsm.bsm.carrat) == null ? 0 : g.Where(bsm => bsm.bsm.stone_gs_type == "DMD").Sum(bsm => bsm.bsm.carrat),
                            }).FirstOrDefault();
                return data;
            }
            catch (Exception excp) {
                return null;
            }
        }

        [HttpGet]
        [Route("BatchSummaryReport/{companyCode}/{branchCode}/{batchNo}")]
        [Route("BatchSummaryReport")]
        public IHttpActionResult BatchSummaryReport(string companyCode, string branchCode, int batchNo)
        {
            ErrorVM error = new ErrorVM();
            var print = new StockTakingBL().GetStockTakingSummaryReport(companyCode, branchCode, batchNo, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }

            //var result = from st in db.KTTU_STOCK_TAKING
            //             where st.batch_no == batchNo && st.company_code == companyCode && st.branch_code == branchCode
            //             group st by new
            //             {
            //                 CompanyCode = st.company_code,
            //                 BranchCode = st.branch_code,
            //                 BatchNo = st.batch_no,
            //                 Item = st.item_name,
            //                 Counter = st.counter_code,
            //             } into g
            //             select new
            //             {
            //                 CompanyCode = g.Key.CompanyCode,
            //                 BranchCode = g.Key.BranchCode,
            //                 BatchNo = g.Key.BatchNo,
            //                 Item = g.Key.Item,
            //                 Counter = g.Key.Counter,
            //                 Qty = g.Sum(e => e.units),
            //                 GrossWt = g.Sum(e => e.gwt),
            //                 NetWt = g.Sum(e => e.nwt),
            //                 Dcts = g.Sum(e => e.D_cts),
            //             };
            //return Ok(result.ToList());

        }

        private int GetAndUpdateNextBatchNo(string companyCode, string branchCode)
        {
            var kstsSeqNos = db.KSTS_SEQ_NOS.Where(
                                rs => rs.company_code == companyCode
                                && rs.branch_code == branchCode
                                && rs.obj_id == "38").FirstOrDefault();
            if (kstsSeqNos == null)
                return 0;

            int nextBatchNo = kstsSeqNos.nextno;
            int batchNoPrefixedwithFinYear = Convert.ToInt32(db.KSTU_ACC_FY_MASTER.FirstOrDefault().fin_year.ToString().Remove(0, 1)
                + nextBatchNo);
            kstsSeqNos.nextno = nextBatchNo + 1;
            db.Entry(kstsSeqNos).State = System.Data.Entity.EntityState.Modified;

            return batchNoPrefixedwithFinYear;
        }

        /// <summary>
        /// List the batch details for reprinting stock taking 
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ListBatchNos/{companyCode}/{branchCode}")]
        [Route("ListBatchNos")]
        [ResponseType(typeof(List<DocumentInfoVM>))]
        public IHttpActionResult ListBatchNos(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var print = new StockTakingBL().GetStockTakingNos(companyCode, branchCode, out error);
            if (print != null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        #endregion
    }
}
