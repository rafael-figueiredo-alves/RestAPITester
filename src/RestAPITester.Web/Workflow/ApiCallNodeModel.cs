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

        AddPort(PortAlignment.Left);
        AddPort(PortAlignment.Right);
        AddPort(PortAlignment.Top);
        AddPort(PortAlignment.Bottom);
    }

    public WorkflowNode WorkflowNode { get; }
    public string EndpointLabel => WorkflowNode.ApiCallTestCase?.Name ?? "(nenhum endpoint selecionado)";
    public HttpMethodType? EndpointMethod
    {
        get
        {
            var name = WorkflowNode.ApiCallTestCase?.Name;
            var methodText = name?.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            return Enum.TryParse<HttpMethodType>(methodText, ignoreCase: true, out var method) ? method : null;
        }
    }
    public bool IsExecuting { get; set; }
    public bool ExecutionSucceeded { get; set; }

    /// <summary>Disparado pelo widget quando o usuário clica no ícone de editar.</summary>
    public Action? OnEditRequested { get; set; }
}