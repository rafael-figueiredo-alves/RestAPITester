namespace RestAPITester.Core.Models;

/// <summary>
/// Uma sequência ordenada de TestCases que representa um fluxo completo
/// (ex: login -> criar recurso -> consultar -> deletar).
/// </summary>
public class TestSuite
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid SourceSpecId { get; set; }
    public string SourceOpenApiFileName { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public List<TestCase> TestCases { get; set; } = new(); // ordem = ordem de execução
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastRunAt { get; set; }
}