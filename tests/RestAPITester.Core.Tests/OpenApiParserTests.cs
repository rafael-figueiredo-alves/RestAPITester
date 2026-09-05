using RestAPITester.Core.Models;
using RestAPITester.Core.OpenApi;
using Xunit;

namespace RestAPITester.Core.Tests;

public class OpenApiParserTests
{
    private static string FixturePath =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "v1.json");

    [Fact]
    public void Parse_DeveLerArquivoSemErros()
    {
        using var stream = File.OpenRead(FixturePath);
        var parser = new OpenApiParser();

        var result = parser.Parse(stream);

        Assert.False(result.HasErrors, string.Join("; ", result.Errors));
        Assert.NotEmpty(result.Endpoints);
    }

    [Fact]
    public void Parse_DeveExtrairMetadadosBasicos()
    {
        using var stream = File.OpenRead(FixturePath);
        var parser = new OpenApiParser();

        var result = parser.Parse(stream);

        Assert.False(string.IsNullOrWhiteSpace(result.Title));
        Assert.False(string.IsNullOrWhiteSpace(result.Version));
    }

    [Fact]
    public void Parse_EndpointDeAuth_DeveSerMapeadoComoPost()
    {
        using var stream = File.OpenRead(FixturePath);
        var parser = new OpenApiParser();

        var result = parser.Parse(stream);

        var loginEndpoint = result.Endpoints
            .FirstOrDefault(e => e.Path.Contains("login", StringComparison.OrdinalIgnoreCase));

        Assert.NotNull(loginEndpoint);
        Assert.Equal(HttpMethodType.Post, loginEndpoint!.Method);
    }

    [Fact]
    public void Parse_EndpointComRequestBody_DeveExtrairSchema()
    {
        using var stream = File.OpenRead(FixturePath);
        var parser = new OpenApiParser();

        var result = parser.Parse(stream);

        var endpointComBody = result.Endpoints.FirstOrDefault(e => e.RequestBody is not null);

        Assert.NotNull(endpointComBody);
        Assert.False(string.IsNullOrWhiteSpace(endpointComBody!.RequestBody!.SchemaJson));
    }
}