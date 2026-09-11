namespace RestAPITester.Core.Models;

/// <summary>
/// Uma URL base nomeada (ex: "Dev", "Homologação", "Produção"), reutilizável
/// em qualquer suíte ou execução avulsa no Workspace.
/// </summary>
public class ApiEnvironment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
}