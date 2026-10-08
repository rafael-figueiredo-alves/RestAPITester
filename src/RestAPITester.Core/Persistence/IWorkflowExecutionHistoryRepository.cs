using RestAPITester.Core.Models;

namespace RestAPITester.Core.Persistence;

/// <summary>
/// Interface for managing workflow execution history in the persistence layer.
/// </summary>
public interface IWorkflowExecutionHistoryRepository
{
    /// <summary>
    /// Saves the workflow execution result to the persistence layer.
    /// </summary>
    /// <param name="result">The workflow execution result to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(WorkflowExecutionResult result);
    
    /// <summary>
    /// Retrieves workflow execution results for a specific workflow asynchronously.
    /// </summary>
    /// <param name="workflowId">The ID of the workflow for which to retrieve execution results.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<List<WorkflowExecutionResult>> GetByWorkflowAsync(Guid workflowId);
}