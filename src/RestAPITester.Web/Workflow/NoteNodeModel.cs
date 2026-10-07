using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using RestAPITester.Core.Models;

namespace RestAPITester.Web.Workflow;

/// <summary>
/// Um "post-it" livre no canvas — não tem porta (não pode ser conectado)
/// e nunca é executado pelo WorkflowRunner.
/// </summary>
public class NoteNodeModel : NodeModel
{
    public NoteNodeModel(Point position, WorkflowNode? workflowNode = null) : base(position)
    {
        WorkflowNode = workflowNode ?? new WorkflowNode
        {
            Type = WorkflowNodeType.Note,
            Title = "Anotação",
            NoteText = "Escreva aqui..."
        };

        Title = WorkflowNode.Title;
        Size = new Size(200, 120);
    }

    public WorkflowNode WorkflowNode { get; }
    public Action? OnEditRequested { get; set; }
}