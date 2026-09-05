namespace RestAPITester.Core.Models;

/// <summary>
/// Representa uma especificação OpenAPI importada e salva localmente,
/// guardando o JSON bruto pra não precisar reimportar o arquivo toda vez.
/// </summary>
public class SavedSpec
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string RawJson { get; set; } = string.Empty;
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
}