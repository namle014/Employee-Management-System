namespace OA.Core.Models
{
    public class ExportStream
    {
        public MemoryStream Stream { get; set; } = new MemoryStream();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
