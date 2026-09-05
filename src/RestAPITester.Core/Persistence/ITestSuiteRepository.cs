using RestAPITester.Core.Models;

namespace RestAPITester.Core.Persistence;

public interface ITestSuiteRepository
{
    Task SaveAsync(TestSuite suite);
    Task UpdateAsync(TestSuite suite);
    Task<List<TestSuite>> GetAllAsync();
    Task DeleteAsync(Guid id);
}