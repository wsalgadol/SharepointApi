namespace Sharepoint.Aplications.Interfaces
{
    public interface IPdfConversionService
    {
        void ConvertToPdf(string sourceUrl, string targetUrl);

    }
}
