using RestAPITester.Core.EnumsAndConstants;

namespace RestAPITester.Core.Models;

/// <summary>
/// Represents a validation issue found in a workflow.
/// </summary>
public class WorkflowValidationIssue
{
    /// <summary>
    /// Gets or sets the severity of the validation issue.
    /// </summary>
    public ValidationSeverity Severity { get; set; }

    /// <summary>
    /// Gets or sets the message describing the validation issue.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the node associated with the validation issue, if applicable.
    /// </summary>
    public Guid? NodeId { get; set; }
}