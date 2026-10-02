namespace RestAPITester.Core.Models;

public class WorkflowExecutionResult
{
    public Guid WorkflowId { get; set; }
    public List<WorkflowNodeExecutionResult> NodeResults { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public bool AllPassed => NodeResults.All(r => r.Success);

    public Guid Id { get; set; } = Guid.NewGuid();
}