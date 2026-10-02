using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using RestAPITester.Core.Models;

namespace RestAPITester.Web.Workflow;

public class SetVariableNodeModel : NodeModel
{
    public SetVariableNodeModel(Point position, WorkflowNode? workflowNode = null) : base(position)
    {
        WorkflowNode = workflowNode ?? new WorkflowNode
        {
            Type = WorkflowNodeType.SetVariable,
            Title = "Definir variável"
        };

        Title = WorkflowNode.Title;
        Size = new Size(200, 60);

        AddPort(PortAlignment.Left);
        AddPort(PortAlignment.Right);
        AddPort(PortAlignment.Top);
        AddPort(PortAlignment.Bottom);
    }

    public WorkflowNode WorkflowNode { get; }
    public bool IsExecuting { get; set; }
    public bool ExecutionSucceeded { get; set; }

    public string Summary => string.IsNullOrWhiteSpace(WorkflowNode.SetVariableName)
        ? "(não configurado)"
        : $"{WorkflowNode.SetVariableName} = {WorkflowNode.SetVariableValue}";

    public Action? OnEditRequested { get; set; }
}