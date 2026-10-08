using RestAPITester.Core.EnumsAndConstants;

namespace RestAPITester.Core.Models;

/// <summary>
/// Representa um parâmetro de um endpoint (query, path, header, etc.)
/// </summary>
public class ParameterInfo
{
    /// <summary>
    /// Nome do parâmetro
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Localização do parâmetro (query, path, header, etc.)
    /// </summary>
    public ParameterLocation Location { get; set; }

    /// <summary>
    /// Indica se o parâmetro é obrigatório
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Tipo de dado do parâmetro (string, integer, boolean, array, object, etc.)
    /// </summary>
    public string SchemaType { get; set; } = "string"; // string, integer, boolean, array, object...

    /// <summary>
    /// Formato do parâmetro (date-time, uuid, int32, etc.)
    /// </summary>
    public string? Format { get; set; } // ex: "date-time", "uuid", "int32"

    /// <summary>
    /// Descrição do parâmetro
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Exemplo de valor do parâmetro
    /// </summary>
    public object? Example { get; set; }
}