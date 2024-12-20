namespace PdfMetadataExtractor.Models
{
    public class PDFReport
    {
        public List<string> ProcessedFiles { get; set; } = new List<string>(); // قائمة الملفات التي تمت معالجتها
        public List<string> FilesWithMissingMetadata { get; set; } = new List<string>(); // ملفات ذات بيانات ناقصة
        public int TotalPages { get; set; } // العدد الإجمالي للصفحات
        public List<PDFFile> PdfFiles { get; set; } = new List<PDFFile>(); // إضافة قائمة الملفات مع بياناتها
    }
}
