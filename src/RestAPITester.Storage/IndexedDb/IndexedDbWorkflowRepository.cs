using IndexedDB.Blazor;
using RestAPITester.Core.Models;
using RestAPITester.Core.Persistence;

namespace RestAPITester.Storage.IndexedDb;

public class IndexedDbWorkflowRepository : IWorkflowRepository
{
    private readonly IIndexedDbFactory _factory;

    public IndexedDbWorkflowRepository(IIndexedDbFactory factory)
    {
        _factory = factory;
    }

    private Task<RestApiTesterDb> GetDbAsync() => _factory.Create<RestApiTesterDb>("RestAPITesterDb", 3);

    public async Task SaveAsync(WorkflowDefinition workflow)
    {
        var db = await GetDbAsync();
        db.Workflows.Add(workflow);
        await db.SaveChanges();
    }

    public async Task UpdateAsync(WorkflowDefinition workflow)
    {
        var db = await GetDbAsync();
        var existing = db.Workflows.FirstOrDefault(w => w.Id == workflow.Id);
        if (existing is not null)
            db.Workflows.Remove(existing);

        db.Workflows.Add(workflow);
        await db.SaveChanges();
    }

    public async Task<List<WorkflowDefinition>> GetAllAsync()
    {
        var db = await GetDbAsync();
        return db.Workflows.ToList();
    }

    public async Task DeleteAsync(Guid id)
    {
        var db = await GetDbAsync();
        var existing = db.Workflows.FirstOrDefault(w => w.Id == id);
        if (existing is not null)
        {
            db.Workflows.Remove(existing);
            await db.SaveChanges();
        }
    }
}