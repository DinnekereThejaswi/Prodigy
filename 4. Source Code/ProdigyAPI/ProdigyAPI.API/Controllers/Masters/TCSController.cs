using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;


namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This Provides API's related to TCS Master.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/TCS")]
    public class TCSController : SIBaseApiController<TCSVM>
    {
        #region Declaration
        ErrorVM error = new ErrorVM();
        TCSBL tcsBL = TCSBL.GetInstance;
        #endregion

        #region Controller Methods

        /// <summary>
        /// This API Provides Account Names (Ledgers).
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AccountName/{companyCode}/{branchCode}")]
        [Route("AccountName")]
        public IHttpActionResult GetAccountName(string companyCode, string branchCode)
        {
            var data = tcsBL.GetAccountName(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This API Provides Calculated on.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CalOn/{companyCode}/{branchCode}")]
        [Route("CalOn")]
        public IHttpActionResult GetCalOn(string companyCode, string branchCode)
        {
            var data = tcsBL.GetCalculatedOn(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This API Provides KYC On.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("KYC/{companyCode}/{branchCode}")]
        [Route("KYC")]
        public IHttpActionResult GetKYC(string companyCode, string branchCode)
        {
            var data = tcsBL.GetKYC(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This API Provides Transaction Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TransType/{companyCode}/{branchCode}")]
        [Route("TransType")]
        public IHttpActionResult GetTransType(string companyCode, string branchCode)
        {
            var data = tcsBL.GetTransType(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This API Provides TCS Details (Grid)
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TCS/{companyCode}/{branchCode}")]
        [Route("TCS")]
        public IHttpActionResult GetTCS(string companyCode, string branchCode)
        {
            var data = tcsBL.GetTCS(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This API Provides to Save TCS Details.
        /// </summary>
        /// <param name="tcs"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Save(TCSVM tcs)
        {
            if(!ModelState.IsValid) {
                return Content(HttpStatusCode.BadRequest, base.ParseModelErrors(ModelState));
            }
            var success = tcsBL.Save(tcs, out error);
            if (success == true) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This API Provides to Update TCS Details.
        /// </summary>
        /// <param name="tcs"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{objID}")]
        [Route("Put")]
        public IHttpActionResult Update(TCSVM tcs)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var success = tcsBL.Update(tcs, out error);
            if (success == true) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        /// <summary>
        /// This API Provides to Delete TCS Master Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="isWithKyc"></param>
        /// <param name="transType"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete/{objID}/{companyCode}/{branchCode}")]
        [Route("Delete")]
        public IHttpActionResult Delete(string companyCode, string branchCode, string isWithKyc, string transType)
        {
            var success = tcsBL.Delete(companyCode, branchCode, isWithKyc, transType, out error);
            if (success == true) {
                return Ok();
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        #endregion
    }
}
