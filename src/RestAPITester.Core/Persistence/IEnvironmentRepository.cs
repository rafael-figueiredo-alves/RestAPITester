using RestAPITester.Core.Models;

namespace RestAPITester.Core.Persistence;

/// <summary>
/// Interface for managing API environments in the persistence layer.
/// </summary>
public interface IEnvironmentRepository
{
    /// <summary>
    /// Saves the specified API environment to the persistence layer.
    /// </summary>
    /// <param name="environment">The API environment to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(ApiEnvironment environment);

    /// <summary>
    /// Retrieves all API environments from the persistence layer.
    /// </summary>
    /// <returns></returns>
    Task<List<ApiEnvironment>> GetAllAsync();

    /// <summary>
    /// Deletes the API environment with the specified ID from the persistence layer.
    /// </summary>
    /// <param name="id">The ID of the API environment to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
}