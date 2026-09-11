using IndexedDB.Blazor;
using RestAPITester.Core.Models;
using RestAPITester.Core.Persistence;

namespace RestAPITester.Storage.IndexedDb;

public class IndexedDbTestSuiteRepository : ITestSuiteRepository
{
    private readonly IIndexedDbFactory _factory;

    public IndexedDbTestSuiteRepository(IIndexedDbFactory factory)
    {
        _factory = factory;
    }

    private Task<RestApiTesterDb> GetDbAsync() => _factory.Create<RestApiTesterDb>("RestAPITesterDb", 3);

    public async Task SaveAsync(TestSuite suite)
    {
        var db = await GetDbAsync();
        db.TestSuites.Add(suite);
        await db.SaveChanges();
    }

    /// <summary>
    /// Substitui o registro existente (remove + adiciona de novo), garantindo que
    /// alterações na lista de TestCases sejam persistidas sem duplicar a suíte.
    /// </summary>
    public async Task UpdateAsync(TestSuite suite)
    {
        var db = await GetDbAsync();
        var existing = db.TestSuites.FirstOrDefault(s => s.Id == suite.Id);
        if (existing is not null)
            db.TestSuites.Remove(existing);

        db.TestSuites.Add(suite);
        await db.SaveChanges();
    }

    public async Task<List<TestSuite>> GetAllAsync()
    {
        var db = await GetDbAsync();
        return db.TestSuites.ToList();
    }

    public async Task DeleteAsync(Guid id)
    {
        var db = await GetDbAsync();
        var existing = db.TestSuites.FirstOrDefault(s => s.Id == id);
        if (existing is not null)
        {
            db.TestSuites.Remove(existing);
            await db.SaveChanges();
        }
    }
}