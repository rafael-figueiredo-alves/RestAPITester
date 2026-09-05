using RestAPITester.Core.Models;

namespace RestAPITester.Core.Persistence;

public interface ISpecRepository
{
    Task SaveAsync(SavedSpec spec);
    Task<List<SavedSpec>> GetAllAsync();
    Task DeleteAsync(Guid id);
}