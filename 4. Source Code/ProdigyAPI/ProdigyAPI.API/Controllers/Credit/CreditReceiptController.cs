using ProdigyAPI.BL.BusinessLayer.CreditReceipt;
using ProdigyAPI.BL.ViewModel.Credit;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.CreditReceipt
{
    /// <summary>
    /// This provides Credit Receipt related API's.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Credit/CreditReceipt")]
    public class CreditReceiptController : SIBaseApiController<CreditReceiptVM>, IBaseMasterActionController<CreditReceiptVM, CreditReceiptVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// Get CreditBill details 
        /// </summary>
        /// <param name="billNo"></param>
        /// <param name="finYear"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CreditBillDetails/{companyCode}/{branchCode}/{finYear}/{billNo}")]
        public IHttpActionResult GetCreditBillDetails(string companyCode, string branchCode, int finYear, int billNo)
        {
            ErrorVM error = new ErrorVM();
            CreditReceiptVM cr = new CreditReceiptBL().GetCreditBillDetails(companyCode, branchCode, finYear, billNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(cr);
        }

        /// <summary>
        /// Save Credit Receipt details.
        /// </summary>
        /// <param name="payments"></param>
        /// <param name="finYear"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post/{finYear}/{billNo}")]
        [Route("Post")]
        public IHttpActionResult Post([FromBody] List<CreditPaymentDetailsVM> payments, [FromUri] int finYear, [FromUri] int billNo)
        {
            //if (!ModelState.IsValid) {
            //    return BadRequest(ModelState);
            //}
            ErrorVM error = new ErrorVM();
            int receiptNo = new CreditReceiptBL().SaveCreditReceiptDetails(payments, finYear, billNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(new { ReceiptNo = receiptNo });
        }

        /// <summary>
        /// Get Credeit Receipt Details by Receipt Number
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CreditReceiptDetails/{companyCode}/{branchCode}/{receiptNo}")]
        [Route("CreditReceiptDetails")]
        public IHttpActionResult GetCreditReceiptDetails(string companyCode, string branchCode, int receiptNo)
        {
            ErrorVM error = new ErrorVM();
            CancelCreditReceiptVM cr = new CreditReceiptBL().GetCreditReceiptDetails(companyCode, branchCode, receiptNo, false, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(cr);
        }

        /// <summary>
        /// Credit receipt for Print.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CreditReceiptDetailsForPrint/{companyCode}/{branchCode}/{receiptNo}")]
        [Route("CreditReceiptDetailsForPrint")]
        public IHttpActionResult GetCreditReceiptDetailsForPrint(string companyCode, string branchCode, int receiptNo)
        {
            ErrorVM error = new ErrorVM();
            CancelCreditReceiptVM cr = new CreditReceiptBL().GetCreditReceiptDetailsForPrint(companyCode, branchCode, receiptNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(cr);
        }

        /// <summary>
        /// To Cancel Credit Receipt.
        /// </summary>
        /// <param name="cancelCR"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CancelCreditReceipt")]
        public IHttpActionResult CancelCreditReceipt([FromBody] CancelCreditReceiptVM cancelCR)
        {
            ErrorVM error = new ErrorVM();
            bool Cancelled = new CreditReceiptBL().CancelCreditReceipt(cancelCR, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Credit Receipt HTML Print
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Print/{companyCode}/{branchCode}/{receiptNo}")]
        [Route("Print")]
        public IHttpActionResult PrintCreditReceipt(string companyCode, string branchCode, int receiptNo)
        {
            ErrorVM error = new ErrorVM();
            BL.ViewModel.Print.ProdigyPrintVM print = new CreditReceiptBL().PrintCreditReceipt(companyCode, branchCode, receiptNo, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(print);
        }

        public IQueryable<CreditReceiptVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<CreditReceiptVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(CreditReceiptVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, CreditReceiptVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}


