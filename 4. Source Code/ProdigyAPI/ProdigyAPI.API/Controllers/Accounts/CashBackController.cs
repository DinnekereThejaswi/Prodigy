using ProdigyAPI.BL.BusinessLayer.Accounts;
using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using System.Web.Http;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 12-Apr-2021
/// </summary>
namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// This Provides API's Related to Cash Back Module in Accounts.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts/CashBack")]
    public class CashBackController : ApiController
    {
        /// <summary>
        /// Get Sales Invoice/Bill Details by specifying Company code, Branch Code and Invoice/Bill Number
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get")]
        [Route("Get/{companyCode}/{branchCode}/{billNo}")]
        public IHttpActionResult GetBillDetails(string companyCode, string branchCode, int billNo)
        {
            ErrorVM error = new ErrorVM();
            CashBackVM cashBack = new CashBackBL().GetBillDetails(companyCode, branchCode, billNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(cashBack);
        }

        /// <summary>
        /// To save the cashback Details.
        /// </summary>
        /// <param name="cashBack"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveCashBackDetails([FromBody] CashBackTotalVM cashBack)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            ErrorVM error = new ErrorVM();
            CashBackResponseVM cashBackResponse = new CashBackResponseVM();
            bool saved = new CashBackBL().SaveCashBackDetails(cashBack, out cashBackResponse, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(cashBackResponse);
        }
    }
}
