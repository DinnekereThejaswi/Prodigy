using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Misc;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;
namespace ProdigyAPI.Controllers
{
    [Authorize]
    public class ElevatedPermissionController : ApiController
    {
        private MagnaDbEntities dbContext = new MagnaDbEntities(true);
        [HttpGet]
        [Route("api/elevatedpermission/get")]
        public IHttpActionResult GetPermission(int permissionCode)
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            string userID = principal.Claims.Where(c => c.Type == "UserID").FirstOrDefault().Value;
            string roleIDString = principal.Claims.Where(c => c.Type == "RoleID").FirstOrDefault().Value;
            string companyCode = principal.Claims.Where(c => c.Type == "CompanyCode").FirstOrDefault().Value;
            string branchCode = principal.Claims.Where(c => c.Type == "BranchCode").FirstOrDefault().Value;
            if (string.IsNullOrEmpty(roleIDString))
                return NotFound();
            int roleID = Convert.ToInt32(roleIDString);

            try {
                var q = (from ep in dbContext.ElevatedPermissions
                         join rep in dbContext.RoleElevatedPermissionMappings
                         on ep.ID equals rep.ElevatedPermissionID
                         where rep.IsActive == true && rep.RoleID == roleID
                         select new {Permitted = true }).FirstOrDefault();
                if(q != null)
                    return Ok();
                else {
                    var p = (from ep in dbContext.ElevatedPermissions
                            join uep in dbContext.UserElevatedPermissionMappings
                            on ep.ID equals uep.ElevatedPermissionID
                            where uep.IsActive == true && uep.UserCode == userID
                             select new { Permitted = true }).FirstOrDefault();
                    if (p != null)
                        return Ok();
                    else
                        return Unauthorized();
                }
            }
            catch (Exception ex) {
                string errorMsg = ex.Message;
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, errorMsg));
            }

        }

        [HttpPost]
        [Route("api/elevatedpermission/post")]
        public IHttpActionResult GetAuthentication(AccessPermissionVM accessPermission)
        {
            string password = GlobalUtilities.Base64Decode(accessPermission.PermissionData);
            if (string.IsNullOrEmpty(password)) {
                return ThrowUnauthorizedResponse();
            }
            string hashedPassword = GlobalUtilities.GetHashcode(password);
            if (string.IsNullOrEmpty(hashedPassword)) {
                return ThrowUnauthorizedResponse();
            }

            var appPass = dbContext.KTTS_APP_PASS.Where(x => x.pass_no == accessPermission.PermissionID                
                && x.company_code == accessPermission.CompanyCode && x.branch_code == accessPermission.BranchCode).FirstOrDefault(); //TODO: Check this, branch and company code comparision is not needed after consolidation
            if(appPass == null)
                return ThrowUnauthorizedResponse();
            if(appPass.pass != hashedPassword)
                return ThrowUnauthorizedResponse();

            return Ok();
        }

        private IHttpActionResult ThrowUnauthorizedResponse()
        {
            ErrorVM error = new ErrorVM();
            error.ErrorStatusCode = HttpStatusCode.NotFound;
            error.description = "Un-Authorized";
            error.customDescription = "Un-Authorized";
            return Content(HttpStatusCode.NotFound, error);
        }
    }
}
