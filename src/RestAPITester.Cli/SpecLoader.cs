using System.Diagnostics.CodeAnalysis;
using RestAPITester.Core.Models;
using RestAPITester.Core.OpenApi;

namespace RestAPITester.Cli;

public sealed record LoadedSpecs(List<EndpointInfo> Endpoints, string? FirstServerUrl)
{
    /// <summary>Precedência: --base-url, depois a URL salva na suíte/fluxo, depois o primeiro servidor da spec.</summary>
    public string ResolveBaseUrl(string? cliOverride, string? savedUrl) =>
        !string.IsNullOrWhiteSpace(cliOverride) ? cliOverride
        : !string.IsNullOrWhiteSpace(savedUrl) ? savedUrl
        : FirstServerUrl ?? string.Empty;
}

public static class SpecLoader
{
    public static bool TryLoad(
        IEnumerable<string> paths,
        [NotNullWhen(true)] out LoadedSpecs? loaded,
        [NotNullWhen(false)] out string? error)
    {
        var endpoints = new List<EndpointInfo>();
        string? firstServer = null;
        var parser = new OpenApiParser();

        foreach (var path in paths)
        {
            if (!File.Exists(path))
            {
                loaded = null;
                error = $"Arquivo de spec não encontrado: {path}";
                return false;
            }

            var parsed = parser.Parse(File.ReadAllText(path));
            if (parsed.HasErrors)
            {
                loaded = null;
                error = $"Erro ao ler a spec '{path}': {string.Join("; ", parsed.Errors)}";
                return false;
            }

            firstServer ??= parsed.Servers.FirstOrDefault();
            endpoints.AddRange(parsed.Endpoints);
        }

        loaded = new LoadedSpecs(endpoints, firstServer);
        error = null;
        return true;
    }
}