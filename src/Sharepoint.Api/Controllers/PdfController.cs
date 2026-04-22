using Sharepoint.Api.Models;
using Sharepoint.Aplications.Interfaces;
using Sharepoint.Logs.Interfaces;
using System;
using System.IO;
using System.Threading;
using System.Web.Http;

namespace Sharepoint.Api.Controllers
{
    public class PdfController : ApiController
    {
        private readonly IPdfConversionService _pdfService;
        private readonly ILoggerService _logger;
        private readonly IFileService _fileService;
        private readonly IFolderService _folderService;

        public PdfController(IPdfConversionService pdfService, ILoggerService logger, IFileService fileService, IFolderService folderService)
        {
            _pdfService = pdfService;
            _logger = logger;
            _fileService = fileService;
            _folderService = folderService;
        }

        [HttpPost]
        [Route("api/pdf/convert")]
        public IHttpActionResult Convert([FromBody] ConvertPdfRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.SourceUrl) || string.IsNullOrWhiteSpace(request.TargetUrl))
                return BadRequest("SourceUrl y TargetUrl son obligatorios");

            try
            {
                _logger.Info($"Iniciando conversión PDF: {request.SourceUrl} → {request.TargetUrl}");
                _folderService.EnsureFolder(request.TargetUrl);

                _logger.Info($"start WAS conversion: {request.SourceUrl} → {request.TargetUrl}");
                _pdfService.ConvertToPdf(request.SourceUrl, request.TargetUrl);
                _logger.Info($"End WAS conversion: {request.SourceUrl} → {request.TargetUrl}");


                int maxWaitMs = 120000;
                int waited = 0;
                int delay = 500;
                int maxDelay = 5000;

                while (!_fileService.Exists(request.TargetUrl) && waited < maxWaitMs)
                {
                    Thread.Sleep(delay);
                    waited += delay;

                    // backoff progresivo
                    delay = Math.Min(delay * 2, maxDelay);
                }

                if (!_fileService.Exists(request.TargetUrl))
                {
                    _logger.Warning($"PDF no encontrado tras {waited / 1000}s: {request.TargetUrl}");
                    return InternalServerError(new Exception("Timeout esperando PDF"));
                }

                _logger.Info($"PDF generado correctamente tras {waited / 1000}s: {request.TargetUrl}");

                if (request.Metadata != null && request.Metadata.Count > 0)
                {
                    _logger.Info($"Aplicando metadatos al PDF: {request.TargetUrl}");
                    var (siteUrl, folder) = _folderService.ParseFolderUrl(request.TargetUrl);
                    string pdfName = Path.GetFileName(request.TargetUrl);
                    string fileRelativeUrl = $"{folder}/{pdfName}";
                    _fileService.SetMetadata(siteUrl, fileRelativeUrl, request.Metadata);
                    _logger.Info($"Metadatos aplicados correctamente");
                }

                return Ok(new { Status = "Conversion executed", request.TargetUrl });
            }
            catch (Exception ex)
            {
                _logger.Error("Error al convertir PDF", ex);
                return InternalServerError(ex);
            }
        }
    }

}
