namespace RestAPITester.Core.Models;

public class TestCase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string EndpointOperationId { get; set; } = string.Empty;
    public Dictionary<string, object?> ParameterValues { get; set; } = new();
    public string? RequestBodyJson { get; set; }
    public int? ExpectedStatusCode { get; set; }
    public List<TestAssertion> Assertions { get; set; } = new();
    public List<VariableCapture> CapturedVariables { get; set; } = new();

    /// <summary>
    /// Headers adicionados manualmente à requisição (ex: "If-Match": "{{etag}}"),
    /// úteis para APIs com conditional requests que não declaram isso como parâmetro na spec.
    /// </summary>
    public Dictionary<string, string> ExtraHeaders { get; set; } = new();

    public bool FollowRedirects { get; set; } = true;
}