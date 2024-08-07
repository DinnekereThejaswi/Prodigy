using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// Use these API to maintain tolerance
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/Applicatoin-password")]
    public class ApplicationPasswordController : SIBaseApiController<ApplicationPasswordVM>
    {
        ApplicationPasswordBL bl = new ApplicationPasswordBL();
        /// <summary>
        /// Gets the list of applicationpassword for the given companyCode and branchCode
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        [ResponseType(typeof(List<ApplicationPasswordVM>))]
        public IHttpActionResult List(string companyCode, string branchCode)
        {
            ErrorVM error = null;
            var data = bl.List(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);

        }

        /// <summary>
        /// Retrives a Application password
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="passwordId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [ResponseType(typeof(ApplicationPasswordVM))]
        public IHttpActionResult Get(string companyCode, string branchCode, int passwordId)
        {
            ErrorVM error = null;
            var data = bl.Get(companyCode, branchCode, passwordId, out error);
            if (data != null) {
                return Ok(data);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Posts a Application password
        /// </summary>
        /// <param name="appPassword"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("post")]
        public IHttpActionResult Post(ApplicationPasswordVM appPassword)
        {
            ErrorVM error = null;
            var result = bl.Add(appPassword, base.GetUserId(), out error);
            if (result == true) {
                return Ok();
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Updates a Application password
        /// </summary>
        /// <param name="appPassword"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("put")]
        public IHttpActionResult Put(ApplicationPasswordVM appPassword)
        {
            ErrorVM error = null;
            var result = bl.Modify(appPassword, base.GetUserId(), out error);
            if (result == true) {
                return Ok();
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Removes/deletes a Application password
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="passwordId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete/{companyCode}/{branchCode}/{passwordId}")]
        public IHttpActionResult Delete(string companyCode, string branchCode, int passwordId)
        {
            ErrorVM error = null;
            var result = bl.Delete(companyCode, branchCode, passwordId, base.GetUserId(), out error);
            if (result == true) {
                return Ok();
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }


    }
}
