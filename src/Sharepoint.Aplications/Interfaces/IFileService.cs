
using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace Sharepoint.Aplications.Interfaces
{
    public interface IFileService
    {
        bool Exists(string url);
        void UploadFile(string siteCollectionUrl, string targetFolder, string pdfName, byte[] pdfBytes);
        File GetFileInfo(string siteUrl, string fileServerRelativeUrl);
        void SetMetadata(string siteUrl, string fileServerRelativeUrl, Dictionary<string, string> metadata);
    }
}
