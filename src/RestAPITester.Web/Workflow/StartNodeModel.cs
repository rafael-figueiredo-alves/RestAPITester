using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using RestAPITester.Core.Models;

namespace RestAPITester.Web.Workflow;

public class StartNodeModel : NodeModel
{
    public StartNodeModel(Point position, WorkflowNode? workflowNode = null) : base(position)
    {
        WorkflowNode = workflowNode ?? new WorkflowNode
        {
            Type = WorkflowNodeType.Start,
            Title = "Início"
        };

        Title = WorkflowNode.Title;
        Size = new Size(140, 60);

        AddPort(PortAlignment.Left);
        AddPort(PortAlignment.Right);
        AddPort(PortAlignment.Top);
        AddPort(PortAlignment.Bottom);
    }

    public WorkflowNode WorkflowNode { get; }
    public bool IsExecuting { get; set; }
    public bool ExecutionSucceeded { get; set; }
}