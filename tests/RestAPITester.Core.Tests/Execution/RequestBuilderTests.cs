using RestAPITester.Core.Execution;
using RestAPITester.Core.Models;
using Xunit;

namespace RestAPITester.Core.Tests.Execution;

public class RequestBuilderTests
{
    private static EndpointInfo SimpleEndpoint => new()
    {
        OperationId = "GetItems",
        Path = "/items/{id}",
        Method = HttpMethodType.Get,
        Parameters =
        [
            new ParameterInfo { Name = "id", Location = ParameterLocation.Path, Required = true },
            new ParameterInfo { Name = "page", Location = ParameterLocation.Query }
        ]
    };

    [Fact]
    public void Build_ComParametrosDePathEQuery_MontaUrlCorreta()
    {
        var builder = new RequestBuilder();
        var testCase = new TestCase
        {
            EndpointOperationId = "GetItems",
            ParameterValues = new Dictionary<string, object?> { ["id"] = "42", ["page"] = "2" }
        };

        var request = builder.Build(SimpleEndpoint, testCase, "https://api.teste.com", new Dictionary<string, string>());

        Assert.Equal("https://api.teste.com/items/42?page=2", request.RequestUri!.ToString());
    }

    [Fact]
    public void Build_ComProxy_ReencaminhaParaUrlDoProxy()
    {
        var builder = new RequestBuilder();
        var testCase = new TestCase
        {
            EndpointOperationId = "GetItems",
            ParameterValues = new Dictionary<string, object?> { ["id"] = "42" }
        };

        var request = builder.Build(
            SimpleEndpoint, testCase, "https://api.teste.com", new Dictionary<string, string>(),
            proxyBaseUrl: "http://localhost:5220");

        Assert.StartsWith("http://localhost:5220/proxy?target=", request.RequestUri!.ToString());
    }

    [Fact]
    public void Build_ComFollowRedirectsFalse_AdicionaFlagNaUrlDoProxy()
    {
        var builder = new RequestBuilder();
        var testCase = new TestCase
        {
            EndpointOperationId = "GetItems",
            ParameterValues = new Dictionary<string, object?> { ["id"] = "42" },
            FollowRedirects = false
        };

        var request = builder.Build(
            SimpleEndpoint, testCase, "https://api.teste.com", new Dictionary<string, string>(),
            proxyBaseUrl: "http://localhost:5220");

        Assert.Contains("followRedirects=false", request.RequestUri!.ToString());
    }

    [Fact]
    public void Build_ComContentTypeFormUrlEncoded_MontaFormUrlEncodedContent()
    {
        var endpoint = new EndpointInfo
        {
            OperationId = "Login",
            Path = "/login",
            Method = HttpMethodType.Post,
            RequestBody = new RequestBodyInfo { ContentType = "application/x-www-form-urlencoded" }
        };

        var testCase = new TestCase
        {
            EndpointOperationId = "Login",
            RequestBodyJson = "username=joao\npassword=123"
        };

        var builder = new RequestBuilder();
        var request = builder.Build(endpoint, testCase, "https://api.teste.com", new Dictionary<string, string>());

        Assert.IsType<FormUrlEncodedContent>(request.Content);
    }

    [Fact]
    public void BuildCurlCommand_ComGetSimples_GeraComandoEsperado()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.teste.com/items/42");
        request.Headers.TryAddWithoutValidation("Authorization", "Bearer abc123");

        var curl = RequestBuilder.BuildCurlCommand(request, null);

        Assert.Contains("curl -X GET \"https://api.teste.com/items/42\"", curl);
        Assert.Contains("-H \"Authorization: Bearer abc123\"", curl);
    }

    [Fact]
    public void BuildCurlCommand_ComCorpo_IncluiFlagD()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.teste.com/items");
        request.Content = new StringContent("{\"nome\":\"teste\"}", System.Text.Encoding.UTF8, "application/json");

        var curl = RequestBuilder.BuildCurlCommand(request, "{\"nome\":\"teste\"}");

        Assert.Contains("-d '{\"nome\":\"teste\"}'", curl);
    }
}