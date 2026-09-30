using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using RestAPITester.Core.Models;

namespace RestAPITester.Web.Workflow;

public class DelayNodeModel : NodeModel
{
    public DelayNodeModel(Point position, WorkflowNode? workflowNode = null) : base(position)
    {
        WorkflowNode = workflowNode ?? new WorkflowNode
        {
            Type = WorkflowNodeType.Delay,
            Title = "Aguardar"
        };

        Title = WorkflowNode.Title;
        Size = new Size(180, 60);

        AddPort(PortAlignment.Left);
        AddPort(PortAlignment.Right);
        AddPort(PortAlignment.Top);
        AddPort(PortAlignment.Bottom);
    }

    public WorkflowNode WorkflowNode { get; }
    public bool IsExecuting { get; set; }
    public bool ExecutionSucceeded { get; set; }
    public string DelaySummary => $"{WorkflowNode.DelayMilliseconds} ms";
    public Action? OnEditRequested { get; set; }
}