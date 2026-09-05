using System.Net;
using RestAPITester.Core.Execution;
using RestAPITester.Core.Models;
using Xunit;

namespace RestAPITester.Core.Tests.Execution;

public class TestExecutorTests
{
    private static EndpointInfo SimpleGetEndpoint => new()
    {
        OperationId = "GetItems",
        Path = "/items",
        Method = HttpMethodType.Get
    };

    [Fact]
    public async Task ExecuteAsync_ComStatusEsperado_DevePassar()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"ok\": true}")
            });

        var executor = new TestExecutor(new HttpClient(handler));

        var testCase = new TestCase
        {
            EndpointOperationId = "GetItems",
            ExpectedStatusCode = 200
        };

        var (result, _) = await executor.ExecuteAsync(
            SimpleGetEndpoint, testCase, "https://api.teste.com", new Dictionary<string, string>());

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_ComStatusDiferenteDoEsperado_DeveFalhar()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.NotFound));

        var executor = new TestExecutor(new HttpClient(handler));

        var testCase = new TestCase
        {
            EndpointOperationId = "GetItems",
            ExpectedStatusCode = 200
        };

        var (result, _) = await executor.ExecuteAsync(
            SimpleGetEndpoint, testCase, "https://api.teste.com", new Dictionary<string, string>());

        Assert.False(result.Success);
        Assert.Contains(result.AssertionResults, a => !a.Passed);
    }

    [Fact]
    public async Task ExecuteAsync_ComRespostaEmArray_DeveCapturarVariavelPorIndice()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                { "items": [ { "id": 42, "nome": "Primeiro" }, { "id": 43, "nome": "Segundo" } ] }
                """)
            });

        var executor = new TestExecutor(new HttpClient(handler));

        var testCase = new TestCase
        {
            EndpointOperationId = "GetItems",
            CapturedVariables =
            [
                new VariableCapture { VariableName = "primeiroId", JsonPath = "$.items[0].id" }
            ]
        };

        var (_, captured) = await executor.ExecuteAsync(
            SimpleGetEndpoint, testCase, "https://api.teste.com", new Dictionary<string, string>());

        Assert.Equal("42", captured["primeiroId"]);
    }

    [Fact]
    public async Task ExecuteAsync_ComVariavelDeSessao_DeveInjetarNoHeaderAuthorization()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var executor = new TestExecutor(new HttpClient(handler));

        var endpoint = SimpleGetEndpoint;
        endpoint.RequiresAuth = true;

        var testCase = new TestCase { EndpointOperationId = "GetItems" };
        var sessionVariables = new Dictionary<string, string> { ["authToken"] = "abc123" };

        await executor.ExecuteAsync(endpoint, testCase, "https://api.teste.com", sessionVariables);

        Assert.Equal("Bearer abc123", handler.LastRequest?.Headers.Authorization?.ToString());
    }
}