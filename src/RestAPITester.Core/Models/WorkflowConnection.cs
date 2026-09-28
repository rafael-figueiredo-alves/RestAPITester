namespace RestAPITester.Core.Models;

/// <summary>Uma seta ligando dois nós do fluxo.</summary>
public class WorkflowConnection
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SourceNodeId { get; set; }
    public Guid TargetNodeId { get; set; }

    /// <summary>Diferencia o caminho "verdadeiro"/"falso" de um nó de Condition. Vazio nos demais casos.</summary>
    public string? Label { get; set; }
}