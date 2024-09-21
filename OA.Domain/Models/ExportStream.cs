using System.IO;
namespace OA.Core.Models
{
    public class ExportStream
    {
        public MemoryStream? Stream { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
    }
}
