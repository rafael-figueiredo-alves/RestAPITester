namespace RestAPITester.Core.Models;

/// <summary>
/// Um fluxo visual que orquestra chamadas de API e outras ações em sequência/condição.
/// Independente de qualquer biblioteca de desenho — isso é só o dado do fluxo.
/// </summary>
public class WorkflowDefinition
{
    /// <summary>
    /// Identificador único do fluxo.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nome do fluxo, usado para exibição e identificação.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição opcional do fluxo, fornecendo contexto adicional.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Identificador do usuário que criou o fluxo.
    /// </summary>
    public Guid SourceSpecId { get; set; }

    /// <summary>
    /// URL base para todas as chamadas de API dentro deste fluxo. Pode ser substituído por nós individuais.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Lista de nós que compõem o fluxo. Cada nó representa uma ação ou decisão no fluxo.
    /// </summary>
    public List<WorkflowNode> Nodes { get; set; } = new();

    /// <summary>
    /// Lista de conexões entre os nós, definindo a sequência e as condições do fluxo.
    /// </summary>
    public List<WorkflowConnection> Connections { get; set; } = new();

    /// <summary>
    /// Data e hora em que o fluxo foi criado, usada para rastreamento e auditoria.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data e hora da última execução do fluxo, se aplicável. Pode ser nulo se o fluxo nunca foi executado.
    /// </summary>
    public DateTime? LastRunAt { get; set; }
}