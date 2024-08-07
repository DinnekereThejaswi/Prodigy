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
    /// Set of APIs for tolerance limits in the applications.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/Tolerance")]
    public class ToleranceMasterController : SIBaseApiController<ToleranceMasterBL>
    {
        /// <summary>
        /// Gets the list of tolerance limits
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        [Route("list/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ToleranceMasterVM>))]
        public IHttpActionResult List(string companyCode, string branchCode)
        {
            ErrorVM error = null;
            var data = new ToleranceMasterBL().List(companyCode, branchCode, out error);
            if (data != null)
                return Ok(data);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Gets a tolerance record
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="id">The ID of the tolerance</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [Route("get/{companyCode}/{branchCode}/{id}")]
        [ResponseType(typeof(ToleranceMasterVM))]
        public IHttpActionResult Get(string companyCode, string branchCode, int id)
        {
            ErrorVM error = null;
            var role = new ToleranceMasterBL().Get(companyCode, branchCode, id, out error);
            if (role != null)
                return Ok(role);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Posts a new record. Please note that Password should be base64 encoded.
        /// </summary>
        /// <param name="item">Object to be posted</param>
        /// <returns></returns>
        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult Post(ToleranceMasterVM item)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new ToleranceMasterBL().Add(item, userID, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = item.ID.ToString(),
                    Message = "New record " + item.ID.ToString() + " has been created successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Updates an existing record. Please note that Password should be base64 encoded.
        /// </summary>
        /// <param name="item">Object data to be modified</param>
        /// <returns></returns>
        [HttpPut]
        [Route("put")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult Put(ToleranceMasterVM item)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new ToleranceMasterBL().Modify(item, userID, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = item.ID.ToString(),
                    Message = "The record " + item.ID.ToString() + " has been modified successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Deletes a record.
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="id">The ID you want to delete.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult Delete(string companyCode, string branchCode, int id)
        {
            ErrorVM error = null;
            string authUser = base.GetUserId();
            bool success = new ToleranceMasterBL().Delete(companyCode, branchCode, id, authUser, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = id.ToString(),
                    Message = "The record " + id + " has been deleted successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

    }
}
