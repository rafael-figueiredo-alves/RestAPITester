using RestAPITester.Core.Models;

namespace RestAPITester.Core.Persistence;

public interface IEnvironmentRepository
{
    Task SaveAsync(ApiEnvironment environment);
    Task<List<ApiEnvironment>> GetAllAsync();
    Task DeleteAsync(Guid id);
}