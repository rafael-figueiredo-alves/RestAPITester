namespace RestAPITester.Core.Models;

/// <summary>
/// Representa o corpo esperado de uma requisição, com o schema em JSON Schema
/// (extraído do OpenApi.Readers) para orientar a geração de dados fictícios.
/// </summary>
public class RequestBodyInfo
{
    public bool Required { get; set; }
    public string ContentType { get; set; } = "application/json";
    public string? SchemaJson { get; set; } // schema serializado, usado pelo DataGeneration
    public object? Example { get; set; }
}