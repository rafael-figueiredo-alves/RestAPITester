namespace RestAPITester.Core.Models;

/// <summary>
/// Define como extrair um valor da resposta de um TestCase (ex: token JWT, ETag)
/// para injetar em requisições seguintes na mesma sessão/suíte.
/// </summary>
public class VariableCapture
{
    public string VariableName { get; set; } = string.Empty;

    /// <summary>Usado quando a captura vem do corpo da resposta (JSON). Ignorado se SourceHeaderName estiver definido.</summary>
    public string JsonPath { get; set; } = string.Empty;

    /// <summary>Se preenchido, captura o valor de um header da resposta (ex: "ETag") em vez do corpo.</summary>
    public string? SourceHeaderName { get; set; }
}