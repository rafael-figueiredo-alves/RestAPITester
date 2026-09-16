namespace RestAPITester.Core.Models;

/// <summary>Pacote de backup com tudo que o RestAPITester salva localmente.</summary>
public class BackupPayload
{
    public List<SavedSpec> Specs { get; set; } = new();
    public List<TestSuite> TestSuites { get; set; } = new();
    public List<ApiEnvironment> Environments { get; set; } = new();
}