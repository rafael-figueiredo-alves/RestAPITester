using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using RestAPITester.Core.Models;

namespace RestAPITester.Web.Workflow;

public class LoopNodeModel : NodeModel
{
    public LoopNodeModel(Point position, WorkflowNode? workflowNode = null) : base(position)
    {
        WorkflowNode = workflowNode ?? new WorkflowNode
        {
            Type = WorkflowNodeType.Loop,
            Title = "Repetir",
            LoopMaxIterations = 5
        };

        Title = WorkflowNode.Title;
        Size = new Size(220, 70);

        AddPort(PortAlignment.Left); // entrada (inclusive a volta do corpo do loop)
        BodyPort = AddPort(PortAlignment.Right);
        ExitPort = AddPort(PortAlignment.Bottom);
    }

    public WorkflowNode WorkflowNode { get; }
    public PortModel BodyPort { get; }
    public PortModel ExitPort { get; }
    public bool IsExecuting { get; set; }
    public bool ExecutionSucceeded { get; set; }

    public string LoopSummary => $"até {WorkflowNode.LoopMaxIterations}x";

    public Action? OnEditRequested { get; set; }
}