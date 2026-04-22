using System.Collections.Generic;

namespace Sharepoint.Api.Models
{
    public class ConvertPdfRequest
    {
        public string SourceUrl { get; set; }
        public string TargetUrl { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}