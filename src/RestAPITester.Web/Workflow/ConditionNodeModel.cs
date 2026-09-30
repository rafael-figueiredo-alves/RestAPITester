using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using RestAPITester.Core.Models;

namespace RestAPITester.Web.Workflow;

public class ConditionNodeModel : NodeModel
{
    public ConditionNodeModel(Point position, WorkflowNode? workflowNode = null) : base(position)
    {
        WorkflowNode = workflowNode ?? new WorkflowNode
        {
            Type = WorkflowNodeType.Condition,
            Title = "Condição"
        };

        Title = WorkflowNode.Title;
        Size = new Size(220, 70);

        AddPort(PortAlignment.Left); // entrada
        TruePort = AddPort(PortAlignment.Right);
        FalsePort = AddPort(PortAlignment.Bottom);
    }

    public WorkflowNode WorkflowNode { get; }
    public PortModel TruePort { get; }
    public PortModel FalsePort { get; }
    public bool IsExecuting { get; set; }
    public bool ExecutionSucceeded { get; set; }

    public string ConditionSummary => string.IsNullOrWhiteSpace(WorkflowNode.ConditionJsonPath)
        ? "(condição não configurada)"
        : $"{WorkflowNode.ConditionJsonPath} = {WorkflowNode.ConditionExpectedValue}";

    public Action? OnEditRequested { get; set; }
}