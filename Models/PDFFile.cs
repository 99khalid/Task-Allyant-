namespace PdfMetadataExtractor.Models
{
    public class PDFFile
    {
        public string FilePath { get; set; } // مسار ملف PDF
        public string Title { get; set; }    // عنوان الملف
        public string Author { get; set; }   // مؤلف الملف
        public DateTime? CreationDate { get; set; } // تاريخ الإنشاء
        public int NumberOfPages { get; set; } // عدد الصفحات
    }
}
