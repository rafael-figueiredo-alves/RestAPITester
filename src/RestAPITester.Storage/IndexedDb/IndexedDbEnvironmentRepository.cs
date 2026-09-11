using IndexedDB.Blazor;
using RestAPITester.Core.Models;
using RestAPITester.Core.Persistence;

namespace RestAPITester.Storage.IndexedDb;

public class IndexedDbEnvironmentRepository : IEnvironmentRepository
{
    private readonly IIndexedDbFactory _factory;

    public IndexedDbEnvironmentRepository(IIndexedDbFactory factory)
    {
        _factory = factory;
    }

    private Task<RestApiTesterDb> GetDbAsync() => _factory.Create<RestApiTesterDb>("RestAPITesterDb", 3);

    public async Task SaveAsync(ApiEnvironment environment)
    {
        var db = await GetDbAsync();
        db.Environments.Add(environment);
        await db.SaveChanges();
    }

    public async Task<List<ApiEnvironment>> GetAllAsync()
    {
        var db = await GetDbAsync();
        return db.Environments.ToList();
    }

    public async Task DeleteAsync(Guid id)
    {
        var db = await GetDbAsync();
        var existing = db.Environments.FirstOrDefault(e => e.Id == id);
        if (existing is not null)
        {
            db.Environments.Remove(existing);
            await db.SaveChanges();
        }
    }
}