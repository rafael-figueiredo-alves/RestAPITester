namespace RestAPITester.Core.Models;

public class ExecutionResult
{
    public Guid TestCaseId { get; set; }
    public int? StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public Dictionary<string, string> ResponseHeaders { get; set; } = new();
    public long DurationMs { get; set; }
    public bool Success { get; set; }
    public List<AssertionResult> AssertionResults { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Comando cURL equivalente à requisição real (sempre contra a URL de destino, não o proxy).</summary>
    public string? CurlCommand { get; set; }
}