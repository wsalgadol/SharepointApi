using Sharepoint.Aplications.Interfaces;
using Sharepoint.Aplications.Services;
using Sharepoint.Logs.Interfaces;
using Sharepoint.Logs.Services;
using System.Web.Http;
using System.Web.Mvc;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;

namespace Sharepoint.Api.App_Start
{
    public static class UnityConfig
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            // Resolver de WebApi
            GlobalConfiguration.Configuration.DependencyResolver =
                new UnityDependencyResolver(container);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<ILoggerService, NLogLoggerService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPdfConversionService, PdfConversionService>(new HierarchicalLifetimeManager());
            container.RegisterType<IFileService, FileService>(new HierarchicalLifetimeManager());
            container.RegisterType<IFolderService, FolderService>(new HierarchicalLifetimeManager());

            return container;
        }
    }
}