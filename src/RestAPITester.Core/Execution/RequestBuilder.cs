using System.Text;
using RestAPITester.Core.Models;

namespace RestAPITester.Core.Execution;

/// <summary>
/// Monta um HttpRequestMessage a partir de um EndpointInfo + TestCase,
/// resolvendo parâmetros de path/query e variáveis capturadas de chamadas anteriores.
/// </summary>
public class RequestBuilder
{
    public HttpRequestMessage Build(
        EndpointInfo endpoint,
        TestCase testCase,
        string baseUrl,
        IReadOnlyDictionary<string, string> sessionVariables,
        string? proxyBaseUrl = null)
    {
        var path = ResolvePathParameters(endpoint, testCase, sessionVariables);
        var url = BuildUrlWithQuery(baseUrl.TrimEnd('/') + path, endpoint, testCase, sessionVariables);

        var request = new HttpRequestMessage(MapMethod(endpoint.Method), url);

        ApplyHeaders(request, endpoint, testCase, sessionVariables);

        if (endpoint.RequestBody is not null && testCase.RequestBodyJson is not null)
        {
            var body = ResolveVariables(testCase.RequestBodyJson, sessionVariables);
            request.Content = new StringContent(body, Encoding.UTF8, endpoint.RequestBody.ContentType);
        }

        // Se um proxy local foi configurado, reaponta a requisição pra ele,
        // levando a URL real como query string ("target").
        if (!string.IsNullOrWhiteSpace(proxyBaseUrl))
        {
            var originalUrl = request.RequestUri!.ToString();
            request.RequestUri = new Uri($"{proxyBaseUrl.TrimEnd('/')}/proxy?target={Uri.EscapeDataString(originalUrl)}");
        }

        return request;
    }

    private static string ResolvePathParameters(
        EndpointInfo endpoint, TestCase testCase, IReadOnlyDictionary<string, string> sessionVariables)
    {
        var path = endpoint.Path;

        foreach (var param in endpoint.Parameters.Where(p => p.Location == ParameterLocation.Path))
        {
            var value = GetParameterValue(param.Name, testCase, sessionVariables);
            path = path.Replace($"{{{param.Name}}}", Uri.EscapeDataString(value ?? string.Empty));
        }

        return path;
    }

    private static string BuildUrlWithQuery(
        string baseUrlWithPath, EndpointInfo endpoint, TestCase testCase,
        IReadOnlyDictionary<string, string> sessionVariables)
    {
        var queryParams = endpoint.Parameters
            .Where(p => p.Location == ParameterLocation.Query)
            .Select(p => (p.Name, Value: GetParameterValue(p.Name, testCase, sessionVariables)))
            .Where(p => p.Value is not null)
            .Select(p => $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(p.Value!)}")
            .ToList();

        return queryParams.Count == 0
            ? baseUrlWithPath
            : $"{baseUrlWithPath}?{string.Join("&", queryParams)}";
    }

    private static void ApplyHeaders(
        HttpRequestMessage request, EndpointInfo endpoint, TestCase testCase,
        IReadOnlyDictionary<string, string> sessionVariables)
    {
        foreach (var param in endpoint.Parameters.Where(p => p.Location == ParameterLocation.Header))
        {
            var value = GetParameterValue(param.Name, testCase, sessionVariables);
            if (value is not null)
                request.Headers.TryAddWithoutValidation(param.Name, value);
        }

        // Convenção: se a suíte capturou uma variável chamada "authToken",
        // e o endpoint requer autenticação, injeta como Bearer automaticamente.
        if (endpoint.RequiresAuth && sessionVariables.TryGetValue("authToken", out var token))
        {
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");
        }
    }

    private static string? GetParameterValue(
        string name, TestCase testCase, IReadOnlyDictionary<string, string> sessionVariables)
    {
        if (testCase.ParameterValues.TryGetValue(name, out var value) && value is not null)
        {
            var stringValue = value.ToString();
            return stringValue is null ? null : ResolveVariables(stringValue, sessionVariables);
        }

        return null;
    }

    /// <summary>
    /// Substitui placeholders como {{authToken}} no texto por variáveis capturadas
    /// de execuções anteriores dentro da mesma TestSuite.
    /// </summary>
    private static string ResolveVariables(string text, IReadOnlyDictionary<string, string> sessionVariables)
    {
        foreach (var (key, value) in sessionVariables)
            text = text.Replace($"{{{{{key}}}}}", value);

        return text;
    }

    private static HttpMethod MapMethod(HttpMethodType method) => method switch
    {
        HttpMethodType.Get => HttpMethod.Get,
        HttpMethodType.Post => HttpMethod.Post,
        HttpMethodType.Put => HttpMethod.Put,
        HttpMethodType.Patch => HttpMethod.Patch,
        HttpMethodType.Delete => HttpMethod.Delete,
        HttpMethodType.Head => HttpMethod.Head,
        HttpMethodType.Options => HttpMethod.Options,
        _ => HttpMethod.Get
    };
}