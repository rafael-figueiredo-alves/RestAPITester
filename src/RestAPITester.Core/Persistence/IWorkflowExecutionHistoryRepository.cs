using RestAPITester.Core.Models;

namespace RestAPITester.Core.Persistence;

public interface IWorkflowExecutionHistoryRepository
{
    Task SaveAsync(WorkflowExecutionResult result);
    Task<List<WorkflowExecutionResult>> GetByWorkflowAsync(Guid workflowId);
}