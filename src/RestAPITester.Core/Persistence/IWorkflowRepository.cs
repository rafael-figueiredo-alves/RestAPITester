using RestAPITester.Core.Models;

namespace RestAPITester.Core.Persistence;

public interface IWorkflowRepository
{
    Task SaveAsync(WorkflowDefinition workflow);
    Task UpdateAsync(WorkflowDefinition workflow);
    Task<List<WorkflowDefinition>> GetAllAsync();
    Task DeleteAsync(Guid id);
}