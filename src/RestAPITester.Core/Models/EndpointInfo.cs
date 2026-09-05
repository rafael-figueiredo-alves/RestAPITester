namespace RestAPITester.Core.Models;

/// <summary>
/// Representa um endpoint extraído do documento OpenAPI (um path + method).
/// </summary>
public class EndpointInfo
{
    public string OperationId { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public HttpMethodType Method { get; set; }
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<ParameterInfo> Parameters { get; set; } = new();
    public RequestBodyInfo? RequestBody { get; set; }
    public List<ResponseInfo> Responses { get; set; } = new();
    public bool RequiresAuth { get; set; }
}