using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using ProdigyAPI.Model.MagnaDb;
using System.Web;

namespace ProdigyAPI.Providers
{
    public class PasswordStampCheckAuthorizeAttribute : AuthorizeAttribute
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities(true);
        #endregion
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var claimsIdentity = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity == null) {
                this.HandleUnauthorizedRequest(actionContext);
            }

            // Check if the password has been changed. If it is, this token should not be accepted any more.
            // We generate a GUID stamp upon registration and every password change, and put it in every token issued.
            var passwordTokenClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "PwdStamp");
            var passwordTimestampClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "RowTimestamp");
            if (passwordTokenClaim == null) {
                // There was no stamp in the token.
                this.HandleUnauthorizedRequest(actionContext);
            }
            else {
                string userID = claimsIdentity.Claims.Where(c => c.Type == "UserID").FirstOrDefault().Value;
                using (var dbContext = new MagnaDbEntities()) {
                    if (dbContext.SDTU_OPERATOR_MASTER.First(u => u.OperatorCode == userID).RowTimestamp.ToString() != passwordTimestampClaim.Value) {
                        this.HandleUnauthorizedRequest(actionContext);
                    }
                }
            }

            base.OnAuthorization(actionContext);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            string errorMsg = "{\"Message\": \"You don't have permission to access this resource. Unauthorized access.\"}";
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
                Content = new StringContent(errorMsg, System.Text.Encoding.UTF8, "application/json")
            };
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try {
                if (HttpContext.Current.User.Identity.IsAuthenticated) {
                    var claimsIdentity = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
                    var claimRoleID = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "RoleID");
                    var claimCompanyCode = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "CompanyCode");
                    var claimBranchCode = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "BranchCode");

                    int roleID = Convert.ToInt32(claimRoleID.Value);
                    string companyCode = Convert.ToString(claimCompanyCode.Value);
                    string branchCode = Convert.ToString(claimBranchCode.Value);

                    string originalHost = "http://localhost:80";
                    string url = actionContext.Request.RequestUri.OriginalString;
                    string removedHost = url.Remove(0, originalHost.Length);

                    // Removing not required things from URL.
                    string removeHostedPath = removedHost.Remove(0, actionContext.ActionDescriptor.Configuration.VirtualPathRoot.Length + 1);

                    // Removing route parameters
                    string routePrfixWithRoute = string.Empty;
                    if (actionContext.ControllerContext.RouteData.Values.Count > 0) {
                        IDictionary<string, object> routeData = actionContext.ControllerContext.RouteData.Values;
                        foreach (var item in routeData) {
                            removeHostedPath = removeHostedPath.Replace("/" + item.Value.ToString(), "");
                        }
                    }
                    routePrfixWithRoute = removeHostedPath;
                    string getActionName = routePrfixWithRoute.Substring(routePrfixWithRoute.LastIndexOf('/'), (routePrfixWithRoute.Length - routePrfixWithRoute.LastIndexOf('/')));
                    string routePrefix = routePrfixWithRoute.Replace(getActionName, "");
                    string method = string.Empty;

                    // Removing Query Parameters there incase.
                    if (actionContext.Request.RequestUri.Query != "") {
                        method = getActionName.Replace(actionContext.Request.RequestUri.Query, "").Replace("/", "");
                    }
                    else {
                        method = getActionName.Replace("/", "");
                    }
                    var permission = (from p in db.RoleMethodPermissions
                                      join m in db.Methods on p.MethodID equals m.ID
                                      where p.CompanyCode == companyCode && p.BranchCode == branchCode && p.RoleID == roleID
                                            && m.RoutePrefix == routePrefix && m.Route == method
                                      select new
                                      {
                                          IsEnabled = p.IsEnabled
                                      }).FirstOrDefault();
                    if (permission == null)
                        return true;

                    if (permission.IsEnabled) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                return HandledAuthorizedError(actionContext);
            }
            catch (Exception excp) {
                return false;
            }
        }

        private bool HandledAuthorizedError(HttpActionContext actionContext)
        {
            string errorMsg = "{\"Message\": \"You are not allowed to access this resource.\"}";
            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
                Content = new StringContent(errorMsg, System.Text.Encoding.UTF8, "application/json")
            };
            return false;
        }

        //protected override bool IsAuthorized(HttpActionContext actionContext)
        //{
        //    bool isAuthorized = true;
        //    if (isAuthorized)
        //        return base.IsAuthorized(actionContext);
        //    else
        //        return false;
        //}
    }
}