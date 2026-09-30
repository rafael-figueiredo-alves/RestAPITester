using System.Net;
using RestAPITester.Core.Execution;
using RestAPITester.Core.Models;

namespace RestAPITester.Core.Tests.Execution;

public class WorkflowRunnerTests
{
    [Fact]
    public async Task RunAsync_NotificaInicioDosNosNaOrdemDeExecucao()
    {
        var startNode = new WorkflowNode { Type = WorkflowNodeType.Start, Title = "Início" };
        var delayNode = new WorkflowNode { Type = WorkflowNodeType.Delay, Title = "Aguardar", DelayMilliseconds = 1 };
        var workflow = new WorkflowDefinition
        {
            Nodes = [startNode, delayNode],
            Connections =
            [
                new WorkflowConnection { SourceNodeId = startNode.Id, TargetNodeId = delayNode.Id }
            ]
        };
        var startedNodes = new List<Guid>();
        var runner = new WorkflowRunner(new TestExecutor(new HttpClient(new FakeHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.OK)))));

        var result = await runner.RunAsync(
            workflow,
            Array.Empty<EndpointInfo>(),
            onNodeStarting: async nodeId =>
            {
                await Task.Yield();
                startedNodes.Add(nodeId);
            });

        Assert.Equal(new[] { startNode.Id, delayNode.Id }, startedNodes);
        Assert.True(result.AllPassed);
    }

    [Fact]
    public async Task RunAsync_ComCondicaoVerdadeira_SegueRamoSim()
    {
        var startNode = new WorkflowNode { Type = WorkflowNodeType.Start, Title = "Início" };
        var apiNode = new WorkflowNode
        {
            Type = WorkflowNodeType.ApiCall,
            Title = "Chamada",
            ApiCallTestCase = new TestCase { EndpointOperationId = "GetStatus" }
        };
        var conditionNode = new WorkflowNode
        {
            Type = WorkflowNodeType.Condition,
            Title = "Status OK?",
            ConditionJsonPath = "$.status",
            ConditionExpectedValue = "ok"
        };
        var trueNode = new WorkflowNode { Type = WorkflowNodeType.Delay, Title = "Ramo Sim", DelayMilliseconds = 1 };
        var falseNode = new WorkflowNode { Type = WorkflowNodeType.Delay, Title = "Ramo Não", DelayMilliseconds = 1 };

        var workflow = new WorkflowDefinition
        {
            BaseUrl = "https://api.teste.com",
            Nodes = [startNode, apiNode, conditionNode, trueNode, falseNode],
            Connections =
            [
                new WorkflowConnection { SourceNodeId = startNode.Id, TargetNodeId = apiNode.Id },
                new WorkflowConnection { SourceNodeId = apiNode.Id, TargetNodeId = conditionNode.Id },
                new WorkflowConnection { SourceNodeId = conditionNode.Id, TargetNodeId = trueNode.Id, Label = "Sim" },
                new WorkflowConnection { SourceNodeId = conditionNode.Id, TargetNodeId = falseNode.Id, Label = "Não" }
            ]
        };

        var endpoint = new EndpointInfo { OperationId = "GetStatus", Path = "/status", Method = HttpMethodType.Get };
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"status\":\"ok\"}")
        });

        var runner = new WorkflowRunner(new TestExecutor(new HttpClient(handler)));
        var result = await runner.RunAsync(workflow, new[] { endpoint });

        Assert.True(result.AllPassed);
        Assert.True(result.NodeResults.First(r => r.NodeId == conditionNode.Id).ConditionResult);
        Assert.Contains(result.NodeResults, r => r.NodeId == trueNode.Id);
        Assert.DoesNotContain(result.NodeResults, r => r.NodeId == falseNode.Id);
    }    
}