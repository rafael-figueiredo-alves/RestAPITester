namespace RestAPITester.Core.Models;

/// <summary>
/// Uma sequência ordenada de TestCases que representa um fluxo completo
/// (ex: login -> criar recurso -> consultar -> deletar).
/// </summary>
public class TestSuite
{
    /// <summary>
    /// Identificador único da TestSuite.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nome da TestSuite.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da TestSuite.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Identificador do OpenAPI Specification (Swagger) que originou a TestSuite.
    /// </summary>
    public Guid SourceSpecId { get; set; }

    /// <summary>
    /// Nome do arquivo OpenAPI Specification (Swagger) que originou a TestSuite.
    /// </summary>
    public string SourceOpenApiFileName { get; set; } = string.Empty;

    /// <summary>
    /// URL base para execução dos TestCases da TestSuite.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Lista de TestCases que compõem a TestSuite, na ordem de execução.
    /// </summary>
    public List<TestCase> TestCases { get; set; } = new(); // ordem = ordem de execução

    /// <summary>
    /// Data e hora de criação da TestSuite.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data e hora da última execução da TestSuite.
    /// </summary>
    public DateTime? LastRunAt { get; set; }
}