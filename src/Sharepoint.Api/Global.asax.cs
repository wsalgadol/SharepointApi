using Sharepoint.Api.App_Start;
using Sharepoint.Api.Areas.HelpPage;
using Sharepoint.Logs.Services;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Sharepoint.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Registrar Web API routes primero
            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Registrar Unity DI
            UnityConfig.Initialise();

            // Registrar Help Page **después** de que los controladores existan
            HelpPageConfig.Register(GlobalConfiguration.Configuration);
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            var logger = new NLogLoggerService();

            logger.Error("Error no controlado en la API", exception);
        }
    }
}
