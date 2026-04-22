using Sharepoint.Aplications.Interfaces;
using Sharepoint.Logs.Interfaces;

namespace Sharepoint.Aplications.Services
{
    public class DummyPdfConversionService : IPdfConversionService
    {
        private readonly ILoggerService _logger;

        public DummyPdfConversionService(ILoggerService logger)
        {
            _logger = logger;
        }

        public void ConvertToPdf(string sourceUrl, string targetUrl)
        {
            _logger.Info("Dummy: simulando conversión PDF");
            _logger.Info($"Source: {sourceUrl}");
            _logger.Info($"Target: {targetUrl}");

            // Opcional: crear un archivo de prueba para que tu API lo vea
            System.IO.File.WriteAllText(targetUrl, "Simulación PDF");
        }
    }

}
