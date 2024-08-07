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
/// Date: 24/01/2020
/// </summary>
namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// Provides API's for Ledger Entry
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts/Ledger")]
    public class LedgerController : ApiController
    {
        /// <summary>
        /// Get List of All Sub Gorup Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        [Route("List")]
        public IHttpActionResult GetSubGroupList(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetLedgerMasterDetails(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get List of All Sub Gorup Details
        /// </summary>
        /// <param name="accCode"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("LedgerDetail/{companyCode}/{branchCode}/{accCode}")]
        [Route("LedgerDetail")]
        public IHttpActionResult GetSubGroupList(string companyCode, string branchCode, int accCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetLedgerMasterDetails(accCode, companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        /// <summary>
        /// Get Ledger Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("LedgerType/{companyCode}/{branchCode}")]
        [Route("LedgerType")]
        public IHttpActionResult GetLedgerType(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetLedgerType(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Sub Group
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SubGroup/{companyCode}/{branchCode}")]
        [Route("SubGroup")]
        public IHttpActionResult SubGroup(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetSubGroup(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }


        /// <summary>
        /// Get Sub Group
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="subGroupID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Group/{companyCode}/{branchCode}/{subGroupID}")]
        [Route("Group")]
        public IHttpActionResult SubGroup(string companyCode, string branchCode, int subGroupID)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetGroup(companyCode, branchCode, subGroupID, out error);
            if (error == null) {
                return Ok(new { Group = result });
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }


        /// <summary>
        /// Get Transaction Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TransactionType/{companyCode}/{branchCode}")]
        [Route("TransactionType")]
        public IHttpActionResult TransactionType(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetTransactionType(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
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
        public IHttpActionResult GSTGroupCode(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetGSTGroupCode(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
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
        /// <param name="gstGroupCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("HSNByGSTGroupCode/{companyCode}/{branchCode}/{gstGroupCode}")]
        [Route("HSNByGSTGroupCode")]
        public IHttpActionResult HSNByGSTGroupCode(string companyCode, string branchCode, int gstGroupCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetHSNByGSTGroupCode(companyCode, branchCode, gstGroupCode, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get GST Service Code (Type)
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GSTServiceGroupCode/{companyCode}/{branchCode}")]
        [Route("GSTServiceGroupCode")]
        public IHttpActionResult GSTServiceGroupCode(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetGSTServiceGroupCode(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// HSN/NAC
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("HSNNAC/{companyCode}/{branchCode}")]
        [Route("HSNNAC")]
        public IHttpActionResult HSNNAC(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetHSNNAC(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// TDS
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TDS/{companyCode}/{branchCode}")]
        [Route("TDS")]
        public IHttpActionResult TDS(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetTDS(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Schedule Types
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetScheduleType/{companyCode}/{branchCode}")]
        [Route("GetScheduleType")]
        public IHttpActionResult GetScheduleType(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetScheduleType(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Get Transaction Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("VType/{companyCode}/{branchCode}")]
        [Route("VType")]
        public IHttpActionResult AccTransactionType(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().GetAccountTransactinType(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Save Ledger Entry
        /// </summary>
        /// <param name="ledger"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveLedgerEntry([FromBody] LedgerMasterVM ledger)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().SaveLedgerDetails(ledger, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Update Ledger Entry
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="ledger"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult UpdateLedgerEntry(string objID, [FromBody] LedgerMasterVM ledger)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().UpdateLedgerDetails(objID, ledger, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// Ledger Deails HTML Print
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("LedgerDetPrint/{companyCode}/{branchCode}")]
        public IHttpActionResult LedgerDetailsPrintHTML(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var result = new LedgerBL().PrintLedgerDetailsHTML(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(result);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
    }
}
