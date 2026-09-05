namespace RestAPITester.Core.Models;

/// <summary>
/// Resultado da execução de um único TestCase.
/// </summary>
public class ExecutionResult
{
    public Guid TestCaseId { get; set; }
    public int? StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public Dictionary<string, string> ResponseHeaders { get; set; } = new();
    public long DurationMs { get; set; }
    public bool Success { get; set; }
    public List<AssertionResult> AssertionResults { get; set; } = new();
    public string? ErrorMessage { get; set; } // ex: erro de rede, timeout
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}