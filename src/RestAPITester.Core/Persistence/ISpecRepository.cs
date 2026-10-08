using RestAPITester.Core.Models;

namespace RestAPITester.Core.Persistence;

/// <summary>
/// Interface for a repository that manages the persistence of SavedSpec objects.
/// </summary>
public interface ISpecRepository
{
    /// <summary>
    /// Saves a SavedSpec object to the repository asynchronously.
    /// </summary>
    /// <param name="spec">The SavedSpec object to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(SavedSpec spec);
    
    /// <summary>
    /// Retrieves all SavedSpec objects from the repository asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<List<SavedSpec>> GetAllAsync();
    
    /// <summary>
    /// Deletes a SavedSpec object with the specified ID from the repository asynchronously.
    /// </summary>
    /// <param name="id">The ID of the SavedSpec object to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
}