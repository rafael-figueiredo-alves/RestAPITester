namespace RestAPITester.Core.Models;

/// <summary>Pacote de backup com tudo que o RestAPITester salva localmente.</summary>
public class BackupPayload
{
    /// <summary>
    /// Lista de especificações salvas no RestAPITester. Cada especificação representa um conjunto de testes ou configurações relacionadas a uma API específica.
    /// </summary>
    public List<SavedSpec> Specs { get; set; } = new();

    /// <summary>
    /// Lista de suítes de teste salvas no RestAPITester. Cada suíte de teste contém um conjunto de casos de teste que podem ser executados juntos.
    /// </summary>
    public List<TestSuite> TestSuites { get; set; } = new();

    /// <summary>
    /// Lista de ambientes de API salvos no RestAPITester. Cada ambiente representa uma configuração específica para interagir com uma API, incluindo variáveis de ambiente e endpoints.
    /// </summary>
    public List<ApiEnvironment> Environments { get; set; } = new();
}