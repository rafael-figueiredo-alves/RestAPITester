using RestAPITester.Core.Models;

namespace RestAPITester.Core.Execution;

/// <summary>
/// Avalia as TestAssertion de um TestCase contra o resultado real da execução.
/// </summary>
public class AssertionEvaluator
{
    /// <summary>
    /// Método principal para avaliar as asserções de um TestCase contra o resultado da execução.
    /// </summary>
    /// <param name="testCase">O caso de teste com as asserções a serem avaliadas</param>
    /// <param name="result">O resultado da execução da requisição</param>
    /// <returns></returns>
    public List<AssertionResult> Evaluate(TestCase testCase, ExecutionResult result)
    {
        var results = new List<AssertionResult>();

        // Teste de StatusCode esperado, se definido no TestCase
        if (testCase.ExpectedStatusCode.HasValue)
        {
            var passed = result.StatusCode == testCase.ExpectedStatusCode.Value;
            results.Add(new AssertionResult
            {
                Type = AssertionType.StatusCode,
                Passed = passed,
                Message = passed
                    ? null
                    : $"Esperado {testCase.ExpectedStatusCode.Value}, recebido {result.StatusCode}"
            });
        }

        // Executa cada asserção definida no TestCase
        foreach (var assertion in testCase.Assertions)
        {
            results.Add(EvaluateSingle(assertion, result));
        }

        return results;
    }

    #region Métodos auxiliares privados de avalisação de asserções
    /// <summary>
    /// Avalia uma única asserção contra o resultado da execução.
    /// Adicionar aqui novos tipos de asserção conforme necessário.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private static AssertionResult EvaluateSingle(TestAssertion assertion, ExecutionResult result)
    {
        return assertion.Type switch
        {
            AssertionType.StatusCode => Check(
                assertion, result.StatusCode?.ToString() == assertion.ExpectedValue),

            AssertionType.BodyContains => Check(
                assertion, result.ResponseBody?.Contains(assertion.ExpectedValue ?? string.Empty) == true),

            AssertionType.HeaderEquals => Check(
                assertion,
                assertion.HeaderName is not null
                && result.ResponseHeaders.TryGetValue(assertion.HeaderName, out var headerValue)
                && headerValue == assertion.ExpectedValue),

            AssertionType.JsonPathEquals => Check(
                assertion, JsonPathHelper.EvaluateFirst(result.ResponseBody, assertion.JsonPath) == assertion.ExpectedValue),

            AssertionType.ResponseTimeBelowMs => Check(
                assertion,
                long.TryParse(assertion.ExpectedValue, out var maxMs) && result.DurationMs <= maxMs),

            _ => new AssertionResult { Type = assertion.Type, Passed = false, Message = "Tipo de asserção não suportado" }
        };
    }

    /// <summary>
    /// Cria um AssertionResult baseado no resultado da avaliação de uma asserção.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="passed"></param>
    /// <returns></returns>
    private static AssertionResult Check(TestAssertion assertion, bool passed) => new()
    {
        Type = assertion.Type,
        Passed = passed,
        Message = passed ? null : "Asserção falhou"
    };
    #endregion
}