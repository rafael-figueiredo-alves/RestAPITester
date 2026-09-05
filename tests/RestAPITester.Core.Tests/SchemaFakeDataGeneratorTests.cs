using RestAPITester.Core.DataGeneration;
using RestAPITester.Core.OpenApi;
using System.Text.Json;
using Xunit;

namespace RestAPITester.Core.Tests;

public class SchemaFakeDataGeneratorTests
{
    private static string FixturePath =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "v1.json");

    [Fact]
    public void GenerateJson_ComEndpointRealDoV1Json_DeveGerarJsonValido()
    {
        using var stream = File.OpenRead(FixturePath);
        var parser = new OpenApiParser();
        var parseResult = parser.Parse(stream);

        var endpointComBody = parseResult.Endpoints
            .FirstOrDefault(e => e.RequestBody?.SchemaJson is not null);

        Assert.NotNull(endpointComBody);

        var generator = new SchemaFakeDataGenerator();
        var json = generator.GenerateJson(endpointComBody!.RequestBody!.SchemaJson!);

        // Deve ser um JSON de objeto válido
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(JsonValueKind.Object, doc.RootElement.ValueKind);
    }

    [Fact]
    public void GenerateJson_ComPropriedadeEmail_DeveGerarEmailValido()
    {
        const string schemaJson = """
        {
          "type": "object",
          "properties": {
            "email": { "type": "string", "format": "email" },
            "nome": { "type": "string" }
          },
          "required": ["email", "nome"]
        }
        """;

        var generator = new SchemaFakeDataGenerator();
        var json = generator.GenerateJson(schemaJson);

        using var doc = JsonDocument.Parse(json);
        var email = doc.RootElement.GetProperty("email").GetString();

        Assert.Contains("@", email);
    }

    [Fact]
    public void GenerateJson_ComArray_DeveGerarListaComItens()
    {
        const string schemaJson = """
        {
          "type": "object",
          "properties": {
            "tags": {
              "type": "array",
              "items": { "type": "string" }
            }
          }
        }
        """;

        var generator = new SchemaFakeDataGenerator();
        var json = generator.GenerateJson(schemaJson);

        using var doc = JsonDocument.Parse(json);
        var tags = doc.RootElement.GetProperty("tags");

        Assert.Equal(JsonValueKind.Array, tags.ValueKind);
        Assert.True(tags.GetArrayLength() >= 1);
    }

    [Fact]
    public void GenerateSimpleValue_ComFormatUuid_DeveGerarGuidValido()
    {
        var generator = new SchemaFakeDataGenerator();
        var value = generator.GenerateSimpleValue("string", "uuid", "id");

        Assert.True(Guid.TryParse(value?.ToString(), out _));
    }
}