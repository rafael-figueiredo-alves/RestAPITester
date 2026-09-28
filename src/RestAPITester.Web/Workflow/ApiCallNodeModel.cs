using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using RestAPITester.Core.Models;

namespace RestAPITester.Web.Workflow;

public class ApiCallNodeModel : NodeModel
{
    public ApiCallNodeModel(Point position, WorkflowNode? workflowNode = null) : base(position)
    {
        WorkflowNode = workflowNode ?? new WorkflowNode
        {
            Type = WorkflowNodeType.ApiCall,
            Title = "Chamada de API"
        };

        Title = WorkflowNode.Title;
        Size = new Size(220, 60);
    }

    public WorkflowNode WorkflowNode { get; }

    public string EndpointLabel => WorkflowNode.ApiCallTestCase?.Name ?? "(nenhum endpoint selecionado)";
}