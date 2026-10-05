namespace PdfMetadataExtractor.Models
{
    public class PDFFile
    {
        public string FileName { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Author { get; set; }
        public DateTime? CreationDate { get; set; }
        public int NumberOfPages { get; set; }
        public string? Error { get; set; }

        public bool HasMissingMetadata =>
            string.IsNullOrWhiteSpace(Title) ||
            string.IsNullOrWhiteSpace(Author) ||
            CreationDate == null;
    }
}
