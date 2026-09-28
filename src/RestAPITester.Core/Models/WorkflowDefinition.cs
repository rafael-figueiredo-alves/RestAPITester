namespace RestAPITester.Core.Models;

/// <summary>
/// Um fluxo visual que orquestra chamadas de API e outras ações em sequência/condição.
/// Independente de qualquer biblioteca de desenho — isso é só o dado do fluxo.
/// </summary>
public class WorkflowDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid SourceSpecId { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public List<WorkflowNode> Nodes { get; set; } = new();
    public List<WorkflowConnection> Connections { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastRunAt { get; set; }
}