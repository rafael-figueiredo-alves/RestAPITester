namespace RestAPITester.Core.Models;

/// <summary>
/// Representa uma resposta possível descrita no OpenAPI para um endpoint (ex: 200, 400, 404).
/// </summary>
public class ResponseInfo
{
    /// <summary>
    /// Código de status HTTP da resposta (ex: "200", "default", etc.).
    /// </summary>
    public string StatusCode { get; set; } = string.Empty; // "200", "default", etc.
    
    /// <summary>
    /// Descrição da resposta.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Tipo de conteúdo da resposta.
    /// </summary>
    public string? ContentType { get; set; }
    
    /// <summary>
    /// Schema da resposta em formato JSON Schema.
    /// </summary>
    public string? SchemaJson { get; set; }
}