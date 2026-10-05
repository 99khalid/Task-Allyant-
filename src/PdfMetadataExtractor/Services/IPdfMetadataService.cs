using PdfMetadataExtractor.Models;

namespace PdfMetadataExtractor.Services
{
    public interface IPdfMetadataService
    {
        PDFFile ExtractMetadata(string fileName, Stream content);

        PDFReport BuildReport(IEnumerable<PDFFile> files);
    }
}
