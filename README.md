# PDF Metadata Extractor

[![CI](https://github.com/99khalid/pdf-metadata-extractor/actions/workflows/ci.yml/badge.svg)](https://github.com/99khalid/pdf-metadata-extractor/actions/workflows/ci.yml)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![iText](https://img.shields.io/badge/iText-9-0A7BBB)

An ASP.NET Core MVC app that takes one or more PDF files and produces a metadata report: title, author, creation date and page count for each file, the total page count, and a list of files with missing metadata. The report can be downloaded as JSON.

## Features

- Upload several PDFs at once
- Extracts title, author, creation date (PDF `D:YYYYMMDDHHmmSS` format) and page count with iText
- Flags files missing a title, author or creation date
- Downloads the report as `pdf_Report.json`
- Files are processed in memory and never stored on the server
- Upload limits: 20 MB per file, 100 MB per request, anti-forgery protection on every form

## Project structure

```
src/PdfMetadataExtractor/            ASP.NET Core MVC app
  Controllers/PDFController.cs       upload, report and download endpoints
  Services/PdfMetadataService.cs     metadata extraction and report building
  Views/PDF/                         upload form and report page
tests/PdfMetadataExtractor.Tests/    xUnit tests (service + integration)
```

## Run it

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
git clone https://github.com/99khalid/pdf-metadata-extractor.git
cd pdf-metadata-extractor
dotnet run --project src/PdfMetadataExtractor
```

Then open the URL printed in the console (for example `http://localhost:5015`).

## Tests

```bash
dotnet test
```

The tests generate PDFs on the fly with iText, so no sample files are needed.

## Example report

```json
{
  "ProcessedFiles": ["report.pdf", "scan.pdf"],
  "FilesWithMissingMetadata": ["scan.pdf"],
  "TotalPages": 12,
  "PdfFiles": [
    { "FileName": "report.pdf", "Title": "Annual Report", "Author": "Jane Doe", "CreationDate": "2024-01-15T10:30:00", "NumberOfPages": 9, "Error": null, "HasMissingMetadata": false },
    { "FileName": "scan.pdf", "Title": null, "Author": null, "CreationDate": null, "NumberOfPages": 3, "Error": null, "HasMissingMetadata": true }
  ]
}
```

## Tech

ASP.NET Core 8 MVC · iText 9 · xUnit · GitHub Actions
