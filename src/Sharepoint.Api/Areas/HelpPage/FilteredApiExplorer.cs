using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace Sharepoint.Api.Areas.HelpPage
{
    public class FilteredApiExplorer : IApiExplorer
    {
        private readonly Collection<ApiDescription> _filteredApis;

        public FilteredApiExplorer(IEnumerable<ApiDescription> apis)
        {
            _filteredApis = new Collection<ApiDescription>(new List<ApiDescription>(apis));
        }

        public bool IsVisible(ApiDescription api) => true;

        Collection<ApiDescription> IApiExplorer.ApiDescriptions => _filteredApis;

        public IEnumerable<ApiDescription> ApiDescriptions => _filteredApis;
    }
}