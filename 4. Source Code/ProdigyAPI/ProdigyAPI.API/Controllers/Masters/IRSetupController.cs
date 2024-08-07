using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 18th June 2021
/// </summary>
namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This Provides API's related to IR Setup Module.
    /// </summary>

    [Authorize]
    [RoutePrefix("api/IRSetup")]
    public class IRSetupController : ApiController
    {
        #region Declaration
        private readonly IRSetupBL irSetup = IRSetupBL.GetInstance;
        private ErrorVM error = new ErrorVM();
        #endregion

        /// <summary>
        /// This Method returns IR Types.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("IRTypes")]
        [Route("IRTypes/{companyCode}/{branchCode}")]
        public IHttpActionResult GetIRTypes(string companyCode, string branchCode)
        {
            var data = irSetup.GetIRTypes(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Method returns IR Types.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("IRSetup")]
        [Route("IRSetup/{companyCode}/{branchCode}/{type}")]
        public IHttpActionResult GetIRSetup(string companyCode, string branchCode, string type)
        {
            var data = irSetup.GetIRSetup(companyCode, branchCode, type, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Method returns IR Types.
        /// </summary>
        /// <param name="ir"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Save(IRSetupVM ir)
        {
            var data = irSetup.Save(ir, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This Method returns IR Types.
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="ir"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult Update(string objID, IRSetupVM ir)
        {
            var data = irSetup.Update(objID, ir, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
    }
}
