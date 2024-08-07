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
/// Date: 27/01/2020
/// </summary>
namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// This Provides API's realted to Contra Entry in Accounts Module.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts/ContraEntry")]
    public class ContraEntryController : ApiController
    {
        /// <summary>
        /// Get Type of Transactions.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Type/{companyCode}/{branchCode}")]
        [Route("Type")]
        public IHttpActionResult GetType(string companyCode, string branchCode)
        {
            return Ok(new ContraEntryBL().GetType(companyCode, branchCode));
        }

        /// <summary>
        /// Get Master Ledger Details By Transaction Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("MasterLedger/{companyCode}/{branchCode}/{type}")]
        [Route("MasterLedger")]
        public IHttpActionResult GetMasterLedger(string companyCode, string branchCode, string type)
        {
            return Ok(new ContraEntryBL().GetMasterLedger(companyCode, branchCode, type));
        }

        /// <summary>
        /// Get List of AccountName
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AccountName/{companyCode}/{branchCode}")]
        [Route("AccountName")]
        public IHttpActionResult GetAccountName(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var  accountName = new ContraEntryBL().GetListOfAccountName(companyCode, branchCode, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(accountName);
        }

        /// <summary>
        /// Get List of all Entry
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="accCodeMaster"></param>
        /// <param name="voutcherDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}/{accCodeMaster}/{voutcherDate}")]
        [Route("List")]
        public IHttpActionResult GetListOfEntry(string companyCode, string branchCode, int accCodeMaster, DateTime voutcherDate)
        {
            return Ok(new ContraEntryBL().GetListOfEntry(companyCode, branchCode, accCodeMaster, voutcherDate));
        }

        /// <summary>
        /// Get List of all Entry
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="voutcherDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PrintList/{companyCode}/{branchCode}/{voutcherDate}")]
        [Route("PrintList")]
        public IHttpActionResult GetListOfEntryPrint(string companyCode, string branchCode, DateTime voutcherDate)
        {
            return Ok(new ContraEntryBL().GetListOfEntry(companyCode, branchCode, voutcherDate));
        }

        /// <summary>
        /// Save Account Votucher Details
        /// </summary>
        /// <param name="voucherDet"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveContraVoucherDetails([FromBody] AccVoucherTransactionsVM voucherDet)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            AccVoucherTransactionsVM createdAccVM = new ContraEntryBL().SaveVourcherDetails(voucherDet, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(createdAccVM);
        }

        /// <summary>
        /// Get Master Ledger Details By Transaction Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="voucherNo"></param>
        /// <param name="accCode"></param>
        /// <param name="accCodeMaster"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Voucher/{companyCode}/{branchCode}/{voucherNo}/{accCode}/{accCodeMaster}")]
        [Route("Voucher")]
        public IHttpActionResult GetVoucherDetails(string companyCode, string branchCode, int voucherNo, int accCode, int accCodeMaster)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            AccVoucherTransactionsVM voutcherDet = new ContraEntryBL().GetVoucherDetails(companyCode, branchCode, voucherNo, accCode, accCodeMaster, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(voutcherDet);
        }

        /// <summary>
        /// Update Contra Entry Voucher
        /// </summary>
        /// <param name="voucherNo"></param>
        /// <param name="voucherDet"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult Put(int voucherNo, [FromBody] AccVoucherTransactionsVM voucherDet)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool created = new ContraEntryBL().UpdateVourcherDetails(voucherNo, voucherDet, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Delete the Voucher Details
        /// </summary>
        /// <param name="voucherDet"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete")]
        public IHttpActionResult DeleteVoucherDetails([FromBody] AccVoucherTransactionsVM voucherDet)
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
        /// Print Contra Voucher
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
            string report = new AccountsCommonBL().Print(companyCode, branchCode, voucherNo, accCodeMaster, accType, tranType, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(report);
        }
    }
}
