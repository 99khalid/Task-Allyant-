using Microsoft.AspNetCore.Mvc.Testing;

namespace PdfMetadataExtractor.Tests;

public class UploadPageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UploadPageTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HomePage_ShowsUploadForm()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Upload PDF Files", html);
    }
}
