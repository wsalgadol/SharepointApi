using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Sharepoint.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            if (!config.Routes.Cast<System.Web.Http.Routing.IHttpRoute>().Any(r => r.RouteTemplate.Contains("{controller}")))
            {
                config.MapHttpAttributeRoutes();
            }

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
