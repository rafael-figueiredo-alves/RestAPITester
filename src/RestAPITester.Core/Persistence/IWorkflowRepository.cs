using RestAPITester.Core.Models;

namespace RestAPITester.Core.Persistence;

/// <summary>
/// Represents a repository for managing workflow definitions.
/// </summary>
public interface IWorkflowRepository
{
    /// <summary>
    /// Saves a new workflow definition to the repository.
    /// </summary>
    /// <param name="workflow">The workflow definition to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(WorkflowDefinition workflow);
   
    /// <summary>
    /// Updates an existing workflow definition in the repository.
    /// </summary>
    /// <param name="workflow">The workflow definition to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(WorkflowDefinition workflow);
    
    /// <summary>
    /// Retrieves all workflow definitions from the repository asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<List<WorkflowDefinition>> GetAllAsync();
    
    /// <summary>
    /// Deletes a workflow definition with the specified ID from the repository asynchronously.
    /// </summary>
    /// <param name="id">The ID of the workflow definition to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
}