using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ProdigyAPI.Providers;
using System.Configuration;

[assembly: OwinStartup(typeof(ProdigyAPI.Startup))]
namespace ProdigyAPI
{
    public partial class Startup
    {            
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            ConfigureAuthorization(app);
            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Resolve Entity framework self referencing loop detected
            //Credit to: https://stackoverflow.com/questions/19467673/entity-framework-self-referencing-loop-detected
            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling 
                = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        public void ConfigureAuthorization(IAppBuilder app)
        {
            app.Use<InvalidAuthMiddleware>();
            double accessTokenLifespanSeconds = Convert.ToDouble(ConfigurationManager.AppSettings["AccessTokenLifespanSeconds"]);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            if (ConfigurationManager.AppSettings["UseJWA"].ToString().ToUpper() != "TRUE") {
                var OAuthOptions = new OAuthAuthorizationServerOptions
                {
                    AllowInsecureHttp = true,
                    TokenEndpointPath = new PathString("/Auth/token"),
                    AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(accessTokenLifespanSeconds),
                    Provider = new AccessTokenBasedAuthProvider(),
                    RefreshTokenProvider = new OAuthRefreshTokenProvider(),  //for refresh tokens                
                };

                app.UseOAuthBearerTokens(OAuthOptions);
                app.UseOAuthAuthorizationServer(OAuthOptions);
                app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            }

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
        }

        
    }
}