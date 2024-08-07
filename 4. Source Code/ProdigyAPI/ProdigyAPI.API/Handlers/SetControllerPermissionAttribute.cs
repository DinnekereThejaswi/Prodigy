using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace ProdigyAPI.Handlers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SetControllerPermissionAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //use these two to authorize
            //string actionName = actionContext.ActionDescriptor.ActionName;
            //string controllerName = actionContext.ControllerContext.Controller.ToString();
            return base.IsAuthorized(actionContext);
        }
    }
}