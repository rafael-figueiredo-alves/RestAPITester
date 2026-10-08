namespace RestAPITester.Core.Models;

/// <summary>
/// Represents the result of executing a workflow, including the workflow ID, node execution results, start and finish times, and overall success status.
/// </summary>
public class WorkflowExecutionResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the workflow that was executed.
    /// </summary>
    public Guid WorkflowId { get; set; }

    /// <summary>
    /// Gets or sets the list of node execution results for the workflow. Each node result contains information about the execution of a specific node in the workflow.
    /// </summary>
    public List<WorkflowNodeExecutionResult> NodeResults { get; set; } = new();

    /// <summary>
    /// Gets or sets the timestamp indicating when the workflow execution started.
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp indicating when the workflow execution finished.
    /// </summary>
    public DateTime FinishedAt { get; set; }

    /// <summary>
    /// Gets a value indicating whether all node executions in the workflow were successful. Returns true if all nodes passed; otherwise, false.
    /// </summary>
    public bool AllPassed => NodeResults.All(r => r.Success);

    /// <summary>
    /// Gets or sets the unique identifier for this workflow execution result instance.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
}