using System.Text.Json;
using RestAPITester.Core.Models;

namespace RestAPITester.Core.Execution;

/// <summary>
/// Avalia as TestAssertion de um TestCase contra o resultado real da execução.
/// </summary>
public class AssertionEvaluator
{
    public List<AssertionResult> Evaluate(TestCase testCase, ExecutionResult result)
    {
        var results = new List<AssertionResult>();

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

        foreach (var assertion in testCase.Assertions)
        {
            results.Add(EvaluateSingle(assertion, result));
        }

        return results;
    }

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

    private static AssertionResult Check(TestAssertion assertion, bool passed) => new()
    {
        Type = assertion.Type,
        Passed = passed,
        Message = passed ? null : "Asserção falhou"
    };
}