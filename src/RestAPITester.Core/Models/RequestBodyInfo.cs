namespace RestAPITester.Core.Models;

/// <summary>
/// Representa o corpo esperado de uma requisição, com o schema em JSON Schema
/// (extraído do OpenApi.Readers) para orientar a geração de dados fictícios.
/// </summary>
public class RequestBodyInfo
{
    /// <summary>
    /// Indica se o corpo da requisição é obrigatório ou não.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Indica o tipo de conteúdo do corpo da requisição, como "application/json", "application/xml", etc.
    /// </summary>
    public string ContentType { get; set; } = "application/json";

    /// <summary>
    /// Indica o schema do corpo da requisição em formato JSON Schema, usado para validação e geração de dados fictícios.
    /// </summary>
    public string? SchemaJson { get; set; } // schema serializado, usado pelo DataGeneration

    /// <summary>
    /// Indica um exemplo de corpo da requisição, que pode ser usado para testes ou documentação.
    /// </summary>
    public object? Example { get; set; }
}