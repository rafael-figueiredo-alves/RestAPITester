using System.Net;
using RestAPITester.Core.Execution;
using RestAPITester.Core.Models;
using Xunit;

namespace RestAPITester.Core.Tests.Execution;

public class TestSuiteRunnerTests
{
    [Fact]
    public async Task RunAsync_DevePropagarTokenDoLoginParaProximaChamada()
    {
        var handler = new FakeHttpMessageHandler(request =>
        {
            if (request.RequestUri!.AbsolutePath == "/login")
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{ "token": "meu-token-123" }""")
                };
            }

            // Segunda chamada: confirma que o header Authorization chegou correto
            var authHeader = request.Headers.Authorization?.ToString();
            return authHeader == "Bearer meu-token-123"
                ? new HttpResponseMessage(HttpStatusCode.OK)
                : new HttpResponseMessage(HttpStatusCode.Unauthorized);
        });

        var runner = new TestSuiteRunner(new TestExecutor(new HttpClient(handler)));

        var endpoints = new List<EndpointInfo>
        {
            new() { OperationId = "Login", Path = "/login", Method = HttpMethodType.Post },
            new() { OperationId = "GetPerfil", Path = "/perfil", Method = HttpMethodType.Get, RequiresAuth = true }
        };

        var suite = new TestSuite
        {
            BaseUrl = "https://api.teste.com",
            TestCases =
            [
                new TestCase
                {
                    EndpointOperationId = "Login",
                    CapturedVariables = [new VariableCapture { VariableName = "authToken", JsonPath = "$.token" }]
                },
                new TestCase
                {
                    EndpointOperationId = "GetPerfil",
                    ExpectedStatusCode = 200
                }
            ]
        };

        var result = await runner.RunAsync(suite, endpoints);

        Assert.True(result.AllPassed);
    }
}