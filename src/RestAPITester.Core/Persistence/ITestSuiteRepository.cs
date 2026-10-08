using RestAPITester.Core.Models;

namespace RestAPITester.Core.Persistence;

/// <summary>
/// Represents a repository for managing test suites.
/// </summary>
public interface ITestSuiteRepository
{
    /// <summary>
    /// Saves a test suite asynchronously.
    /// </summary>
    /// <param name="suite">The test suite to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(TestSuite suite);
    
    /// <summary>
    /// Updates a test suite asynchronously.
    /// </summary>
    /// <param name="suite">The test suite to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(TestSuite suite);
    
    /// <summary>
    /// Retrieves all test suites asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<List<TestSuite>> GetAllAsync();
    
    /// <summary>
    /// Deletes a test suite with the specified ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the test suite to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
}