using Microsoft.OpenApi;
using RestAPITester.Core.EnumsAndConstants;
using RestAPITester.Core.Models;
using System.Text.Json;

namespace RestAPITester.Core.OpenApi;

/// <summary>
/// Lê um documento OpenAPI (JSON, suportando 3.0.x e 3.1.x) e converte
/// em uma lista de EndpointInfo normalizada para o restante da aplicação.
/// </summary>
public class OpenApiParser
{
    /// <summary>
    /// Lê um documento OpenAPI (JSON, suportando 3.0.x e 3.1.x) a partir de um Stream
    /// </summary>
    /// <param name="openApiContent">O conteúdo do documento OpenAPI</param>
    /// <returns></returns>
    public ParseResult Parse(Stream openApiContent)
    {
        using var reader = new StreamReader(openApiContent);
        return Parse(reader.ReadToEnd());
    }

    /// <summary>
    /// Lê um documento OpenAPI (JSON, suportando 3.0.x e 3.1.x) a partir de uma string
    /// </summary>
    /// <param name="openApiContent">O conteúdo do documento OpenAPI</param>
    /// <returns></returns>
    public ParseResult Parse(string openApiContent)
    {
        var (document, diagnostic) = OpenApiDocument.Parse(openApiContent);

        var endpoints = new List<EndpointInfo>();

        if (document?.Paths is not null)
        {
            foreach (var pathItem in document.Paths)
            {
                var path = pathItem.Key;

                foreach (var operationEntry in pathItem.Value.Operations!)
                {
                    var httpMethod = MapMethod(operationEntry.Key);
                    var operation = operationEntry.Value;

                    var endpoint = new EndpointInfo
                    {
                        OperationId = string.IsNullOrWhiteSpace(operation.OperationId)
                            ? $"{httpMethod}_{path}".Replace("/", "_")
                            : operation.OperationId,
                        Path = path,
                        Method = httpMethod,
                        Summary = operation.Summary,
                        Description = operation.Description,
                        Tags = operation.Tags?.Select(t => t.Name ?? string.Empty).ToList() ?? new(),
                        RequiresAuth = operation.Security is { Count: > 0 }
                    };

                    endpoint.Parameters = MapParameters(operation.Parameters);
                    endpoint.RequestBody = MapRequestBody(operation.RequestBody);
                    endpoint.Responses = MapResponses(operation.Responses);

                    endpoints.Add(endpoint);
                }
            }
        }

        return new ParseResult
        {
            Endpoints = endpoints,
            Title = document?.Info?.Title ?? "API sem título",
            Version = document?.Info?.Version ?? "-",
            Servers = document?.Servers?.Select(s => s.Url ?? string.Empty).ToList() ?? new(),
            HasErrors = diagnostic?.Errors?.Count > 0,
            Errors = diagnostic?.Errors?.Select(e => e.Message).ToList() ?? new()
        };
    }

    #region Métodos auxiliares privados

    /// <summary>
    /// Mapeia o HttpMethod do Microsoft.OpenApi para o HttpMethodType usado internamente na aplicação.
    /// </summary>
    /// <param name="method">O HttpMethod do Microsoft.OpenApi</param>
    /// <returns>O HttpMethodType correspondente</returns>
    private static HttpMethodType MapMethod(HttpMethod method)
    {
        if (method == HttpMethod.Get) return HttpMethodType.Get;
        if (method == HttpMethod.Post) return HttpMethodType.Post;
        if (method == HttpMethod.Put) return HttpMethodType.Put;
        if (method == HttpMethod.Patch) return HttpMethodType.Patch;
        if (method == HttpMethod.Delete) return HttpMethodType.Delete;
        if (method == HttpMethod.Head) return HttpMethodType.Head;
        if (method == HttpMethod.Options) return HttpMethodType.Options;
        return HttpMethodType.Get;
    }

    /// <summary>
    /// Mapeia os parâmetros do Microsoft.OpenApi para a lista de ParameterInfo usada internamente na aplicação.
    /// </summary>
    /// <param name="parameters">Os parâmetros do Microsoft.OpenApi</param>
    /// <returns>A lista de ParameterInfo</returns>
    private static List<ParameterInfo> MapParameters(IList<IOpenApiParameter>? parameters)
    {
        if (parameters is null) return new();

        return parameters.Select(p => new ParameterInfo
        {
            Name = p.Name ?? string.Empty,
            Location = p.In switch
            {
                Microsoft.OpenApi.ParameterLocation.Query => Models.ParameterLocation.Query,
                Microsoft.OpenApi.ParameterLocation.Path => Models.ParameterLocation.Path,
                Microsoft.OpenApi.ParameterLocation.Header => Models.ParameterLocation.Header,
                Microsoft.OpenApi.ParameterLocation.Cookie => Models.ParameterLocation.Cookie,
                _ => Models.ParameterLocation.Query
            },
            Required = p.Required,
            SchemaType = MapSchemaType(p.Schema?.Type),
            Format = p.Schema?.Format,
            Description = p.Description,
            Example = p.Example?.ToString()
        }).ToList();
    }

    /// <summary>
    /// Mapeia o RequestBody do Microsoft.OpenApi para o RequestBodyInfo usado internamente na aplicação.
    /// </summary>
    /// <param name="requestBody">O RequestBody do Microsoft.OpenApi</param>
    /// <returns>O RequestBodyInfo correspondente</returns>
    private static RequestBodyInfo? MapRequestBody(IOpenApiRequestBody? requestBody)
    {
        if (requestBody is null) return null;

        var content = requestBody.Content!.TryGetValue("application/json", out var jsonContent)
            ? jsonContent
            : requestBody.Content.Values.FirstOrDefault();

        if (content is null) return null;

        return new RequestBodyInfo
        {
            Required = requestBody.Required,
            ContentType = requestBody.Content.ContainsKey("application/json")
                ? "application/json"
                : requestBody.Content.Keys.FirstOrDefault() ?? "application/json",
            SchemaJson = content.Schema is null ? null : SerializeSchema(content.Schema),
            Example = content.Example?.ToString()
        };
    }
    
    /// <summary>
    /// Mapeia as respostas do Microsoft.OpenApi para a lista de ResponseInfo usada internamente na aplicação.
    /// </summary>
    /// <param name="responses">As respostas do Microsoft.OpenApi</param>
    /// <returns>A lista de ResponseInfo</returns>
    private static List<ResponseInfo> MapResponses(OpenApiResponses? responses)
    {
        if (responses is null) return new();

        var result = new List<ResponseInfo>();

        foreach (var (statusCode, response) in responses)
        {
            OpenApiMediaType? content = null;

            if (response.Content is not null)
            {
                content = response.Content.TryGetValue("application/json", out var jsonContent)
                    ? jsonContent
                    : response.Content.Values.FirstOrDefault();
            }

            result.Add(new ResponseInfo
            {
                StatusCode = statusCode,
                Description = response.Description,
                ContentType = content is null ? null : "application/json",
                SchemaJson = content?.Schema is null ? null : SerializeSchema(content.Schema)
            });
        }

        return result;
    }   

    /// <summary>
    /// Serializa o IOpenApiSchema para uma representação JSON simplificada,
    /// usada depois pelo módulo de DataGeneration (Bogus) para gerar valores fictícios.
    /// </summary>
    private static string SerializeSchema(IOpenApiSchema schema)
    {
        var simplified = SimplifySchema(schema);
        return JsonSerializer.Serialize(simplified);
    }

    /// <summary>
    /// Simplifica o IOpenApiSchema para um objeto anônimo que pode ser serializado em JSON.
    /// </summary>
    /// <param name="schema">O IOpenApiSchema a ser simplificado</param>
    /// <returns>O objeto anônimo simplificado</returns>
    private static object SimplifySchema(IOpenApiSchema schema)
    {
        var typeStr = MapSchemaType(schema.Type);

        var node = new Dictionary<string, object?>
        {
            ["type"] = typeStr,
            ["format"] = schema.Format,
            ["nullable"] = schema.Type?.HasFlag(JsonSchemaType.Null) ?? false
        };

        if (typeStr == "object" && schema.Properties?.Count > 0)
        {
            node["properties"] = schema.Properties.ToDictionary(
                p => p.Key,
                p => SimplifySchema(p.Value));
            node["required"] = schema.Required?.ToList() ?? new List<string>();
        }

        if (typeStr == "array" && schema.Items is not null)
        {
            node["items"] = SimplifySchema(schema.Items);
        }

        if (schema.Enum?.Count > 0)
        {
            node["enum"] = schema.Enum.Select(e => e?.ToString() ?? string.Empty).ToList();
        }

        return node;
    }

    /// <summary>
    /// Converte o JsonSchemaType (enum flags do Microsoft.OpenApi 2.x) para a mesma
    /// representação em string que já usávamos internamente ("object", "array", "string"...).
    /// </summary>
    private static string MapSchemaType(JsonSchemaType? type)
    {
        if (type is null) return "string";
        if (type.Value.HasFlag(JsonSchemaType.Object)) return "object";
        if (type.Value.HasFlag(JsonSchemaType.Array)) return "array";
        if (type.Value.HasFlag(JsonSchemaType.Boolean)) return "boolean";
        if (type.Value.HasFlag(JsonSchemaType.Integer)) return "integer";
        if (type.Value.HasFlag(JsonSchemaType.Number)) return "number";
        if (type.Value.HasFlag(JsonSchemaType.String)) return "string";
        return "string";
    }
    #endregion
}