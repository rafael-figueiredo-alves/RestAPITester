using RestAPITester.Core.Models;

/// <summary>
/// Resultado do parsing de um documento OpenAPI: os endpoints extraídos
/// mais metadados úteis para exibir na UI (título, versão, servidores, erros de validação).
/// </summary>
public class ParseResult
{
    public string Title { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public List<string> Servers { get; set; } = new();
    public List<EndpointInfo> Endpoints { get; set; } = new();
    public bool HasErrors { get; set; }
    public List<string> Errors { get; set; } = new();
}