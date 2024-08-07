using ProdigyAPI.BL.BusinessLayer.Accounts;
using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 2017-05-17
/// </summary>
namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// Provides API's Related to Account Posting Setup Module in Accounts.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/AccountPostingSetup")]
    public class AccountPostingSetupController : ApiController
    {
        /// <summary>
        /// GS Codes
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GSCodes/{companyCode}/{branchCode}")]
        [Route("GSCodes")]
        public IHttpActionResult GetGSCodes(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new AccountPostingSetupBL().GetGS(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Transaction Types
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TransactionTypes/{companyCode}/{branchCode}")]
        [Route("TransactionTypes")]
        public IHttpActionResult GetTransactionType(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new AccountPostingSetupBL().GetTransactionTypes(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }


        /// <summary>
        /// Get Account Posting Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AllInOne/{companyCode}/{BranchCode}")]
        public IHttpActionResult GetAccountPostingSetupAll(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            IEnumerable<AccountPostingSetupVM> data = new AccountPostingSetupBL().AllInOne(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Account Posting Details by Transaction Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="transType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get")]
        public IHttpActionResult GetAccountPostingSetup(string companyCode, string branchCode, string transType)
        {
            ErrorVM error = new ErrorVM();
            IEnumerable<AccountPostingSetupVM> data = new AccountPostingSetupBL().GetAccountPostingSetup(companyCode, branchCode, transType, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }


        /// <summary>
        /// Save Account Posting Details
        /// </summary>
        /// <param name="accountPostingSetup"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveAccountPostingSetup(AccountPostingSetupVM accountPostingSetup)
        {
            ErrorVM error = new ErrorVM();
            bool saved = new AccountPostingSetupBL().SaveAccountPostingSetup(accountPostingSetup, out error);
            if (error == null && saved) {
                return Ok(saved);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Ledger Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="transType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Ledger/{companyCode}/{branchCode}/{transType}")]
        public IHttpActionResult GetLedgerDetails(string companyCode, string branchCode, string transType)
        {
            ErrorVM error = new ErrorVM();
            var data = new AccountPostingSetupBL().GetLedger(companyCode, branchCode, transType, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }

        }

        /// <summary>
        /// Get All Payment Modes
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PayMode/{companyCode}/{branchCode}")]
        [Route("PayMode")]
        public IHttpActionResult GetPayModes(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new AccountPostingSetupBL().GetPayModes(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
    }
}
