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
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProdigyAPI.Controllers.UserManagement
{
    /// <summary>
    /// Set of APIs for application role management.
    /// </summary>
    [RoutePrefix("api/rights-management/role-assignment")]
    [Authorize]
    public class RoleAssignmentsController : SIBaseApiController<RoleAssignmentBL>
    {
        /// <summary>
        /// Use this API to get the role assignment for existing role
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        [Route("get/{companyCode}/{branchCode}/{roleID}")]
        [ResponseType(typeof(RoleAssignmentViewModel))]
        public IHttpActionResult GetRoleAssignment(string companyCode, string branchCode, int roleID)
        {
            ErrorVM error = null;
            var data = new RoleAssignmentBL().GetRoleAssignment(companyCode, branchCode, roleID, out error);
            if (data != null)
                return Ok(data);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        /// <summary>
        /// Use this API to intialize the data for creating new roles.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("initialize-role")]
        [Route("initialize-role/{companyCode}/{branchCode}")]
        [ResponseType(typeof(RoleAssignmentViewModel))]
        public IHttpActionResult FillRoleData(string companyCode, string branchCode)
        {
            ErrorVM error = null;
            var data = new RoleAssignmentBL().GetRoleAssignment(companyCode, branchCode, 0, out error);
            if (data != null)
                return Ok(data);
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }

        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(DocumentCreationVM))]
        public IHttpActionResult Post(RoleAssignmentViewModel roleAssignment)
        {
            ErrorVM error = null;
            string userID = base.GetUserId();
            var result = new RoleAssignmentBL().Post(roleAssignment, userID, out error);
            if (result == true) {
                DocumentCreationVM doc = new DocumentCreationVM
                {
                    DocumentNo = roleAssignment.RoleID.ToString(),
                    Message = "Role permissions updated succesfully for roleID" + roleAssignment.RoleID.ToString()
                };
                return Ok(doc);
            }
            else {
                return Content(HttpStatusCode.BadRequest, error);
            }
        }
    }
}
