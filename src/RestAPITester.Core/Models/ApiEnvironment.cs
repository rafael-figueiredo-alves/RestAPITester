namespace RestAPITester.Core.Models;

/// <summary>
/// Entidade que representa um ambiente de API, contendo informações como nome, URL base e variáveis fixas.
/// </summary>
public class ApiEnvironment
{
    /// <summary>
    /// Identificador único do ambiente de API.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nome do ambiente de API (ex: "Desenvolvimento", "Produção").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL base do ambiente de API (ex: "https://api.example.com").
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Variáveis sempre disponíveis quando esse ambiente é usado (ex: uma API key fixa).
    /// </summary>
    public Dictionary<string, string> FixedVariables { get; set; } = new();
}