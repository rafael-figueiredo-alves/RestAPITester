namespace RestAPITester.Core.Models;

/// <summary>
/// Um teste individual contra um endpoint específico: valores concretos de parâmetros,
/// corpo da requisição e o que se espera como resultado.
/// </summary>
public class TestCase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string EndpointOperationId { get; set; } = string.Empty; // referencia o EndpointInfo.OperationId
    public Dictionary<string, object?> ParameterValues { get; set; } = new(); // chave = ParameterInfo.Name
    public string? RequestBodyJson { get; set; } // corpo já preenchido (fictício ou manual)
    public int? ExpectedStatusCode { get; set; }
    public List<TestAssertion> Assertions { get; set; } = new();

    /// <summary>
    /// Nome de uma variável capturada da resposta (ex: token) para reuso em outro TestCase da suíte.
    /// </summary>
    public List<VariableCapture> CapturedVariables { get; set; } = new();
}