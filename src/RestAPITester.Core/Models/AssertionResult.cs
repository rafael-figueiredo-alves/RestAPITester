namespace RestAPITester.Core.Models;

/// <summary>
/// Entidade que representa o resultado de uma asserção em um teste de API.
/// </summary>
public class AssertionResult
{
    /// <summary>
    /// Tipos de asserção que podem ser aplicados em um teste de API.
    /// </summary>
    public AssertionType Type { get; set; }

    /// <summary>
    /// Indica se a asserção foi bem-sucedida ou não.
    /// </summary>
    public bool Passed { get; set; }

    /// <summary>
    /// Mensagem detalhando o resultado da asserção, útil para depuração e análise de falhas.
    /// </summary>
    public string? Message { get; set; }
}