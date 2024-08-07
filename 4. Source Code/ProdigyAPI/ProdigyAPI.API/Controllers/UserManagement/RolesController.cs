using ProdigyAPI.BL.BusinessLayer.UserManagement;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.UserManagement;
using ProdigyAPI.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProdigyAPI.Controllers.UserManagement
{
    /// <summary>
    /// Set of APIs for application role management.
    /// </summary>
    [RoutePrefix("api/rights-management/roles")]
    [Authorize]
    public class RolesController : SIBaseApiController<RoleBL>
    {
        /// <summary>
        /// Gets the role list
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        [Route("list/{companyCode}/{branchCode}")]
        [ResponseType(typeof(List<RoleViewModel>))]
        public IHttpActionResult GetIssueToList(string companyCode, string branchCode)
        {
            ErrorVM error = null;
            var roleList = new RoleBL().List(companyCode, branchCode, out error);
            if (roleList != null)
                return Ok(roleList);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Gets a role
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="roleId">The role ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [Route("get/{companyCode}/{branchCode}/{roleId}")]
        [ResponseType(typeof(RoleViewModel))]
        public IHttpActionResult GetUser(string companyCode, string branchCode, int roleId)
        {
            ErrorVM error = null;
            var role = new RoleBL().Get(companyCode, branchCode, roleId, out error);
            if (role != null)
                return Ok(role);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Posts a new role
        /// The allowed values for Status is one of these: Active or Closed
        /// </summary>
        /// <param name="role">Role object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult Add(RoleViewModel role)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new RoleBL().Add(role, userID, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = role.Name,
                    Message = "New role " + role.Name + " has been created successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// Updates an existing role.
        /// Only active roles can be modified.
        /// </summary>
        /// <param name="role">Role object</param>
        /// <returns></returns>
        [HttpPut]
        [Route("put")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult Update(RoleViewModel role)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            bool success = new RoleBL().Modify(role, userID, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = role.Name,
                    Message = "Role " + role.Name + " has been modified successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }
       
        /// <summary>
        /// Activate/opens a role. 
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="roleId">The Role ID which you want to update.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("activate")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult Activate(string companyCode, string branchCode, int roleId)
        {
            ErrorVM error = null;
            string authUser = base.GetUserId();
            bool success = new RoleBL().Open(companyCode, branchCode, roleId, authUser, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = roleId.ToString(),
                    Message = "The role " + roleId + " has been activated successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        /// <summary>
        /// De-Activates/closes a role. 
        /// </summary>
        /// <param name="companyCode">Company Code</param>
        /// <param name="branchCode">Branch Code</param>
        /// <param name="roleId">The Role ID which you want to update.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("de-activate")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult DeActivate(string companyCode, string branchCode, int roleId)
        {
            ErrorVM error = null;
            string authUser = base.GetUserId();
            bool success = new RoleBL().Close(companyCode, branchCode, roleId, authUser, out error);
            if (success) {
                DocumentCreationVM docCreation = new DocumentCreationVM
                {
                    DocumentNo = roleId.ToString(),
                    Message = "The role " + roleId + " has been de-activated successfully."
                };
                return Ok(docCreation);
            }
            else
                return Content(HttpStatusCode.BadRequest, error);
        }
    }
}
