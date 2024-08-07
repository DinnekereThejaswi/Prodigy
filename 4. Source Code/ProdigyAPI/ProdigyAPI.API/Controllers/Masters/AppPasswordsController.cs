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
    /// Set of APIs for managing application passwords
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/application-password")]
    public class AppPasswordsController : SIBaseApiController<ApplicationPasswordBL>
    {
        /// <summary>
        /// Gets the list of application passwords
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        [Route("list/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<ApplicationPasswordVM>))]
        public IHttpActionResult List(string companyCode, string branchCode)
        {
            ErrorVM error = null;
            var data = new ApplicationPasswordBL().List(companyCode, branchCode, out error);
            if (data != null)
                return Ok(data);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Gets an app password record
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="id">The ID of the app password</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [Route("get/{companyCode}/{branchCode}/{id}")]
        [ResponseType(typeof(ApplicationPasswordVM))]
        public IHttpActionResult Get(string companyCode, string branchCode, int id)
        {
            ErrorVM error = null;
            var role = new ApplicationPasswordBL().Get(companyCode, branchCode, id, out error);
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
        public IHttpActionResult Post(ApplicationPasswordVM item)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new ApplicationPasswordBL().Add(item, userID, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = item.PasswordNo.ToString(),
                    Message = "New record " + item.PasswordNo.ToString() + " has been created successfully."
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
        public IHttpActionResult Put(ApplicationPasswordVM item)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new ApplicationPasswordBL().Modify(item, userID, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = item.PasswordNo.ToString(),
                    Message = "The record " + item.PasswordNo.ToString() + " has been modified successfully."
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
            bool success = new ApplicationPasswordBL().Delete(companyCode, branchCode, id, authUser, out error);
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
