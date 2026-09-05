using Json.Path;
using System.Text.Json.Nodes;

namespace RestAPITester.Core.Execution;

/// <summary>
/// Ponto único de avaliação de JSONPath (via JsonPath.Net), usado tanto pelas
/// asserções quanto pela captura de variáveis. Suporta arrays, wildcards e filtros.
/// </summary>
public static class JsonPathHelper
{
    /// <summary>
    /// Retorna o primeiro valor encontrado, como string. Útil pra capturar um único
    /// valor (ex: token) ou comparar um campo específico numa asserção.
    /// </summary>
    public static string? EvaluateFirst(string? json, string? path)
    {
        var values = EvaluateAll(json, path);
        return values.FirstOrDefault();
    }

    /// <summary>
    /// Retorna todos os valores encontrados como string — útil quando o path usa
    /// wildcard (ex: "$.items[*].nome") e você quer todas as ocorrências.
    /// </summary>
    public static List<string> EvaluateAll(string? json, string? path)
    {
        if (json is null || string.IsNullOrWhiteSpace(path)) return new();

        try
        {
            var node = JsonNode.Parse(json);
            var jsonPath = JsonPath.Parse(path);
            var evalResult = jsonPath.Evaluate(node);

            return evalResult.Matches
                .Where(m => m.Value is not null)
                .Select(m => m.Value is JsonValue value && value.TryGetValue<string>(out var s)
                    ? s
                    : m.Value!.ToJsonString())
                .ToList();
        }
        catch
        {
            return new();
        }
    }
}