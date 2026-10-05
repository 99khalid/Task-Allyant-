namespace PdfMetadataExtractor.Models
{
    public class PDFReport
    {
        public List<string> ProcessedFiles { get; set; } = new();
        public List<string> FilesWithMissingMetadata { get; set; } = new();
        public int TotalPages { get; set; }
        public List<PDFFile> PdfFiles { get; set; } = new();
    }
}
