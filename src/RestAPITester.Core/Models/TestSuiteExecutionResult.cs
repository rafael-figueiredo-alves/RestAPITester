namespace RestAPITester.Core.Models;

/// <summary>
/// Represents the result of executing a test suite, including the execution results of individual tests, start and finish times, and overall success status.
/// </summary>
public class TestSuiteExecutionResult
{
    /// <summary>
    /// Gets or sets the unique identifier for the test suite execution result.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the unique identifier for the associated test suite.
    /// </summary>
    public Guid TestSuiteId { get; set; }

    /// <summary>
    /// Gets or sets the list of execution results for individual tests within the test suite.
    /// </summary>
    public List<ExecutionResult> Results { get; set; } = new();

    /// <summary>
    /// Gets or sets the timestamp indicating when the test suite execution started.
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp indicating when the test suite execution finished.
    /// </summary>
    public DateTime FinishedAt { get; set; }

    /// <summary>
    /// Gets a value indicating whether all tests in the test suite execution passed successfully.
    /// </summary>
    public bool AllPassed => Results.All(r => r.Success);
}