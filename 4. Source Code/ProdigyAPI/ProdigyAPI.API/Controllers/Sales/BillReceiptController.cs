using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.Handlers;
using System.Web.Http;
using System;
using System.Linq;
using System.Web.Http.OData.Query;
using System.Collections.Generic;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.BusinessLayer.Sales;
using ProdigyAPI.BL.ViewModel.Sales;

namespace ProdigyAPI.Controllers.Sales
{
    /// <summary>
    /// This Provides API's Related to Bill Receipt Module.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/BillReceipt")]
    public class BillReceiptController : SIBaseApiController<PaymentVM>, IBaseMasterActionController<PaymentVM, PaymentVM>
    {
        #region Default Controller Methods
        public IHttpActionResult Count(ODataQueryOptions<PaymentVM> oDataOptions)
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

        public IQueryable<PaymentVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(PaymentVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, PaymentVM t)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Supplymentary Controller Methods
        /// <summary>
        /// Save Bill Receipt Details
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        [Route("Post")]
        public IHttpActionResult SaveBillReceiptDet(BillReceiptVM payments)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool saved = new BillReceiptBL().SaveBillReceiptDetails(payments, out error);
            if (saved) {
                return Ok();
            }
            else {
                return Content(System.Net.HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Get All Payment modes related to Billed Receitps
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [Route("PayModes/{companyCode}/{branchCode}")]
        public IHttpActionResult GetBilledReceiptPayModes(string companyCode, string branchCode)
        {
            return Ok(new BillReceiptBL().GetBillReceiptPayModes(companyCode, branchCode));
        }
        #endregion
    }
}