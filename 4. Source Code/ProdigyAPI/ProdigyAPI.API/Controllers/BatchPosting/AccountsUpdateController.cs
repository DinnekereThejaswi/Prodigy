using ProdigyAPI.BL.BusinessLayer.BatchPosting;
using ProdigyAPI.BL.ViewModel.BatchPosting;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Handlers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;


namespace ProdigyAPI.Controllers.BatchPosting
{
    /// <summary>
    /// Provides API for accounts update
    /// </summary>
    [Authorize]
    [RoutePrefix("api/batch-posting/accounts-update")]
    public class AccountsUpdateController : SIBaseApiController<ChitUpdateBL>
    {
        /// <summary>
        /// Post the account updates
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="txnDate">Date of the transaction</param>
        /// <returns></returns>
        [HttpPost]
        [Route("accounts-update")]
        [Route("accounts-update/{companyCode}/{branchCode}/{txnDate}")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult UpdateAccounts(string companyCode, string branchCode, DateTime txnDate)
        {
            string errorMessage = null;
            string userID = base.GetUserId();
            var result = new AccountsUpdateBL().Post(companyCode, branchCode, txnDate, false, false, userID, out errorMessage);
            if (result == true) {
                var docCreation = new DocumentCreationVM { DocumentNo = "Accounts Updation", Message = "Accounts updated successfully" };
                return Ok(docCreation);
            }
            else {

                return Content(HttpStatusCode.BadRequest, new ErrorVM { description = "Failed to update accounts. Error: " + errorMessage });
            }
        }
    }
}
