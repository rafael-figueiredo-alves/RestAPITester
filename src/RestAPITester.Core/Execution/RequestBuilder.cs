using System.Text;
using RestAPITester.Core.EnumsAndConstants;
using RestAPITester.Core.Models;

namespace RestAPITester.Core.Execution;

/// <summary>
/// Construtor de requisições HTTP a partir de informações de endpoint e caso de teste.
/// </summary>
public class RequestBuilder
{
    /// <summary>
    /// Método principal para construir uma requisição HTTP a partir de um endpoint e caso de teste.
    /// </summary>
    /// <param name="endpoint">O endpoint para o qual construir a requisição</param>
    /// <param name="testCase">O caso de teste com as informações de execução</param>
    /// <param name="baseUrl">A URL base para a requisição</param>
    /// <param name="sessionVariables">As variáveis de sessão disponíveis</param>
    /// <param name="proxyBaseUrl">A URL base do proxy, se aplicável</param>
    /// <returns></returns>
    public HttpRequestMessage Build(EndpointInfo endpoint, TestCase testCase,string baseUrl,IReadOnlyDictionary<string, string> sessionVariables,string? proxyBaseUrl = null)
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
            var proxyUri = $"{proxyBaseUrl.TrimEnd('/')}/proxy?target={Uri.EscapeDataString(originalUrl)}";

            if (!testCase.FollowRedirects)
                proxyUri += "&followRedirects=false";

            request.RequestUri = new Uri(proxyUri);
        }

        return request;
    }

    /// <summary>
    /// Constrói um comando cURL a partir de uma requisição HTTP, incluindo método, URL, cabeçalhos e corpo da requisição.
    /// </summary>
    /// <param name="request">A requisição HTTP</param>
    /// <param name="requestBodyJson">O corpo da requisição em formato JSON</param>
    /// <returns>O comando cURL</returns>
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

    #region Métodos auxiliares privados
    /// <summary>
    /// Resolve os parâmetros de caminho na URL do endpoint, substituindo-os pelos valores fornecidos no caso de teste ou nas variáveis de sessão.
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="testCase"></param>
    /// <param name="sessionVariables"></param>
    /// <returns></returns>
    private static string ResolvePathParameters(EndpointInfo endpoint, TestCase testCase, IReadOnlyDictionary<string, string> sessionVariables)
    {
        var path = endpoint.Path;

        foreach (var param in endpoint.Parameters.Where(p => p.Location == ParameterLocation.Path))
        {
            var value = GetParameterValue(param.Name, testCase, sessionVariables);
            path = path.Replace($"{{{param.Name}}}", Uri.EscapeDataString(value ?? string.Empty));
        }

        return path;
    }

    /// <summary>
    /// Constrói a URL completa com os parâmetros de consulta (query parameters) adicionados, se houver.
    /// </summary>
    /// <param name="baseUrlWithPath">a URL base com o caminho do endpoint</param>
    /// <param name="endpoint">informações sobre o endpoint</param>
    /// <param name="testCase">o caso de teste</param>
    /// <param name="sessionVariables">as variáveis de sessão</param>
    /// <returns></returns>
    private static string BuildUrlWithQuery(string baseUrlWithPath, EndpointInfo endpoint, TestCase testCase,IReadOnlyDictionary<string, string> sessionVariables)
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

    /// <summary>
    /// Aplica os cabeçalhos HTTP à requisição, incluindo parâmetros de cabeçalho do endpoint, cabeçalhos extras do caso de teste e o token de autenticação, se disponível.
    /// </summary>
    /// <param name="request">a requisição HTTP</param>
    /// <param name="endpoint">informações sobre o endpoint</param>
    /// <param name="testCase">o caso de teste</param>
    /// <param name="sessionVariables">as variáveis de sessão</param>
    private static void ApplyHeaders(HttpRequestMessage request, EndpointInfo endpoint, TestCase testCase,IReadOnlyDictionary<string, string> sessionVariables)
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

    /// <summary>
    /// Obtém o valor de um parâmetro, primeiro verificando os valores fornecidos no caso de teste e, se não encontrado, retornando null. Se o valor for encontrado, ele é resolvido para substituir quaisquer variáveis de sessão presentes.
    /// </summary>
    /// <param name="name">o nome do parâmetro</param>
    /// <param name="testCase">o caso de teste</param>
    /// <param name="sessionVariables">as variáveis de sessão</param>
    /// <returns></returns>
    private static string? GetParameterValue(string name, TestCase testCase, IReadOnlyDictionary<string, string> sessionVariables)
    {
        if (testCase.ParameterValues.TryGetValue(name, out var value) && value is not null)
        {
            var stringValue = value.ToString();
            return stringValue is null ? null : ResolveVariables(stringValue, sessionVariables);
        }

        return null;
    }

    /// <summary>
    /// Resolve variáveis de sessão no texto fornecido, substituindo ocorrências de {{variableName}} pelos valores correspondentes nas variáveis de sessão.
    /// </summary>
    /// <param name="text">o texto a ser resolvido</param>
    /// <param name="sessionVariables">as variáveis de sessão</param>
    /// <returns></returns>
    private static string ResolveVariables(string text, IReadOnlyDictionary<string, string> sessionVariables)
    {
        foreach (var (key, value) in sessionVariables)
            text = text.Replace($"{{{{{key}}}}}", value);

        return text;
    }

    /// <summary>
    /// Método auxiliar para mapear o tipo de método HTTP definido no enum HttpMethodType para a classe HttpMethod do .NET.
    /// </summary>
    /// <param name="method">o tipo de método HTTP</param>
    /// <returns></returns>
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

    /// <summary>
    /// Constrói o conteúdo do corpo da requisição HTTP com base no tipo de conteúdo e no corpo fornecido. Suporta "application/x-www-form-urlencoded", "multipart/form-data" e outros tipos de conteúdo como texto simples.
    /// </summary>
    /// <param name="contentType">O tipo de conteúdo da requisição</param>
    /// <param name="body">O corpo da requisição</param>
    /// <returns></returns>
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

    /// <summary>
    /// Analisa linhas de texto no formato "chave=valor" e retorna uma coleção de tuplas (Key, Value). Linhas vazias são ignoradas. Espaços em branco ao redor das chaves e valores são removidos.
    /// </summary>
    /// <param name="text">O texto a ser analisado</param>
    /// <returns></returns>
    private static IEnumerable<(string Key, string Value)> ParseKeyValueLines(string text)
    {
        foreach (var line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
                yield return (parts[0].Trim(), parts[1].Trim());
        }
    }
    #endregion
}