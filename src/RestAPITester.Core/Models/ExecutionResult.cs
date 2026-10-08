namespace RestAPITester.Core.Models;

/// <summary>
/// Entidade que representa o resultado da execução de um caso de teste.
/// </summary>
public class ExecutionResult
{
    /// <summary>
    /// Identificador único do caso de teste que gerou este resultado.
    /// </summary>
    public Guid TestCaseId { get; set; }

    /// <summary>
    /// Código de status HTTP retornado pela requisição. Pode ser nulo se a requisição não foi concluída com sucesso.
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Corpo da resposta retornada pela requisição. Pode ser nulo se a requisição não foi concluída com sucesso.
    /// </summary>
    public string? ResponseBody { get; set; }

    /// <summary>
    /// Cabeçalhos da resposta retornada pela requisição. Pode ser nulo se a requisição não foi concluída com sucesso.
    /// </summary>
    public Dictionary<string, string> ResponseHeaders { get; set; } = new();

    /// <summary>
    /// Duração da execução da requisição em milissegundos.
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Indica se a execução do caso de teste foi bem-sucedida ou não.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Lista de resultados das asserções realizadas durante a execução do caso de teste.
    /// </summary>
    public List<AssertionResult> AssertionResults { get; set; } = new();

    /// <summary>
    /// Mensagem de erro detalhada em caso de falha na execução do caso de teste. Pode ser nula se a execução foi bem-sucedida.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Data e hora em que a execução do caso de teste foi realizada.
    /// </summary>
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Comando cURL equivalente à requisição real (sempre contra a URL de destino, não o proxy).</summary>
    public string? CurlCommand { get; set; }
}