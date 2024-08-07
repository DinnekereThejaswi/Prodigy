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
    /// This Provides API's for Cheque Master.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts/Cheque")]
    public class ChequeController : ApiController
    {
        #region Check Entry
        /// <summary>
        /// Get All List of Cheques.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        [Route("List")]
        public IHttpActionResult List(string companyCode, string branchCode)
        {
            //return Ok(new ChequeBL().List(companyCode, branchCode));
            var data = new ChequeBL().List(companyCode, branchCode);
            return Ok(data);
        }

        /// <summary>
        /// post all the details
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Post([FromBody] ChequeVM v)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool created = new ChequeBL().Save(v, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// post all the details
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete")]
        public IHttpActionResult Delete(ChequeVM v)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool created = new ChequeBL().Delete(v, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        #endregion

        #region Check Closing
        /// <summary>
        /// Get All List of Banks for Cheque Closing
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Bank/{companyCode}/{branchCode}")]
        [Route("Bank")]
        public IHttpActionResult GetCheckClosingBankList(string companyCode, string branchCode)
        {
            return Ok(new ChequeBL().GetCheckClosingBank(companyCode, branchCode));
        }

        /// <summary>
        /// Get All List of Banks for Cheque Closing
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="accCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ChequeList/{companyCode}/{branchCode}/{accCode}")]
        [Route("ChequeList")]
        public IHttpActionResult GetCheckList(string companyCode, string branchCode, int accCode)
        {
            return Ok(new ChequeBL().GetCheckList(companyCode, branchCode, accCode));
        }

        /// <summary>
        /// post all the details
        /// </summary>
        /// <param name="cheque"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OpenClose")]
        public IHttpActionResult OpenClose(ChequeVM cheque)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool deleted = new ChequeBL().OpenOrCloseCheque(cheque, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }
        #endregion
    }
}
