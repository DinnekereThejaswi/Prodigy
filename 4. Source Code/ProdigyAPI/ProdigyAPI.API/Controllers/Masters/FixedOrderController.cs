using ProdigyAPI.BL.BusinessLayer.Order;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Query;

/// <summary>
/// Module: Fixed Order or Order Account Posting
/// Author: Mustureswara M M (Eshwar)
/// Date: 01-02-2021
/// </summary>
namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This Provides API's related to Fixed Order or Order Account Posting Module.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Masters/FixedOrder")]
    public class FixedOrderController : SIBaseApiController<FixedOrderVM>, IBaseMasterActionController<FixedOrderVM, FixedOrderVM>
    {
        #region Supplementary Methods
        /// <summary>
        /// Get Rate Types
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RateType/{companyCode}/{branchCode}")]
        [Route("RateType")]
        public IHttpActionResult GetRateType(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new OrderBL().GetRateType(companyCode, branchCode, out error);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get All Account Ledgers
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AccountLedger/{companyCode}/{branchCode}")]
        [Route("AccountLedger")]
        public IHttpActionResult GetAccountLedgers(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new OrderBL().GetLedgerNames(companyCode, branchCode, out error);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get List of Order Account Posting.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        [Route("List")]
        public IHttpActionResult GetAllOrderAccountPosting(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new OrderBL().GetAllOrderAccountPosting(companyCode, branchCode, out error);
            if (error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Save Order Account Posting Details
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveOrderAccountPosting(FixedOrderVM order)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            order.OperatorCode = base.GetUserId();
            ErrorVM error = new ErrorVM();
            bool saved = new OrderBL().SaveOrderTypeAccountPosting(order, out error);
            if (saved == true && error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Update the Order Account Posting details.
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{objID}")]
        [Route("Put")]
        public IHttpActionResult UpdateOrderAccountPosting([FromUri] string objID, [FromBody] FixedOrderVM order)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            order.OperatorCode = base.GetUserId();
            ErrorVM error = new ErrorVM();
            bool saved = new OrderBL().UpdateOrderTypeAccountPosting(objID, order, out error);
            if (saved == true && error.ErrorStatusCode == HttpStatusCode.OK) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        #endregion

        #region Default Controller Method
        public IHttpActionResult Count(ODataQueryOptions<FixedOrderVM> oDataOptions)
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

        public IQueryable<FixedOrderVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(FixedOrderVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, FixedOrderVM t)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

