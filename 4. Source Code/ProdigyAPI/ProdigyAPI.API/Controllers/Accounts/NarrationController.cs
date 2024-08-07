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
/// Date: 23/01/2020
/// </summary>
namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// This Provides API's for Narration
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts/Narration")]
    public class NarrationController : ApiController
    {
        /// <summary>
        /// Get All Narration Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        [Route("List")]
        public IQueryable GetNarrationList(string companyCode, string branchCode)
        {
            return new NarrationBL().GetAllNarrationList(companyCode, branchCode);
        }

        /// <summary>
        /// Save Narration Details
        /// </summary>
        /// <param name="narration"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveNarrationDetails([FromBody] NarrationVM narration)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool created = new NarrationBL().SaveNarrationDetails(narration, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Save Narration Details
        /// </summary>
        /// <param name="narration"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete")]
        public IHttpActionResult DeleteNarrationDetails([FromBody] NarrationVM narration)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool created = new NarrationBL().DeleteNarrationDetails(narration, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }
    }
}
