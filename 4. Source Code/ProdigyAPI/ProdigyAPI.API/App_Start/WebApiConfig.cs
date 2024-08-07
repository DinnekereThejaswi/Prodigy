using Newtonsoft.Json.Serialization;
using ProdigyAPI.Handlers;
using ProdigyAPI.Providers;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.OData;
using System.Web.Http.OData.Extensions;
using System.Web.Http.OData.Query;

namespace ProdigyAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();
            // Web API configuration and services
            int pageSize = 100;
            var configData = ConfigurationManager.AppSettings["PageSizeForODataFilter"];
            if (!string.IsNullOrEmpty(configData))
                pageSize = Convert.ToInt32(configData);
            config.AddODataQueryFilter(new EnableQueryAttribute
            {
                AllowedQueryOptions = AllowedQueryOptions.All,
                PageSize = pageSize
            });

            // Web API routes  
            config.MapHttpAttributeRoutes();
            //config.Routes.MapHttpRoute(
            //     "WithActionApi",
            //     "api/{controller}/{action}/{id}"
            //);
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = RouteParameter.Optional }
            );

            var json = config.Formatters.JsonFormatter;
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            
            config.MessageHandlers.Add(new RequestAndResponseLoggingHandler());
            config.Filters.Add(new SIExceptionFilterAttribute()); //custom exceptions
            if (ConfigurationManager.AppSettings["UseJWA"].ToString().ToUpper() == "TRUE") {
                config.MessageHandlers.Add(new TokenValidationHandler()); //Jwt validation
            }
        }
    }
}
