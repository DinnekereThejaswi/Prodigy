using ProdigyAPI.BL.BusinessLayer.Accounts;
using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Handlers;
using System.Linq;
using System.Web.Http;
using System;
using System.Web.Http.OData.Query;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 20th April 2021
/// Module Type: Controller
/// </summary>
namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// Provides API's Releated to Account Code Settings Module in Accounts.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/AccountCodeSettings")]
    public class AccountCodeSettingsController : SIBaseApiController<AccountCodeMasterVM>, IBaseMasterActionController<AccountCodeMasterVM, AccountCodeMasterVM>
    {
        /// <summary>
        /// Get List of All Account Ledger Master.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AccountLedger/{companyCode}/{branchCode}")]
        [Route("AccountLedger")]
        public IHttpActionResult GetAccountLedger([FromUri] string companyCode, [FromUri] string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new AccountCodeSettingBL().GetAccountLedger(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Method Provides List of all Account Code Master Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        [Route("List")]
        public IQueryable<AccountCodeMasterVM> GetAccountCodeMaster([FromUri] string companyCode, [FromUri] string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new AccountCodeSettingBL().GetAccountCodeMaster(companyCode, branchCode, out error);
            return data.AsQueryable();
            //if (error == null) {
            //    return Ok(data.AsEnumerable<AccountCodeMasterVM>());
            //}
            //else {
            //    return Content(error.ErrorStatusCode, error);
            //}
        }

        /// <summary>
        /// This Method Provides List of all Account Code Master Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Count/{companyCode}/{branchCode}")]
        [Route("Count")]
        public dynamic GetRecordCount([FromUri] string companyCode, [FromUri] string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new AccountCodeSettingBL().GetAccountCodeMaster(companyCode, branchCode, out error);
            return Ok(new { RecordCount = data.ToList().Count() });
        }

        /// <summary>
        /// This Method Provides to Save the Account Code Master Details.
        /// </summary>
        /// <param name="accCodeMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveAccountCode([FromBody] AccountCodeMasterVM accCodeMaster)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            ErrorVM error = new ErrorVM();
            var data = new AccountCodeSettingBL().SaveAccountCodeMaster(accCodeMaster, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Method Provides to Update the Account Code Master Details.
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="accCodeMaster"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult UpdateAccountCode([FromUri] int objID, [FromBody] AccountCodeMasterVM accCodeMaster)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            ErrorVM error = new ErrorVM();
            var data = new AccountCodeSettingBL().UpdateAccountCodeMaster(objID, accCodeMaster, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        public IQueryable<AccountCodeMasterVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<AccountCodeMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(AccountCodeMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, AccountCodeMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }
    }
}
