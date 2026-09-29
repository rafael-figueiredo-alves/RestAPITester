namespace RestAPITester.Core.Models;

public class WorkflowNodeExecutionResult
{
    public Guid NodeId { get; set; }
    public string NodeTitle { get; set; } = string.Empty;
    public WorkflowNodeType NodeType { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public ExecutionResult? ApiCallResult { get; set; } // preenchido só pra nós ApiCall
    public long DurationMs { get; set; }
}