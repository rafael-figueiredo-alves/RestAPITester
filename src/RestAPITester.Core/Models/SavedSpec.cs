namespace RestAPITester.Core.Models;

/// <summary>
/// Representa uma especificação OpenAPI importada e salva localmente,
/// guardando o JSON bruto pra não precisar reimportar o arquivo toda vez.
/// </summary>
public class SavedSpec
{
    /// <summary>
    /// Identificador único da especificação salva.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Nome da especificação salva.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// JSON bruto da especificação salva.
    /// </summary>
    public string RawJson { get; set; } = string.Empty;
    
    /// <summary>
    /// Data e hora em que a especificação foi importada.
    /// </summary>
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
}