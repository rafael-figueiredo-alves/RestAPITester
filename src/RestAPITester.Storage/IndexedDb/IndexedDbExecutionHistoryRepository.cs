using IndexedDB.Blazor;
using RestAPITester.Core.Models;
using RestAPITester.Core.Persistence;

namespace RestAPITester.Storage.IndexedDb;

public class IndexedDbExecutionHistoryRepository : IExecutionHistoryRepository
{
    private const int MaxEntriesPerSuite = 20;
    private readonly IIndexedDbFactory _factory;

    public IndexedDbExecutionHistoryRepository(IIndexedDbFactory factory)
    {
        _factory = factory;
    }

    private Task<RestApiTesterDb> GetDbAsync() => _factory.Create<RestApiTesterDb>("RestAPITesterDb", 3);

    public async Task SaveAsync(TestSuiteExecutionResult result)
    {
        var db = await GetDbAsync();
        db.ExecutionHistory.Add(result);
        await db.SaveChanges();

        // Mantém só as N execuções mais recentes por suíte, pra não crescer sem limite.
        var entries = db.ExecutionHistory
            .Where(r => r.TestSuiteId == result.TestSuiteId)
            .OrderByDescending(r => r.StartedAt)
            .ToList();

        foreach (var old in entries.Skip(MaxEntriesPerSuite))
            db.ExecutionHistory.Remove(old);

        await db.SaveChanges();
    }

    public async Task<List<TestSuiteExecutionResult>> GetBySuiteAsync(Guid suiteId)
    {
        var db = await GetDbAsync();
        return db.ExecutionHistory
            .Where(r => r.TestSuiteId == suiteId)
            .OrderByDescending(r => r.StartedAt)
            .ToList();
    }
}