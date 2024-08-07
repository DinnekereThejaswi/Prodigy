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
/// Author: Eshwar
/// Date: 30/01/2020
/// </summary>
namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// This provides API's related to Journal Voucher Entry.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts/JournalEntry")]
    public class JournalVoucherEntryController : ApiController
    {
        /// <summary>
        /// Get Account Names
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AccountName/{companyCode}/{branchCode}")]
        [Route("AccountName")]
        public IHttpActionResult GetAccountName(string companyCode, string branchCode)
        {
            return Ok(new JournalVoucherEntryBL().GetAccountName(companyCode, branchCode));
        }

        /// <summary>
        /// Get Voucher Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="voucherNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}/{voucherNo}")]
        public IHttpActionResult GetVoucherDet(string companyCode, string branchCode, int voucherNo)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            List<AccVoucherTransactionsVM> voutcherDet = new JournalVoucherEntryBL().GetJournalVoucherDetails(companyCode, branchCode, voucherNo, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(voutcherDet);
        }

        /// <summary>
        /// Get Voucher Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PrintList/{companyCode}/{branchCode}/{date}")]
        public IHttpActionResult GetVoucherDetForPrint(string companyCode, string branchCode, DateTime date)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            List<AccVoucherTransactionsVM> voutcherDet = new JournalVoucherEntryBL().GetJournalVoucherDetails(companyCode, branchCode, date, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(voutcherDet);
        }

        /// <summary>
        /// Save Journal Entry Detials
        /// </summary>
        /// <param name="voucherDet"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveJournalEntry([FromBody] List<AccVoucherTransactionsVM> voucherDet)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            AccVoucherTransactionsVM created = new JournalVoucherEntryBL().SaveJournalVoucherEntry(voucherDet, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(created);
        }

        /// <summary>
        /// Delete the Voucher Details
        /// </summary>
        /// <param name="voucherDet"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete")]
        public IHttpActionResult DeleteVoucherDetails([FromBody] List<AccVoucherTransactionsVM> voucherDet)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool created = new AccountsCommonBL().DeleteVourcherDetails(voucherDet, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Print Journal Voucher Entry
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="voucherNo"></param>
        /// <param name="accCodeMaster"></param>
        /// <param name="tranType"></param>
        /// <param name="accType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Print/{companyCode}/{branchCode}/{voucherNo}/{accCodeMaster}/{transType}/{accType}")]
        [Route("Print")]
        public IHttpActionResult Print(string companyCode, string branchCode, int voucherNo, int accCodeMaster, string tranType, string accType)
        {
            ErrorVM error = new ErrorVM();
            string report = new JournalVoucherEntryBL().Print(companyCode, branchCode, voucherNo, accType, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(report);
        }
    }
}
