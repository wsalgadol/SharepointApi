using Microsoft.SharePoint.Client;
using Sharepoint.Aplications.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;

namespace Sharepoint.Aplications.Services
{
    public class FolderService : IFolderService
    {
        private static readonly ConcurrentDictionary<string, string> _webCache
            = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public void EnsureFolder(string absoluteUrl)
        {
            var (siteCollectionUrl, serverRelativeFolder) = ParseFolderUrl(absoluteUrl);
            using (ClientContext context = new ClientContext(siteCollectionUrl))
            {
                context.Load(context.Web, w => w.ServerRelativeUrl);
                context.ExecuteQuery();

                string webRoot = context.Web.ServerRelativeUrl.TrimEnd('/');

                string relative = serverRelativeFolder
                    .Substring(webRoot.Length)
                    .Trim('/');

                var parts = relative.Split('/');

                string current = webRoot;

                foreach (var part in parts)
                {
                    current += "/" + part;

                    try
                    {
                        var folder = context.Web.GetFolderByServerRelativeUrl(current);
                        context.Load(folder);
                        context.ExecuteQuery();
                    }
                    catch
                    {
                        var parent = context.Web.GetFolderByServerRelativeUrl(
                            current.Substring(0, current.LastIndexOf('/'))
                        );

                        parent.Folders.Add(part);
                        context.ExecuteQuery();
                    }
                }
            }                
        }


        public (string siteCollectionUrl, string serverRelativeFolder) ParseFolderUrl(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                throw new ArgumentNullException(nameof(fileUrl));

            var uri = new Uri(fileUrl);

            string host = $"{uri.Scheme}://{uri.Host}";
            string path = uri.AbsolutePath;

            var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // 🔹 cache key = host + primer segmento
            string cacheKey = host + "/" + (segments.Length > 0 ? segments[0] : "");

            if (!_webCache.TryGetValue(cacheKey, out string detectedWeb))
            {
                detectedWeb = DetectWeb(host, segments);
                _webCache.TryAdd(cacheKey, detectedWeb);
            }

            int lastSlash = path.LastIndexOf('/');
            if (lastSlash < 0)
                throw new ArgumentException("URL sin archivo válido");

            string serverRelativeFolder = path.Substring(0, lastSlash);

            return (detectedWeb, serverRelativeFolder);
        }

        private string DetectWeb(string host, string[] segments)
        {
            for (int i = segments.Length; i > 0; i--)
            {
                string candidate = "/" + string.Join("/", segments.Take(i));

                try
                {
                    using (var ctx = new ClientContext(host + candidate))
                    {
                        ctx.Credentials = CredentialCache.DefaultNetworkCredentials;
                        ctx.Load(ctx.Web, w => w.ServerRelativeUrl);
                        ctx.ExecuteQuery();

                        return host + ctx.Web.ServerRelativeUrl;
                    }
                }
                catch
                {
                    continue;
                }
            }

            return host;
        }
    }
}
