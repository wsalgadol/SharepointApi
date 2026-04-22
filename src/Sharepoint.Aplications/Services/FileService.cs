using Microsoft.SharePoint.Client;
using Sharepoint.Aplications.Interfaces;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using File = Microsoft.SharePoint.Client.File;

namespace Sharepoint.Aplications.Services
{
    public class FileService : IFileService
    {
        private readonly string _domain = ConfigurationManager.AppSettings["SH_Domain"];
        private readonly string _user = ConfigurationManager.AppSettings["SH_User"];
        private readonly string _passWord = ConfigurationManager.AppSettings["SH_Pass"];

        public bool Exists(string url)
        {
            try
            {
                if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "HEAD";
                    request.Credentials = CredentialCache.DefaultNetworkCredentials;

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        return response.StatusCode == HttpStatusCode.OK;
                    }
                }
                else
                {
                    return System.IO.File.Exists(url);
                }
            }
            catch
            {
                return false;
            }
        }

        public void UploadFile(string siteUrl, string folderServerRelativeUrl, string fileName, byte[] content)
        {
            using (ClientContext context = new ClientContext(siteUrl))
            {
                context.Credentials = CredentialCache.DefaultNetworkCredentials;

                var fileInfo = new FileCreationInformation
                {
                    Content = content,
                    Url = fileName,
                    Overwrite = true,
                };

                Folder folder = context.Web.GetFolderByServerRelativeUrl(folderServerRelativeUrl);
                folder.Files.Add(fileInfo);
                context.ExecuteQuery();
            }
        }

        public File GetFileInfo(string siteUrl, string fileServerRelativeUrl)
        {
            using (ClientContext ctx = new ClientContext(siteUrl))
            {
                ctx.Credentials = CredentialCache.DefaultNetworkCredentials;
                var wordFile = ctx.Web.GetFileByServerRelativeUrl(fileServerRelativeUrl);
                ctx.Load(wordFile);
                ctx.ExecuteQuery();
                return wordFile;
            }
        }

        public void SetMetadata(string siteUrl, string fileServerRelativeUrl, System.Collections.Generic.Dictionary<string, string> metadata)
        {
            if (metadata == null || metadata.Count == 0) return;

            using (ClientContext ctx = new ClientContext(siteUrl))
            {
                ctx.Credentials = CredentialCache.DefaultNetworkCredentials;
                var file = ctx.Web.GetFileByServerRelativeUrl(fileServerRelativeUrl);
                var item = file.ListItemAllFields;

                foreach (var kvp in metadata)
                {
                    item[kvp.Key] = kvp.Value;
                }

                item.Update();
                ctx.ExecuteQuery();
            }
        }
    }
}
