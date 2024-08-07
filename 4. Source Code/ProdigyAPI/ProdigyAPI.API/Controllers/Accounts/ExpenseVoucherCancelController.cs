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
/// Date: 26-05-2021
/// </summary>
namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// This provides API's Related to Expense Voucher Cancel Module in Accounts.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/ExpenseVoucherCancel")]
    public class ExpenseVoucherCancelController : ApiController
    {
        /// <summary>
        /// Get Expanse Voucher Details by Voucher No
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="voucherNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Voucher/{companyCode}/{branchCode}/{voucherNo}")]
        [Route("Voucher")]
        public IHttpActionResult GetVoucherDetails(string companyCode, string branchCode, int voucherNo)
        {
            ErrorVM error = new ErrorVM();
            IEnumerable<TDSExpanseDetailsVM> data = new ExpenseVoucherCancelBL().GetVoucherDetails(companyCode, branchCode, voucherNo, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Cancel Voucher Entry
        /// </summary>
        /// <param name="expanse"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult CancelVoucherEntry(TDSExpenseCancelVM expanse)
        {
            ErrorVM error = new ErrorVM();
            bool cancelled = new ExpenseVoucherCancelBL().CancelVoucherDetails(expanse, out error);
            if (cancelled) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
    }
}
