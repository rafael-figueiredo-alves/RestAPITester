namespace RestAPITester.Core.Models;

/// <summary>
/// Resultado agregado da execução de uma TestSuite inteira.
/// </summary>
public class TestSuiteExecutionResult
{
    public Guid TestSuiteId { get; set; }
    public List<ExecutionResult> Results { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public bool AllPassed => Results.All(r => r.Success);
}