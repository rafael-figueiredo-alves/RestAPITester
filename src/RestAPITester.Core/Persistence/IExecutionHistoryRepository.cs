using RestAPITester.Core.Models;

namespace RestAPITester.Core.Persistence;

/// <summary>
/// Interface for managing execution history in the persistence layer.
/// </summary>
public interface IExecutionHistoryRepository
{
    /// <summary>
    /// Saves the execution result of a test suite to the persistence layer.
    /// </summary>
    /// <param name="result">The execution result to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(TestSuiteExecutionResult result);
    
    /// <summary>
    /// Retrieves execution results for a specific test suite from the persistence layer.
    /// </summary>
    /// <param name="suiteId">The ID of the test suite.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<List<TestSuiteExecutionResult>> GetBySuiteAsync(Guid suiteId);
}