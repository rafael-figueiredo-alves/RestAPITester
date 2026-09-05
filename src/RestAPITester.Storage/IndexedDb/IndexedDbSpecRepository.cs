using IndexedDB.Blazor;
using RestAPITester.Core.Models;
using RestAPITester.Core.Persistence;

namespace RestAPITester.Storage.IndexedDb;

public class IndexedDbSpecRepository : ISpecRepository
{
    private readonly IIndexedDbFactory _factory;

    public IndexedDbSpecRepository(IIndexedDbFactory factory)
    {
        _factory = factory;
    }

    private Task<RestApiTesterDb> GetDbAsync() => _factory.Create<RestApiTesterDb>("RestAPITesterDb", 1);

    public async Task SaveAsync(SavedSpec spec)
    {
        var db = await GetDbAsync();
        db.Specs.Add(spec);
        await db.SaveChanges();
    }

    public async Task<List<SavedSpec>> GetAllAsync()
    {
        var db = await GetDbAsync();
        return db.Specs.ToList();
    }

    public async Task DeleteAsync(Guid id)
    {
        var db = await GetDbAsync();
        var existing = db.Specs.FirstOrDefault(s => s.Id == id);
        if (existing is not null)
        {
            db.Specs.Remove(existing);
            await db.SaveChanges();
        }
    }
}