using ProdigyAPI.BL.BusinessLayer.Accounts;
using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Handlers;
using ProdigyAPI.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// This provides API's for Cash In Hand.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts/CashInHand")]
    public class CashInHandController : ApiController
    {
        /// <summary>
        /// Get Cash in Hand
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get")]
        public IHttpActionResult GetCashInHand(string companyCode, string branchCode, DateTime date)
        {
            return Ok(new CashInHandBL().GetCashInHand(companyCode, branchCode, date));
        }

        /// <summary>
        /// post all the details
        /// </summary>
        /// <param name="cashInHand"></param>
        /// <returns></returns>
        //[PasswordStampCheckAuthorizeAttribute]
        [HttpPost]
        [Route("Post")]
        //[Route("SaveCashInHand")]
        public IHttpActionResult Post(CashInHandVM cashInHand)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool created = new CashInHandBL().Save(cashInHand, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }
    }
}
