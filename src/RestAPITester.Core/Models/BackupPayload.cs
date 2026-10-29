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

    /// <summary>Ausente em backups antigos — nesse caso desserializa como lista vazia.</summary>
    public List<WorkflowDefinition> Workflows { get; set; } = new();

    /// <summary>
    /// Gera ids novos para tudo (assim a restauração só adiciona, nunca sobrescreve) e
    /// reaponta as referências que dependem do id da spec: SourceSpecId das suítes e dos
    /// fluxos, e ApiCallSpecId de cada nó de API dentro dos fluxos.
    /// Referências para uma spec que não está no backup são mantidas como estão.
    /// </summary>
    public void AssignNewIds()
    {
        var specIdMap = new Dictionary<Guid, Guid>();

        foreach (var spec in Specs)
        {
            var newId = Guid.NewGuid();
            specIdMap[spec.Id] = newId;
            spec.Id = newId;
        }

        foreach (var suite in TestSuites)
        {
            suite.Id = Guid.NewGuid();
            suite.SourceSpecId = Remap(suite.SourceSpecId);
        }

        foreach (var environment in Environments)
            environment.Id = Guid.NewGuid();

        foreach (var workflow in Workflows)
        {
            workflow.Id = Guid.NewGuid();
            workflow.SourceSpecId = Remap(workflow.SourceSpecId);

            // Os ids dos nós e das conexões ficam como estão: só valem dentro do próprio fluxo.
            foreach (var node in workflow.Nodes)
                node.ApiCallSpecId = Remap(node.ApiCallSpecId);
        }

        Guid Remap(Guid oldId) => specIdMap.TryGetValue(oldId, out var newId) ? newId : oldId;
    }
}