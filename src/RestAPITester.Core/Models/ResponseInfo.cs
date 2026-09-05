namespace RestAPITester.Core.Models;

/// <summary>
/// Representa uma resposta possível descrita no OpenAPI para um endpoint (ex: 200, 400, 404).
/// </summary>
public class ResponseInfo
{
    public string StatusCode { get; set; } = string.Empty; // "200", "default", etc.
    public string? Description { get; set; }
    public string? ContentType { get; set; }
    public string? SchemaJson { get; set; }
}