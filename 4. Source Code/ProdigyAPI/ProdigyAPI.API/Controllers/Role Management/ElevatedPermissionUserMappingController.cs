using ProdigyAPI.BL.BusinessLayer.AccessManagement;
using ProdigyAPI.BL.ViewModel.AccessManagement;
using ProdigyAPI.BL.ViewModel.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProdigyAPI.Controllers.Role_Management
{
    [Authorize]
    [RoutePrefix("api/permission-user-map")]
    public class ElevatedPermissionUserMappingController : ApiController
    {
        [HttpGet]
        [Route("Permissions")]
        public IHttpActionResult GetPermissions()
        {
            return Ok(new RoleBasedAccessManagementBL().GetPermissions());
        }

        [HttpGet]
        [Route("Users")]
        public IHttpActionResult GetUsers(string companyCode, string branchCode)
        {
            return Ok(new UserBasedAccessManagementBL().GetUsers(companyCode, branchCode));
        }

        [HttpGet]
        [Route("List")]
        [ResponseType(typeof(ElevatedPermissionUserMap))]
        public IHttpActionResult List(string companyCode, string branchCode)
        {
            return Ok(new UserBasedAccessManagementBL().List(companyCode, branchCode));
        }

        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Post(ElevatedPermissionUserMap elevatedPermissionUserMap)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error;
            bool success = new UserBasedAccessManagementBL().Post(elevatedPermissionUserMap, GetLoggedInUser(), out error);
            if (success)
                return Ok();
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        [HttpPut]
        [Route("Activate")]
        public IHttpActionResult Activate(ElevatedPermissionUserMap elevatedPermissionUserMap)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error;
            bool success = new UserBasedAccessManagementBL().Activate(elevatedPermissionUserMap, GetLoggedInUser(), out error);
            if (success)
                return Ok();
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        [HttpPut]
        [Route("De-activate")]
        public IHttpActionResult DeActivate(ElevatedPermissionUserMap elevatedPermissionUserMap)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error;
            bool success = new UserBasedAccessManagementBL().DeActivate(elevatedPermissionUserMap, GetLoggedInUser(), out error);
            if (success)
                return Ok();
            else
                return Content(HttpStatusCode.BadRequest, error);
        }

        private string GetLoggedInUser()
        {
            string userId = "--";
            try {
                var claimsIdentity = RequestContext.Principal.Identity as ClaimsIdentity;
                if (claimsIdentity == null) {
                    return userId;
                }
                var userIdClaim = claimsIdentity.Claims.Where(c => c.Type == "UserID").FirstOrDefault();
                if (userIdClaim != null)
                    userId = userIdClaim.Value;
            }
            catch (Exception) {
            }
            return userId;
        }
    }
}
