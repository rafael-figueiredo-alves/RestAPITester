namespace RestAPITester.Core.Models;

public enum ValidationSeverity { Warning, Error }

public class WorkflowValidationIssue
{
    public ValidationSeverity Severity { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? NodeId { get; set; }
}