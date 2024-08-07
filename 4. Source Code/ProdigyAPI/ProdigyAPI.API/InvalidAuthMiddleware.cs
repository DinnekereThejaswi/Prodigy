using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;

namespace ProdigyAPI
{
    public class InvalidAuthMiddleware : OwinMiddleware
    {
        public InvalidAuthMiddleware(OwinMiddleware nextOwin): base(nextOwin)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            await Next.Invoke(context);
            if (context.Response.StatusCode == 400 && context.Response.Headers.ContainsKey("AuthorizationResponse")) {
                context.Response.Headers.Remove("AuthorizationResponse");
                context.Response.StatusCode = 401;
            }
        }
    }
}