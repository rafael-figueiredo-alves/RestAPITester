namespace RestAPITester.Core.Models;

public class ApiEnvironment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Variáveis sempre disponíveis quando esse ambiente é usado (ex: uma API key fixa).</summary>
    public Dictionary<string, string> FixedVariables { get; set; } = new();
}