using System.Diagnostics;
using System.Net.Http.Headers;

// Sistema de proxy simples para testar APIs REST. Ele redireciona todas as requisições para o endpoint /proxy, que por sua vez encaminha a requisição para o destino especificado no parâmetro 'target' da query string.

var builder = WebApplication.CreateBuilder(args);

// Configura o Kestrel para escutar na porta 5218
builder.WebHost.UseUrls("http://localhost:5218");

// Configura dois clientes HTTP: um que segue redirecionamentos e outro que não segue.
builder.Services.AddHttpClient("proxy-client-redirect")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = true });

builder.Services.AddHttpClient("proxy-client-noredirect")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Define os métodos HTTP que o proxy aceitará
var verbs = new[] { "GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS" };

// Rota do proxy: redireciona a requisição para o destino especificado no parâmetro 'target' da query string
app.MapMethods("/proxy", verbs, async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    var targetUrl = context.Request.Query["target"].ToString();

    if (string.IsNullOrWhiteSpace(targetUrl) || !Uri.TryCreate(targetUrl, UriKind.Absolute, out var targetUri))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Parâmetro 'target' ausente ou inválido.");
        return;
    }

    var followRedirects = !string.Equals(context.Request.Query["followRedirects"], "false", StringComparison.OrdinalIgnoreCase);

    using var forwardRequest = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUri);

    if (context.Request.ContentLength is > 0)
    {
        forwardRequest.Content = new StreamContent(context.Request.Body);
        if (context.Request.ContentType is not null)
            forwardRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(context.Request.ContentType);
    }

    foreach (var header in context.Request.Headers)
    {
        if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase)) continue;
        if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)) continue;
        if (header.Key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase)) continue;

        if (!forwardRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
            forwardRequest.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
    }

    var client = httpClientFactory.CreateClient(followRedirects ? "proxy-client-redirect" : "proxy-client-noredirect");
    using var response = await client.SendAsync(forwardRequest, HttpCompletionOption.ResponseHeadersRead);

    context.Response.StatusCode = (int)response.StatusCode;

    foreach (var header in response.Headers)
        context.Response.Headers[header.Key] = header.Value.ToArray();
    foreach (var header in response.Content.Headers)
        context.Response.Headers[header.Key] = header.Value.ToArray();

    context.Response.Headers.Remove("transfer-encoding");

    await response.Content.CopyToAsync(context.Response.Body);
});

// Roteamento client-side do Blazor: qualquer rota não reconhecida cai no index.html
app.MapFallbackToFile("index.html");

// Assim que o Kestrel sinalizar que já está escutando, abre o navegador sozinho.
app.Lifetime.ApplicationStarted.Register(() =>
{
    Process.Start(new ProcessStartInfo("http://localhost:5218") { UseShellExecute = true });
});

app.Run();