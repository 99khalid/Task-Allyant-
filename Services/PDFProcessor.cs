using PdfMetadataExtractor.Models;
using iText.Kernel.Pdf;
using System.Text.Json;

namespace PdfMetadataExtractor.Services
{
    public class PDFProcessor
    {
        public PDFReport ProcessPDFs(List<string> filePaths)
        {
            PDFReport report = new PDFReport();
            foreach (var filePath in filePaths)
            {
                // استخراج البيانات من ملف PDF
                var pdfFile = ExtractMetadata(filePath);
                report.ProcessedFiles.Add(filePath);

                // التحقق من البيانات المفقودة
                if (string.IsNullOrWhiteSpace(pdfFile.Title) ||
                    string.IsNullOrWhiteSpace(pdfFile.Author) ||
                    pdfFile.CreationDate == null)
                {
                    report.FilesWithMissingMetadata.Add(filePath);
                }

                // جمع إجمالي عدد الصفحات
                report.TotalPages += pdfFile.NumberOfPages;

                // إضافة pdfFile إلى التقرير لعرض البيانات في الـ View
                report.PdfFiles.Add(pdfFile);
            }

            // إنشاء ملف JSON
            SaveReportAsJson(report);
            return report;
        }

       private PDFFile ExtractMetadata(string filePath)
{
    var pdfFile = new PDFFile();

    try
    {
        using (var pdfReader = new PdfReader(filePath))
        {
            using (var pdfDoc = new PdfDocument(pdfReader))
            {
                // استخراج البيانات الوصفية باستخدام GetDocumentInfo
                var info = pdfDoc.GetDocumentInfo();
                pdfFile.FilePath = filePath; // المسار الفعلي للملف
                
                // استخراج العنوان والمؤلف
                pdfFile.Title = info.GetTitle();
                pdfFile.Author = info.GetAuthor();

                // استخراج تاريخ الإنشاء (إذا كان موجودًا)
                var creationDateRaw = info.GetMoreInfo("Date created");

                if (!string.IsNullOrWhiteSpace(creationDateRaw))
                {
                    DateTime creationDate;

                    // محاولة تحويل التاريخ من تنسيق "D:yyyyMMddHHmmss" إذا كان موجودًا
                    bool dateParsed = DateTime.TryParseExact(
                        creationDateRaw.Replace("D:", ""), // إزالة الجزء "D:"
                        "yyyyMMddHHmmss", 
                        null, 
                        System.Globalization.DateTimeStyles.None, 
                        out creationDate
                    );

                    // إذا تم التحويل بنجاح، استخدم التاريخ المحول
                    if (dateParsed)
                    {
                        pdfFile.CreationDate = creationDate;
                    }
                    else
                    {
                        // إذا لم يتم التحويل، محاولة تحويل التاريخ باستخدام الصيغة العامة
                        pdfFile.CreationDate = DateTime.TryParse(creationDateRaw, out creationDate) ? creationDate : (DateTime?)null;
                    }
                }

                // الحصول على عدد الصفحات في ملف PDF
                pdfFile.NumberOfPages = pdfDoc.GetNumberOfPages();
            }
        }
    }
    catch (Exception ex)
    {
        // التعامل مع أي استثناءات قد تحدث أثناء المعالجة
        Console.WriteLine($"Error processing {filePath}: {ex.Message}");
    }

    return pdfFile;
}

        private void SaveReportAsJson(PDFReport report)
        {
            // تحديد المسار لحفظ التقرير
            var reportPath = "pdf_Report.json";
            var jsonReport = JsonSerializer.Serialize(report);
            File.WriteAllText(reportPath, jsonReport);
        }
    }
}
