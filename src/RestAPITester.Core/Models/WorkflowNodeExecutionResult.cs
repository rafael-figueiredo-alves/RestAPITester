using RestAPITester.Core.EnumsAndConstants;

namespace RestAPITester.Core.Models;

/// <summary>
/// Represents the result of executing a workflow node, including its ID, title, type, success status, error message (if any), API call result (if applicable), execution duration, and condition result.   
/// </summary>
public class WorkflowNodeExecutionResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the workflow node.
    /// </summary>
    public Guid NodeId { get; set; }

    /// <summary>
    /// Gets or sets the title of the workflow node.
    /// </summary>
    public string NodeTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the workflow node, represented by the WorkflowNodeType enumeration.
    /// </summary>
    public WorkflowNodeType NodeType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the execution of the workflow node was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message associated with the execution of the workflow node, if any. This property is null if the execution was successful.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the result of the API call associated with the workflow node execution, if applicable. This property is null if the node type is not an API call.
    /// </summary>
    public ExecutionResult? ApiCallResult { get; set; } // preenchido só pra nós ApiCall

    /// <summary>
    /// Gets or sets the duration of the workflow node execution in milliseconds.
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Gets or sets the result of the condition evaluation for the workflow node, if applicable. This property is null if the node type is not a condition.
    /// </summary>
    public bool? ConditionResult { get; set; }
}