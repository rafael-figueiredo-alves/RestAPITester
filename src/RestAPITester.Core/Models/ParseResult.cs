using RestAPITester.Core.Models;

/// <summary>
/// Resultado do parsing de um documento OpenAPI: os endpoints extraídos
/// mais metadados úteis para exibir na UI (título, versão, servidores, erros de validação).
/// </summary>
public class ParseResult
{
    /// <summary>
    /// Título do documento OpenAPI (campo "info.title").
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Versão do documento OpenAPI (campo "info.version").
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Lista de servidores definidos no documento OpenAPI (campo "servers").
    /// </summary>
    public List<string> Servers { get; set; } = new();

    /// <summary>
    /// Lista de endpoints extraídos do documento OpenAPI, com informações detalhadas sobre cada um.
    /// </summary>
    public List<EndpointInfo> Endpoints { get; set; } = new();

    /// <summary>
    /// Indica se houve erros durante o parsing do documento OpenAPI.
    /// </summary>
    public bool HasErrors { get; set; }

    /// <summary>
    /// Lista de mensagens de erro geradas durante o parsing do documento OpenAPI.
    /// </summary>
    public List<string> Errors { get; set; } = new();
}