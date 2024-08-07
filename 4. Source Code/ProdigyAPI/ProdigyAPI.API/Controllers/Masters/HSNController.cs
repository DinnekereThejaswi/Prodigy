using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData.Query;

/// <summary>
/// Author: Mustureswara M M (Author)
/// Date: 26/November/2020
/// </summary>
namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// Proivdes API's Related to HSN Master.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Masters/HSN")]
    public class HSNController : SIBaseApiController<HSNUCVM>, IBaseMasterActionController<HSNUCVM, HSNUCVM>
    {
        #region Supplementory Methods
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        public IHttpActionResult List(string companyCode, string branchCode)
        {
            return Ok(new HSNBL().GetHSNDetails(companyCode, branchCode));
        }

        /// <summary>
        /// Save HSN Master Details
        /// </summary>
        /// <param name="hsn"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Save(HSNUCVM hsn)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            ErrorVM error = new ErrorVM();
            bool saved = new HSNBL().Save(hsn, out error);
            if (error == null && saved == true) {
                return Ok();
            }
            else {
                return Content(System.Net.HttpStatusCode.BadRequest, error.description);
            }
        }

        /// <summary>
        /// Save HSN Master Details
        /// </summary>
        /// <param name="hsn"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Put")]
        public IHttpActionResult Update(HSNUCVM hsn)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            ErrorVM error = new ErrorVM();
            bool saved = new HSNBL().Update(hsn, out error);
            if (error == null && saved == true) {
                return Ok();
            }
            else {
                return Content(System.Net.HttpStatusCode.BadRequest, error.description);
            }
        }
        #endregion

        #region Controller Methods
        public IHttpActionResult Count(ODataQueryOptions<HSNUCVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<HSNUCVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(HSNUCVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, HSNUCVM t)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}