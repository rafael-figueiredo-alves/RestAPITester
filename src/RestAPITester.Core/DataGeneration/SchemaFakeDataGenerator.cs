using Bogus;
using System.Text.Json;

namespace RestAPITester.Core.DataGeneration;

/// <summary>
/// Gera dados fictícios a partir do schema simplificado produzido pelo OpenApiParser
/// (o mesmo formato salvo em RequestBodyInfo.SchemaJson / ParameterInfo).
/// </summary>
public class SchemaFakeDataGenerator
{
    private readonly Faker _faker = new("pt_BR");

    /// <summary>
    /// Gera um objeto fictício completo a partir do SchemaJson de um RequestBodyInfo.
    /// Retorna já serializado como string JSON, pronto para enviar no corpo da requisição.
    /// </summary>
    public string GenerateJson(string schemaJson)
    {
        using var doc = JsonDocument.Parse(schemaJson);
        var value = GenerateValue(doc.RootElement);
        return JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Gera um valor fictício simples, usado para parâmetros individuais (query/path/header).
    /// </summary>
    public object? GenerateSimpleValue(string schemaType, string? format, string parameterName)
    {
        return GenerateScalar(schemaType, format, parameterName);
    }

    private object? GenerateValue(JsonElement schema)
    {
        var type = schema.TryGetProperty("type", out var t) ? t.GetString() : "string";
        var format = schema.TryGetProperty("format", out var f) ? f.GetString() : null;

        return type switch
        {
            "object" => GenerateObject(schema),
            "array" => GenerateArray(schema),
            _ => GenerateScalar(type ?? "string", format, propertyNameHint: null)
        };
    }

    private Dictionary<string, object?> GenerateObject(JsonElement schema)
    {
        var result = new Dictionary<string, object?>();

        if (!schema.TryGetProperty("properties", out var properties))
            return result;

        foreach (var prop in properties.EnumerateObject())
        {
            var propSchema = prop.Value;
            var type = propSchema.TryGetProperty("type", out var t) ? t.GetString() : "string";
            var format = propSchema.TryGetProperty("format", out var f) ? f.GetString() : null;

            result[prop.Name] = type switch
            {
                "object" => GenerateObject(propSchema),
                "array" => GenerateArray(propSchema),
                _ => GenerateScalar(type ?? "string", format, prop.Name)
            };
        }

        return result;
    }

    private List<object?> GenerateArray(JsonElement schema)
    {
        var count = _faker.Random.Int(1, 3); // quantidade fictícia de itens
        var list = new List<object?>();

        if (!schema.TryGetProperty("items", out var itemsSchema))
            return list;

        for (var i = 0; i < count; i++)
            list.Add(GenerateValue(itemsSchema));

        return list;
    }

    /// <summary>
    /// Gera um valor escalar com base no tipo/format do schema e, quando possível,
    /// no nome da propriedade (heurística: "email" -> email fictício, "nome"/"name" -> nome fictício, etc.)
    /// </summary>
    private object? GenerateScalar(string type, string? format, string? propertyNameHint)
{
    var hint = propertyNameHint?.ToLowerInvariant() ?? string.Empty;

    // Heurísticas por format (do OpenAPI schema) — têm prioridade,
    // pois vêm de uma declaração explícita na spec, mais confiável que o nome.
    if (format == "uuid") return Guid.NewGuid().ToString();
    if (format == "date-time") return _faker.Date.Recent().ToString("O");
    if (format == "date") return _faker.Date.Past().ToString("yyyy-MM-dd");
    if (format == "email") return _faker.Internet.Email();
    if (format == "int32" || format == "int64") return _faker.Random.Int(1, 10000);

    // Heurísticas por nome de propriedade (fallback quando o format não decide)
    if (hint.Contains("email")) return _faker.Internet.Email();
    if (hint.Contains("senha") || hint.Contains("password")) return _faker.Internet.Password();
    if (hint.Contains("telefone") || hint.Contains("phone")) return _faker.Phone.PhoneNumber();
    if (hint.Contains("nome") || hint == "name" || hint.EndsWith("name")) return _faker.Name.FullName();
    if (hint.Contains("cpf")) return _faker.Random.ReplaceNumbers("###.###.###-##");
    if (hint.Contains("cnpj")) return _faker.Random.ReplaceNumbers("##.###.###/####-##");
    if (hint.Contains("cidade") || hint.Contains("city")) return _faker.Address.City();
    if (hint.Contains("endereco") || hint.Contains("address")) return _faker.Address.StreetAddress();
    if (hint.Contains("url") || hint.Contains("site")) return _faker.Internet.Url();
    if (hint == "id" || hint.EndsWith("id")) return _faker.Random.Int(1, 10000);

    // Fallback por tipo genérico do schema
    return type switch
    {
        "integer" => _faker.Random.Int(1, 10000),
        "number" => _faker.Random.Double(0, 10000),
        "boolean" => _faker.Random.Bool(),
        "string" => _faker.Lorem.Word(),
        _ => null
    };
}
}