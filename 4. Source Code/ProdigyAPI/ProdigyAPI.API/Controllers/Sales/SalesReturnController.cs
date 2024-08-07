using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.OData.Query;
using ProdigyAPI.BL.BusinessLayer.Sales;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.BusinessLayer;

namespace ProdigyAPI.Controllers.Sales
{
    /// <summary>
    /// Sales Return controller provieds API's for Sales Return.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/SalesReturn")]
    public class SalesReturnController : SIBaseApiController<SalesReturnMasterVM>, IBaseMasterActionController<SalesReturnMasterVM, SalesReturnMasterVM>
    {

        #region Controller Methods
        /// <summary>
        /// Get sales return information by Estimation No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="estNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}/{estNo}")]
        public IHttpActionResult GetSalesReturn(string companyCode, string branchCode, int estNo)
        {
            ErrorVM error = new ErrorVM();
            SalesReturnMasterVM smv = new ConfirmSalesReturnBL().GetSalesReturn(companyCode, branchCode, estNo, true, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(smv);
        }

        /// <summary>
        /// Get sales return information by SR Bill No.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="srBillNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDetByBillNo/{companyCode}/{branchCode}/{srBillNo}")]
        public IHttpActionResult GetSalesReturnByBillNo(string companyCode, string branchCode, int srBillNo)
        {
            ErrorVM error = new ErrorVM();
            SalesReturnMasterVM smv = new ConfirmSalesReturnBL().GetSalesReturn(companyCode, branchCode, srBillNo, false, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(smv);
        }

        /// <summary>
        /// Save sales return informatioin.
        /// </summary>
        /// <param name="salesReturn"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveSalesReturn(ConfirmSalesReturnVM salesReturn)
        {
            ErrorVM error = new ErrorVM();
            ConfirmVM confirm = new ConfirmSalesReturnBL().ConfirmSalesReturnWithTxn(salesReturn, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(confirm);
        }

        /// <summary>
        /// Print sales return bill
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="srBillNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Print/{companyCode}/{branchCode}/{srBillNo}")]
        public IHttpActionResult PrintSalesReturnBill(string companyCode, string branchCode, int srBillNo)
        {
            ErrorVM error = new ErrorVM();
            BL.ViewModel.Print.ProdigyPrintVM print = new ConfirmSalesReturnBL().PrintSRBill(companyCode, branchCode, srBillNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(print);
        }

        /// <summary>
        /// Cancel SR bill
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CancelSRBill")]
        public IHttpActionResult CancelPurchaseBill(ConfirmSalesReturnVM sr)
        {
            ErrorVM error = new ErrorVM();
            bool cancelled = new ConfirmSalesReturnBL().CancelSRBill(sr, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok();
        }

        /// <summary>
        /// Get Adjusted Bills for the day.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AdjustedSRBill/{companyCode}/{branchCode}")]
        [Route("AdjustedSRBill")]
        public IHttpActionResult GetAdjustedPurchaseBills(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            dynamic data = new ConfirmSalesReturnBL().GetAdjustedSRBill(companyCode, branchCode, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(data);
        }

        /// <summary>
        /// Get Search parameters for Adjust SR Bills
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchParams")]
        public IHttpActionResult GetAllSearchParams()
        {
            List<SearchParamVM> lstSearchParams = new List<SearchParamVM>();
            lstSearchParams.Add(new SearchParamVM() { Key = "SRNO", Value = "SR No" });
            lstSearchParams.Add(new SearchParamVM() { Key = "DATE", Value = "Date" });
            lstSearchParams.Add(new SearchParamVM() { Key = "AMOUNT", Value = "Amount" });
            return Ok(lstSearchParams);
        }

        /// <summary>
        /// Get search results by sending search parameters.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="searchType"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Search/{companyCode}/{branchCode}/{searchType}/{searchValue}")]
        [Route("Search")]
        public dynamic GetAllSearchDetails(string companyCode, string branchCode, string searchType, string searchValue)
        {
            dynamic result = new ConfirmSalesReturnBL().GetAllSearchResult(companyCode, branchCode, searchType, searchValue);
            return result;
        }

        /// <summary>
        /// Dotmatrix print of Sales Return Bill No
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="srBillNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DotMatrixPrint/{companyCode}/{branchCode}/{srBillNo}")]
        public IHttpActionResult SRBillDotMatrixPrint(string companyCode, string branchCode, int srBillNo)
        {
            ErrorVM error = new ErrorVM();
            string print = new ConfirmSalesReturnBL().SRBillDotMatrixPrint(companyCode, branchCode, srBillNo, out error);
            if (error == null) {
                return Ok(print);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        public IQueryable<SalesReturnMasterVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(SalesReturnMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, SalesReturnMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<SalesReturnMasterVM> oDataOptions)
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
        #endregion
    }
}