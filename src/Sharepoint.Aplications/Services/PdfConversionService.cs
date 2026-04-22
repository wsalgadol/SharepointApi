
using Microsoft.Office.Word.Server.Conversions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Sharepoint.Aplications.Interfaces;
using Sharepoint.Logs.Interfaces;
using System;
using System.IO;
using System.Net;

namespace Sharepoint.Aplications.Services
{
    public class PdfConversionService : IPdfConversionService
    {
        private readonly IFolderService _folderService;
        private readonly IFileService _fileService;
        private readonly ILoggerService _logger;

        public PdfConversionService(IFolderService folderService, IFileService fileService, ILoggerService logger)
        {
            _folderService = folderService;
            _fileService = fileService;
            _logger = logger;
        }

        public void ConvertToPdf(string sourceUrl, string targetUrl)
        {
            var (siteCollectionUrl, serverRelativeSourceFolder) =  _folderService.ParseFolderUrl(sourceUrl);
            var filename = Path.GetFileName(sourceUrl);
            string serverRelativeUrl = $"{serverRelativeSourceFolder}/{filename}";

            _logger.Info($"Obtener el archivo de Word: {serverRelativeUrl}");
            var fileInfo = _fileService.GetFileInfo(siteCollectionUrl, serverRelativeUrl);

            _logger.Info($"convertit el archivo");
            byte[] pdfBytes = ConvertirWordAPdf(fileInfo);

            var (targetSiteCollectionUrl, targetFolder) = _folderService.ParseFolderUrl(targetUrl);
            string pdfName = Path.GetFileName(targetUrl);

            _logger.Info($"Subir el archivo pdf: {targetSiteCollectionUrl} - {targetFolder}");
            _fileService.UploadFile(targetSiteCollectionUrl, targetFolder, pdfName, pdfBytes);
        }

        public byte[] ConvertirWordAPdf(Microsoft.SharePoint.Client.File spFile)
        {
            if (spFile == null) throw new ArgumentNullException(nameof(spFile));

            byte[] result = null;
            var ctx = spFile.Context;
            
            ClientResult<Stream> data = spFile.OpenBinaryStream();
            ctx.ExecuteQuery(); 

            using (Stream read = data.Value)
            using (MemoryStream write = new MemoryStream())
            {                
                var convSettings = new ConversionJobSettings { UpdateFields = true };
                var sc = new SyncConverter("Word Automation Services", convSettings)
                {
                    UserToken = SPUserToken.SystemAccount
                };

                sc.Settings.CompatibilityMode = CompatibilityMode.Word2007;
                sc.Settings.UpdateFields = false;
                sc.Settings.OutputFormat = SaveFormat.PDF;
                
                ConversionItemInfo info = sc.Convert(read, write);
                if (info.Succeeded)
                {
                    result = write.ToArray();
                }
            }

            return result;
        }
    }
}
