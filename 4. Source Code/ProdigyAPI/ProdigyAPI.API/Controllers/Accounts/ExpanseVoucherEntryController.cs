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
/// Date: 2021-05-19
/// </summary>
namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// This Provides API's related to Expanse Voucher Entry Module in Accounts.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/ExpanseVoucherEntry")]
    public class ExpanseVoucherEntryController : ApiController
    {
        /// <summary>
        /// This Methods returns all Vendors.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="ledgerType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Vendor/{companyCode}/{branchCode}/{ledgerType}")]
        [Route("Vendor")]
        public IHttpActionResult GetVendorMaster(string companyCode, string branchCode, string ledgerType)
        {
            ErrorVM error = new ErrorVM();
            dynamic data = new ExpanseVoucherEntryBL().GetVendorMaster(companyCode, branchCode, ledgerType, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Ledger Details by Journal, Cash and Bank.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="ledgerType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Ledger/{companyCode}/{branchCode}/{ledgerType}")]
        [Route("Ledger")]
        public IHttpActionResult GetLedgers(string companyCode, string branchCode, string ledgerType)
        {
            ErrorVM error = new ErrorVM();
            dynamic data = new ExpanseVoucherEntryBL().GetLedgers(companyCode, branchCode, ledgerType, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Expanse Entry
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ExpanseEntry/{companyCode}/{branchCode}/{date}")]
        [Route("ExpanseEntry")]
        public IHttpActionResult GetExpanseLedgerEntry(string companyCode, string branchCode, DateTime date)
        {
            ErrorVM error = new ErrorVM();
            var data = new ExpanseVoucherEntryBL().GetExpanseLedgerEntry(companyCode, branchCode, date, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// TDS Ledger
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="supplierCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TDSLedger/{companyCode}/{branchCode}/{supplierCode?}")]
        [Route("TDSLedger")]
        public IHttpActionResult GetTDSLedger(string companyCode, string branchCode, int? supplierCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new ExpanseVoucherEntryBL().GetTDSLedger(companyCode, branchCode, supplierCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Cess Ledger
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="supplierCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CessLedger/{companyCode}/{branchCode}/{supplierCode?}")]
        [Route("CessLedger")]
        public IHttpActionResult GetCessLedger(string companyCode, string branchCode, int? supplierCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new ExpanseVoucherEntryBL().GetCessLedger(companyCode, branchCode, supplierCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get GST Group Codes
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GSTGroupCode/{companyCode}/{branchCode}")]
        [Route("GSTGroupCode")]
        public IHttpActionResult GetGSTGroupCode(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new ExpanseVoucherEntryBL().GetGSTGroupCode(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get TIN and PAN Detials by Supplier/Vendor Code
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        ///  <param name="supplierCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TINAndPAN/{companyCode}/{branchCode}/{supplierCode}")]
        [Route("TINAndPAN")]
        public IHttpActionResult GetTINandPAN(string companyCode, string branchCode, int supplierCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new ExpanseVoucherEntryBL().GetTINandPAN(companyCode, branchCode, supplierCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// To save the Expanse Ledger Entry
        /// </summary>
        /// <param name="expanse"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveExpanseLedgerEntry(List<TDSExpanseDetailsVM> expanse)
        {
            ErrorVM error = new ErrorVM();
            int expenseNo = 0;
            var data = new ExpanseVoucherEntryBL().SaveExpanseLedgerEntry(expanse, out error, out expenseNo);
            if (error == null) {
                return Ok(new { ExpenseNo = expenseNo });
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Update Expanse Ledger
        /// </summary>
        /// <param name="id"></param>
        /// <param name="expanse"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult UpdateExpanseLedgerEntry(int id, List<TDSExpanseDetailsVM> expanse)
        {
            ErrorVM error = new ErrorVM();
            var data = new ExpanseVoucherEntryBL().UpdateExpanseLedgerEntry(id, expanse, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Ledger Details by Supplier Code (Account Code)
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="supplierCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("LedgerDet/{companyCode}/{branchCode}/{supplierCode}")]
        [Route("LedgerDet")]
        public IHttpActionResult GetLedgerDet(string companyCode, string branchCode, int supplierCode)
        {
            ErrorVM error = new ErrorVM();
            var data = new ExpanseVoucherEntryBL().GetLedgerDetails(companyCode, branchCode, supplierCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
    }
}
