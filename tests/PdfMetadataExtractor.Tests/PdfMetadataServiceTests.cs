using System.Text;
using iText.Kernel.Pdf;
using Microsoft.Extensions.Logging.Abstractions;
using PdfMetadataExtractor.Models;
using PdfMetadataExtractor.Services;

namespace PdfMetadataExtractor.Tests;

public class PdfMetadataServiceTests
{
    private readonly PdfMetadataService _service = new(NullLogger<PdfMetadataService>.Instance);

    private static MemoryStream CreatePdf(int pages, string? title = null, string? author = null)
    {
        var output = new MemoryStream();
        var writer = new PdfWriter(output);
        writer.SetCloseStream(false);

        using (var pdf = new PdfDocument(writer))
        {
            var info = pdf.GetDocumentInfo();
            if (title != null) info.SetTitle(title);
            if (author != null) info.SetAuthor(author);

            for (var i = 0; i < pages; i++)
            {
                pdf.AddNewPage();
            }
        }

        output.Position = 0;
        return output;
    }

    [Fact]
    public void ExtractMetadata_ReadsTitleAuthorDateAndPages()
    {
        using var pdf = CreatePdf(pages: 3, title: "Annual Report", author: "Khalid Hassan");

        var result = _service.ExtractMetadata("report.pdf", pdf);

        Assert.Equal("report.pdf", result.FileName);
        Assert.Equal("Annual Report", result.Title);
        Assert.Equal("Khalid Hassan", result.Author);
        Assert.NotNull(result.CreationDate);
        Assert.Equal(3, result.NumberOfPages);
        Assert.False(result.HasMissingMetadata);
        Assert.Null(result.Error);
    }

    [Fact]
    public void ExtractMetadata_FlagsMissingAuthor()
    {
        using var pdf = CreatePdf(pages: 1, title: "No author");

        var result = _service.ExtractMetadata("no-author.pdf", pdf);

        Assert.True(result.HasMissingMetadata);
        Assert.Equal(1, result.NumberOfPages);
    }

    [Fact]
    public void ExtractMetadata_ReturnsErrorForInvalidPdf()
    {
        using var notAPdf = new MemoryStream(Encoding.UTF8.GetBytes("this is not a pdf"));

        var result = _service.ExtractMetadata("fake.pdf", notAPdf);

        Assert.NotNull(result.Error);
        Assert.Equal(0, result.NumberOfPages);
    }

    [Fact]
    public void BuildReport_SumsPagesAndListsFilesWithMissingMetadata()
    {
        var files = new[]
        {
            new PDFFile { FileName = "a.pdf", Title = "A", Author = "X", CreationDate = DateTime.Today, NumberOfPages = 2 },
            new PDFFile { FileName = "b.pdf", Title = "B", NumberOfPages = 5 },
        };

        var report = _service.BuildReport(files);

        Assert.Equal(7, report.TotalPages);
        Assert.Equal(new[] { "a.pdf", "b.pdf" }, report.ProcessedFiles);
        Assert.Equal(new[] { "b.pdf" }, report.FilesWithMissingMetadata);
    }

    [Theory]
    [InlineData("D:20240115103000+02'00'", 2024, 1, 15)]
    [InlineData("D:20231231", 2023, 12, 31)]
    public void ParsePdfDate_ParsesPdfDateFormat(string raw, int year, int month, int day)
    {
        var date = PdfMetadataService.ParsePdfDate(raw);

        Assert.NotNull(date);
        Assert.Equal(new DateTime(year, month, day), date!.Value.Date);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not a date")]
    public void ParsePdfDate_ReturnsNullForMissingOrInvalidValues(string? raw)
    {
        Assert.Null(PdfMetadataService.ParsePdfDate(raw));
    }
}
