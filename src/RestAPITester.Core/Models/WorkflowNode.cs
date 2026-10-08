using RestAPITester.Core.EnumsAndConstants;

namespace RestAPITester.Core.Models;

/// <summary>
/// Um nó do fluxo. Quais campos importam depende do Type — por exemplo,
/// só nós ApiCall usam ApiCallTestCase.
/// </summary>
public class WorkflowNode
{
    /// <summary>
    /// Identificador único do nó, usado para referenciar este nó em outros nós (por exemplo, o próximo nó de um ApiCall).
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// O tipo do nó, que determina quais campos são relevantes.
    /// </summary>
    public WorkflowNodeType Type { get; set; }

    /// <summary>
    /// O título do nó, usado na UI para identificar o nó visualmente.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Posição no canvas — usada só pela UI, mas salva aqui pra preservar o layout.</summary>
    public double PositionX { get; set; }

    /// <summary>
    /// Posição no canvas — usada só pela UI, mas salva aqui pra preservar o layout.
    /// </summary>
    public double PositionY { get; set; }

    /// <summary>Usado quando Type == ApiCall — reaproveita o mesmo TestCase das suítes.</summary>
    public TestCase? ApiCallTestCase { get; set; }

    /// <summary>Usado quando Type == Delay.</summary>
    public int DelayMilliseconds { get; set; } = 1000;

    /// <summary>Usados quando Type == Condition (implementação vem num passo futuro).</summary>
    public string? ConditionJsonPath { get; set; }

    /// <summary>
    /// Usados quando Type == Condition (implementação vem num passo futuro).
    /// </summary>
    public string? ConditionExpectedValue { get; set; }

    /// <summary>De qual spec salva veio o endpoint escolhido neste nó (Type == ApiCall).</summary>
    public Guid ApiCallSpecId { get; set; }

    /// <summary>Quantas vezes o corpo do loop roda, no máximo (Type == Loop).</summary>
    public int LoopMaxIterations { get; set; } = 5;

    /// <summary>Usados quando Type == SetVariable.</summary>
    public string? SetVariableName { get; set; }

    /// <summary>
    /// Valor a ser atribuído à variável (Type == SetVariable). Pode ser um valor literal ou uma expressão que será avaliada no contexto do fluxo.
    /// </summary>
    public string? SetVariableValue { get; set; }

    /// <summary>Usado quando Type == Note.</summary>
    public string? NoteText { get; set; }    
}