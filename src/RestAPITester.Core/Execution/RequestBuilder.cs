using System.Text;
using RestAPITester.Core.Models;

namespace RestAPITester.Core.Execution;

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
            request.Content = BuildBodyContent(endpoint.RequestBody.ContentType, body);
        }

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

        // Headers "avulsos" (ex: If-Match, If-None-Match) — cobre ETag e afins,
        // mesmo quando a spec não declara isso como parâmetro formal do endpoint.
        foreach (var (headerName, headerValue) in testCase.ExtraHeaders)
        {
            if (request.Headers.Contains(headerName)) continue;
            var resolved = ResolveVariables(headerValue, sessionVariables);
            request.Headers.TryAddWithoutValidation(headerName, resolved);
        }

        // Bearer automático: não depende mais de endpoint.RequiresAuth (várias APIs
        // só declaram segurança globalmente na spec, não por operação).
        if (!request.Headers.Contains("Authorization") &&
            sessionVariables.TryGetValue("authToken", out var token))
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

    public static string BuildCurlCommand(HttpRequestMessage request, string? requestBodyJson)
    {
        var sb = new StringBuilder();
        sb.Append($"curl -X {request.Method.Method} \"{request.RequestUri}\"");

        foreach (var header in request.Headers)
            foreach (var value in header.Value)
                sb.Append($" \\\n  -H \"{header.Key}: {value}\"");

        if (request.Content is not null)
        {
            if (request.Content.Headers.ContentType is not null)
                sb.Append($" \\\n  -H \"Content-Type: {request.Content.Headers.ContentType}\"");

            if (!string.IsNullOrEmpty(requestBodyJson))
                sb.Append($" \\\n  -d '{requestBodyJson.Replace("'", "'\\''")}'");
        }

        return sb.ToString();
    }    

    private static HttpContent BuildBodyContent(string contentType, string body)
    {
        if (contentType.Contains("x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
        {
            var pairs = ParseKeyValueLines(body).Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value));
            return new FormUrlEncodedContent(pairs);
        }

        if (contentType.Contains("multipart/form-data", StringComparison.OrdinalIgnoreCase))
        {
            var multipart = new MultipartFormDataContent();
            foreach (var (key, value) in ParseKeyValueLines(body))
                multipart.Add(new StringContent(value), key);
            return multipart; // define o próprio Content-Type com boundary automaticamente
        }

        return new StringContent(body, Encoding.UTF8, contentType);
    }

    private static IEnumerable<(string Key, string Value)> ParseKeyValueLines(string text)
    {
        foreach (var line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
                yield return (parts[0].Trim(), parts[1].Trim());
        }
    }    
}