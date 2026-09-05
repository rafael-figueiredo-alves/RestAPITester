namespace RestAPITester.Core.Models;

public enum ParameterLocation
{
    Query,
    Path,
    Header,
    Cookie
}

/// <summary>
/// Representa um parâmetro de um endpoint (query, path, header, etc.)
/// </summary>
public class ParameterInfo
{
    public string Name { get; set; } = string.Empty;
    public ParameterLocation Location { get; set; }
    public bool Required { get; set; }
    public string SchemaType { get; set; } = "string"; // string, integer, boolean, array, object...
    public string? Format { get; set; } // ex: "date-time", "uuid", "int32"
    public string? Description { get; set; }
    public object? Example { get; set; }
}