using RestAPITester.Core.EnumsAndConstants;

namespace RestAPITester.Core.Models;

/// <summary>
/// Uma verificação simples aplicada sobre o resultado da execução de um TestCase.
/// </summary>
public class TestAssertion
{
    /// <summary>
    /// O tipo de verificação que será aplicada sobre o resultado da execução de um TestCase.
    /// </summary>
    public AssertionType Type { get; set; }

    /// <summary>
    /// O caminho JSON usado para localizar o valor a ser verificado no corpo da resposta, quando o tipo de verificação é JsonPathEquals.
    /// </summary>
    public string? JsonPath { get; set; }     // usado quando Type == JsonPathEquals

    /// <summary>
    /// O nome do cabeçalho da resposta a ser verificado, quando o tipo de verificação é HeaderEquals.
    /// </summary>
    public string? HeaderName { get; set; }   // usado quando Type == HeaderEquals

    /// <summary>
    /// O valor esperado para a verificação, que será comparado com o valor real obtido do resultado da execução do TestCase.
    /// </summary>
    public string? ExpectedValue { get; set; }
}