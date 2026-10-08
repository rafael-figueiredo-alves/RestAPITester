using RestAPITester.Core.EnumsAndConstants;

namespace RestAPITester.Core.Models;

/// <summary>
/// Representa um endpoint extraído do documento OpenAPI (um path + method).
/// </summary>
public class EndpointInfo
{
    /// <summary>
    /// Identificador único do endpoint, conforme definido na especificação OpenAPI.
    /// </summary>
    public string OperationId { get; set; } = string.Empty;

    /// <summary>
    /// Caminho do endpoint, conforme definido na especificação OpenAPI.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Método HTTP do endpoint, conforme definido na especificação OpenAPI.
    /// </summary>
    public HttpMethodType Method { get; set; }

    /// <summary>
    /// Resumo do endpoint, conforme definido na especificação OpenAPI.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Descrição detalhada do endpoint, conforme definido na especificação OpenAPI.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tags associadas ao endpoint, conforme definido na especificação OpenAPI.
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Parâmetros do endpoint, conforme definido na especificação OpenAPI.
    /// </summary>
    public List<ParameterInfo> Parameters { get; set; } = new();

    /// <summary>
    /// Corpo da requisição do endpoint, conforme definido na especificação OpenAPI.
    /// </summary>
    public RequestBodyInfo? RequestBody { get; set; }

    /// <summary>
    /// Respostas do endpoint, conforme definido na especificação OpenAPI.
    /// </summary>
    public List<ResponseInfo> Responses { get; set; } = new();

    /// <summary>
    /// Indica se o endpoint requer autenticação, conforme definido na especificação OpenAPI.
    /// </summary>
    public bool RequiresAuth { get; set; }

    /// <summary>
    /// Identificador único e estável do endpoint (Método + Path) — mais confiável
    /// que o operationId da própria spec, que não é garantido ser único.
    /// </summary>
    public string Key => $"{Method.ToString().ToUpperInvariant()} {Path}";
}