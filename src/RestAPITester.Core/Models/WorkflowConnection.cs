namespace RestAPITester.Core.Models;

/// <summary>Uma seta ligando dois nós do fluxo.</summary>
public class WorkflowConnection
{
    /// <summary>
    /// Identificador único da conexão. Gerado automaticamente ao criar a conexão.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identificador do nó de origem da conexão. Deve ser um dos nós do fluxo.
    /// </summary>
    public Guid SourceNodeId { get; set; }

    /// <summary>
    /// Identificador do nó de destino da conexão. Deve ser um dos nós do fluxo.
    /// </summary>
    public Guid TargetNodeId { get; set; }

    /// <summary>Diferencia o caminho "verdadeiro"/"falso" de um nó de Condition. Vazio nos demais casos.</summary>
    public string? Label { get; set; }
}