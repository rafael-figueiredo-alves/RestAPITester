using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// CORS totalmente aberto de propósito: isso é uma ferramenta de dev local,
// só o seu próprio Blazor (rodando em outra porta local) vai chamá-la.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddHttpClient("proxy-client")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });

var app = builder.Build();
app.UseCors();

var verbs = new[] { "GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS" };

app.MapMethods("/proxy", verbs, async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    var targetUrl = context.Request.Query["target"].ToString();

    if (string.IsNullOrWhiteSpace(targetUrl) || !Uri.TryCreate(targetUrl, UriKind.Absolute, out var targetUri))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Parâmetro 'target' ausente ou inválido.");
        return;
    }

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

    var client = httpClientFactory.CreateClient("proxy-client");
    using var response = await client.SendAsync(forwardRequest, HttpCompletionOption.ResponseHeadersRead);

    context.Response.StatusCode = (int)response.StatusCode;

    foreach (var header in response.Headers)
        context.Response.Headers[header.Key] = header.Value.ToArray();
    foreach (var header in response.Content.Headers)
        context.Response.Headers[header.Key] = header.Value.ToArray();

    context.Response.Headers.Remove("transfer-encoding");

    await response.Content.CopyToAsync(context.Response.Body);
});

app.Run();