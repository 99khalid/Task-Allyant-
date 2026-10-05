using iText.Kernel.Pdf;
using PdfMetadataExtractor.Models;

namespace PdfMetadataExtractor.Services
{
    public class PdfMetadataService : IPdfMetadataService
    {
        private readonly ILogger<PdfMetadataService> _logger;

        public PdfMetadataService(ILogger<PdfMetadataService> logger)
        {
            _logger = logger;
        }

        public PDFFile ExtractMetadata(string fileName, Stream content)
        {
            var pdfFile = new PDFFile { FileName = fileName };

            try
            {
                using var pdfReader = new PdfReader(content);
                pdfReader.SetCloseStream(false);
                using var pdfDoc = new PdfDocument(pdfReader);

                var info = pdfDoc.GetDocumentInfo();
                pdfFile.Title = info.GetTitle();
                pdfFile.Author = info.GetAuthor();
                pdfFile.CreationDate = ParsePdfDate(info.GetMoreInfo(PdfName.CreationDate.GetValue()));
                pdfFile.NumberOfPages = pdfDoc.GetNumberOfPages();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not read PDF {FileName}", fileName);
                pdfFile.Error = "This file could not be read as a PDF.";
            }

            return pdfFile;
        }

        public PDFReport BuildReport(IEnumerable<PDFFile> files)
        {
            var report = new PDFReport();

            foreach (var file in files)
            {
                report.ProcessedFiles.Add(file.FileName);
                report.PdfFiles.Add(file);
                report.TotalPages += file.NumberOfPages;

                if (file.HasMissingMetadata)
                {
                    report.FilesWithMissingMetadata.Add(file.FileName);
                }
            }

            return report;
        }

        // PDF dates look like "D:20240115103000+02'00'" (only the year is mandatory).
        public static DateTime? ParsePdfDate(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            try
            {
                return PdfDate.Decode(raw);
            }
            catch (Exception)
            {
                return DateTime.TryParse(raw, out var parsed) ? parsed : null;
            }
        }
    }
}
