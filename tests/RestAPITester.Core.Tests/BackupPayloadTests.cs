using RestAPITester.Core.Models;
using RestAPITester.Core.EnumsAndConstants;
using Xunit;

namespace RestAPITester.Core.Tests;

public class BackupPayloadTests
{
    [Fact]
    public void AssignNewIds_RemapeiaReferenciasDeSpecEmSuitesEFluxos()
    {
        var spec = new SavedSpec { Name = "eTasks" };
        var oldSpecId = spec.Id;

        var suite = new TestSuite { Name = "Suíte", SourceSpecId = oldSpecId };

        var startNode = new WorkflowNode { Type = WorkflowNodeType.Start, Title = "Início" };
        var apiNode = new WorkflowNode { Type = WorkflowNodeType.ApiCall, Title = "API", ApiCallSpecId = oldSpecId };
        var workflow = new WorkflowDefinition
        {
            Name = "Fluxo",
            SourceSpecId = oldSpecId,
            Nodes = [startNode, apiNode],
            Connections = [new WorkflowConnection { SourceNodeId = startNode.Id, TargetNodeId = apiNode.Id }]
        };
        var oldWorkflowId = workflow.Id;

        var backup = new BackupPayload
        {
            Specs = [spec],
            TestSuites = [suite],
            Workflows = [workflow]
        };

        backup.AssignNewIds();

        Assert.NotEqual(oldSpecId, spec.Id);
        Assert.Equal(spec.Id, suite.SourceSpecId);
        Assert.Equal(spec.Id, workflow.SourceSpecId);
        Assert.Equal(spec.Id, apiNode.ApiCallSpecId);
        Assert.NotEqual(oldWorkflowId, workflow.Id);

        // As conexões internas do fluxo continuam apontando para os mesmos nós.
        Assert.Equal(startNode.Id, workflow.Connections[0].SourceNodeId);
        Assert.Equal(apiNode.Id, workflow.Connections[0].TargetNodeId);
    }

    [Fact]
    public void AssignNewIds_MantemReferenciaQuandoASpecNaoEstaNoBackup()
    {
        var missingSpecId = Guid.NewGuid();
        var node = new WorkflowNode { Type = WorkflowNodeType.ApiCall, ApiCallSpecId = missingSpecId };
        var backup = new BackupPayload
        {
            Workflows = [new WorkflowDefinition { Nodes = [node] }]
        };

        backup.AssignNewIds();

        Assert.Equal(missingSpecId, node.ApiCallSpecId);
    }

    [Fact]
    public void Deserializar_BackupAntigoSemFluxos_RetornaListaVazia()
    {
        const string oldBackupJson = """{ "Specs": [], "TestSuites": [], "Environments": [] }""";

        var backup = System.Text.Json.JsonSerializer.Deserialize<BackupPayload>(oldBackupJson);

        Assert.NotNull(backup);
        Assert.Empty(backup!.Workflows);
    }
}