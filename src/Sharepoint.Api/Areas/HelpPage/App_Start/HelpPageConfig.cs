// Uncomment the following to provide samples for PageResult<T>. Must also add the Microsoft.AspNet.WebApi.OData
// package to your project.
////#define Handle_PageResultOfT

using Sharepoint.Api.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
#if Handle_PageResultOfT
using System.Web.Http.OData;
#endif

namespace Sharepoint.Api.Areas.HelpPage
{
    public static class HelpPageConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Filtrar solo PdfController por nombre
            var pdfApis = config.Services.GetApiExplorer().ApiDescriptions
                .Where(api => api.ActionDescriptor.ControllerDescriptor.ControllerName == "Pdf")
                .ToList();

            // Reemplazamos ApiExplorer interno para Help Page
            config.Services.Replace(typeof(IApiExplorer), new FilteredApiExplorer(pdfApis));
        }
    }
}