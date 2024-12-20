using Microsoft.AspNetCore.Mvc;
using PdfMetadataExtractor.Models;
using PdfMetadataExtractor.Services;
using System.IO;

namespace PdfMetadataExtractor.Controllers
{
    public class PDFController : Controller
    {
        private readonly PDFProcessor _pdfProcessor;

        public PDFController()
        {
            _pdfProcessor = new PDFProcessor();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

            [HttpPost]
        public IActionResult ProcessFiles(List<IFormFile> pdfFiles)
        {
            var filePaths = new List<string>();

            // تحديد المجلد لحفظ الملفات فيه
            string uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            // التأكد من وجود المجلد، إذا لم يكن موجودًا نقوم بإنشائه
            if (!Directory.Exists(uploadFolderPath))
            {
                Directory.CreateDirectory(uploadFolderPath);
            }

            // حفظ الملفات في المجلد المحدد ومعالجة المسارات
            foreach (var file in pdfFiles)
            {
                if (file.ContentType != "application/pdf")
                {
                    continue; // تجاهل الملفات غير الـ PDF
                }

                // تحديد المسار الفعلي للملف
                var filePath = Path.Combine(uploadFolderPath, file.FileName);

                // حفظ الملف في المجلد المحدد
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                filePaths.Add(filePath);
            }

            // معالجة الملفات وإنشاء التقرير
            var report = _pdfProcessor.ProcessPDFs(filePaths);
            TempData["Report"] = System.Text.Json.JsonSerializer.Serialize(report);

            return View("Report", report);
        }

        [HttpPost]
        public IActionResult DownloadReport()
        {
            if (TempData["Report"] is string reportJson)
            {
                var reportBytes = System.Text.Encoding.UTF8.GetBytes(reportJson);
                return File(reportBytes, "application/json", "pdf_Report.json");
            }

            return BadRequest("No report available to download.");
        }
    }
}
