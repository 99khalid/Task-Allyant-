using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PdfMetadataExtractor.Models;
using PdfMetadataExtractor.Services;

namespace PdfMetadataExtractor.Controllers
{
    public class PDFController : Controller
    {
        public const long MaxFileSizeBytes = 20 * 1024 * 1024;
        public const long MaxRequestSizeBytes = 100 * 1024 * 1024;

        private readonly IPdfMetadataService _pdfMetadataService;

        public PDFController(IPdfMetadataService pdfMetadataService)
        {
            _pdfMetadataService = pdfMetadataService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(MaxRequestSizeBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestSizeBytes)]
        public IActionResult ProcessFiles(List<IFormFile> pdfFiles)
        {
            var files = new List<PDFFile>();

            // Files are read in memory and never written to disk.
            foreach (var file in pdfFiles)
            {
                var fileName = Path.GetFileName(file.FileName);

                if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (file.Length == 0 || file.Length > MaxFileSizeBytes)
                {
                    files.Add(new PDFFile { FileName = fileName, Error = "File is empty or larger than 20 MB." });
                    continue;
                }

                using var stream = file.OpenReadStream();
                files.Add(_pdfMetadataService.ExtractMetadata(fileName, stream));
            }

            if (files.Count == 0)
            {
                ModelState.AddModelError("pdfFiles", "Please select at least one PDF file.");
                return View("Index");
            }

            var report = _pdfMetadataService.BuildReport(files);
            TempData["Report"] = JsonSerializer.Serialize(report);

            return View("Report", report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DownloadReport()
        {
            if (TempData["Report"] is string reportJson)
            {
                var reportBytes = System.Text.Encoding.UTF8.GetBytes(reportJson);
                return File(reportBytes, "application/json", "pdf_Report.json");
            }

            return BadRequest("No report available to download.");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
