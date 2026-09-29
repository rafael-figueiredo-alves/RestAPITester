namespace RestAPITester.Core.Models;

public enum WorkflowNodeType
{
    Start,
    ApiCall,
    Delay,
    Condition
}

/// <summary>
/// Um nó do fluxo. Quais campos importam depende do Type — por exemplo,
/// só nós ApiCall usam ApiCallTestCase.
/// </summary>
public class WorkflowNode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public WorkflowNodeType Type { get; set; }
    public string Title { get; set; } = string.Empty;

    /// <summary>Posição no canvas — usada só pela UI, mas salva aqui pra preservar o layout.</summary>
    public double PositionX { get; set; }
    public double PositionY { get; set; }

    /// <summary>Usado quando Type == ApiCall — reaproveita o mesmo TestCase das suítes.</summary>
    public TestCase? ApiCallTestCase { get; set; }

    /// <summary>Usado quando Type == Delay.</summary>
    public int DelayMilliseconds { get; set; } = 1000;

    /// <summary>Usados quando Type == Condition (implementação vem num passo futuro).</summary>
    public string? ConditionJsonPath { get; set; }
    public string? ConditionExpectedValue { get; set; }

    /// <summary>De qual spec salva veio o endpoint escolhido neste nó (Type == ApiCall).</summary>
    public Guid ApiCallSpecId { get; set; }
}