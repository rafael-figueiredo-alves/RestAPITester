using RestAPITester.Core.Models;

namespace RestAPITester.Core.Persistence;

public interface IExecutionHistoryRepository
{
    Task SaveAsync(TestSuiteExecutionResult result);
    Task<List<TestSuiteExecutionResult>> GetBySuiteAsync(Guid suiteId);
}