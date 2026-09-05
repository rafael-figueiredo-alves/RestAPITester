namespace RestAPITester.Core.Models;

/// <summary>
/// Define como extrair um valor da resposta de um TestCase (ex: token JWT)
/// para injetar em requisições seguintes na mesma TestSuite.
/// </summary>
public class VariableCapture
{
    public string VariableName { get; set; } = string.Empty; // ex: "authToken"
    public string JsonPath { get; set; } = string.Empty;      // ex: "$.token"
}