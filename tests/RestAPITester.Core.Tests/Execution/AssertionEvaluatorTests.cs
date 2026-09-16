using RestAPITester.Core.Execution;
using RestAPITester.Core.Models;
using Xunit;

namespace RestAPITester.Core.Tests.Execution;

public class AssertionEvaluatorTests
{
    private readonly AssertionEvaluator _evaluator = new();

    [Fact]
    public void Evaluate_BodyContains_PassaQuandoTextoEstaPresente()
    {
        var testCase = new TestCase
        {
            Assertions = [new TestAssertion { Type = AssertionType.BodyContains, ExpectedValue = "sucesso" }]
        };
        var result = new ExecutionResult { ResponseBody = "{\"status\":\"sucesso\"}" };

        Assert.True(_evaluator.Evaluate(testCase, result).Single().Passed);
    }

    [Fact]
    public void Evaluate_BodyContains_FalhaQuandoTextoAusente()
    {
        var testCase = new TestCase
        {
            Assertions = [new TestAssertion { Type = AssertionType.BodyContains, ExpectedValue = "erro" }]
        };
        var result = new ExecutionResult { ResponseBody = "{\"status\":\"sucesso\"}" };

        Assert.False(_evaluator.Evaluate(testCase, result).Single().Passed);
    }

    [Fact]
    public void Evaluate_HeaderEquals_ComparaValorDoHeader()
    {
        var testCase = new TestCase
        {
            Assertions = [new TestAssertion { Type = AssertionType.HeaderEquals, HeaderName = "ETag", ExpectedValue = "abc123" }]
        };
        var result = new ExecutionResult { ResponseHeaders = new Dictionary<string, string> { ["ETag"] = "abc123" } };

        Assert.True(_evaluator.Evaluate(testCase, result).Single().Passed);
    }

    [Fact]
    public void Evaluate_JsonPathEquals_ComArray_ComparaValorPorIndice()
    {
        var testCase = new TestCase
        {
            Assertions = [new TestAssertion { Type = AssertionType.JsonPathEquals, JsonPath = "$.items[0].id", ExpectedValue = "42" }]
        };
        var result = new ExecutionResult { ResponseBody = "{\"items\":[{\"id\":42}]}" };

        Assert.True(_evaluator.Evaluate(testCase, result).Single().Passed);
    }

    [Fact]
    public void Evaluate_ResponseTimeBelowMs_FalhaQuandoDemoraMais()
    {
        var testCase = new TestCase
        {
            Assertions = [new TestAssertion { Type = AssertionType.ResponseTimeBelowMs, ExpectedValue = "100" }]
        };
        var result = new ExecutionResult { DurationMs = 250 };

        Assert.False(_evaluator.Evaluate(testCase, result).Single().Passed);
    }
}