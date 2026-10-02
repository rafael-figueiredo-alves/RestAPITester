using IndexedDB.Blazor;
using RestAPITester.Core.Models;
using RestAPITester.Core.Persistence;

namespace RestAPITester.Storage.IndexedDb;

public class IndexedDbWorkflowExecutionHistoryRepository : IWorkflowExecutionHistoryRepository
{
    private const int MaxEntriesPerWorkflow = 20;
    private readonly IIndexedDbFactory _factory;

    public IndexedDbWorkflowExecutionHistoryRepository(IIndexedDbFactory factory)
    {
        _factory = factory;
    }

    private Task<RestApiTesterDb> GetDbAsync() => _factory.Create<RestApiTesterDb>(DatabaseConsts.DbName, DatabaseConsts.DbVersion);

    public async Task SaveAsync(WorkflowExecutionResult result)
    {
        var db = await GetDbAsync();
        db.WorkflowExecutionHistory.Add(result);
        await db.SaveChanges();

        var entries = db.WorkflowExecutionHistory
            .Where(r => r.WorkflowId == result.WorkflowId)
            .OrderByDescending(r => r.StartedAt)
            .ToList();

        foreach (var old in entries.Skip(MaxEntriesPerWorkflow))
            db.WorkflowExecutionHistory.Remove(old);

        await db.SaveChanges();
    }

    public async Task<List<WorkflowExecutionResult>> GetByWorkflowAsync(Guid workflowId)
    {
        var db = await GetDbAsync();
        return db.WorkflowExecutionHistory
            .Where(r => r.WorkflowId == workflowId)
            .OrderByDescending(r => r.StartedAt)
            .ToList();
    }
}