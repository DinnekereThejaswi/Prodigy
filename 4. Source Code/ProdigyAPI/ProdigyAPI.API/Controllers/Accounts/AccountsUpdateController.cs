using ProdigyAPI.BL.BusinessLayer.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// This provides API for Final Account Update.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts/AccountsUpdate")]
    public class AccountsUpdateController : ApiController
    {
        /// <summary>
        /// Final Account update
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Update")]
        public IHttpActionResult FinalAccountUpdated(string companyCode, string branchCode, DateTime startDate, DateTime endDate)
        {
            ErrorVM error = new ErrorVM();
            bool saved = new AccountsUpdateBL().FinalUpdate(companyCode, branchCode, startDate, endDate, out error);
            if (saved) {
                return Ok();
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
    }
}
