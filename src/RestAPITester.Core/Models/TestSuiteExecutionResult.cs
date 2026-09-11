namespace RestAPITester.Core.Models;

public class TestSuiteExecutionResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TestSuiteId { get; set; }
    public List<ExecutionResult> Results { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public bool AllPassed => Results.All(r => r.Success);
}