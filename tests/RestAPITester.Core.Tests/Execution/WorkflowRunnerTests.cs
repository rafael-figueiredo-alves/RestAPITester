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

    [Fact]
    public async Task RunAsync_ComLoop_RepeteCorpoExatamenteNVezes()
    {
        var start = new WorkflowNode { Type = WorkflowNodeType.Start, Title = "Início" };
        var loop = new WorkflowNode { Type = WorkflowNodeType.Loop, Title = "Repetir", LoopMaxIterations = 3 };
        var body = new WorkflowNode { Type = WorkflowNodeType.Delay, Title = "Corpo", DelayMilliseconds = 1 };
        var after = new WorkflowNode { Type = WorkflowNodeType.Delay, Title = "Depois do loop", DelayMilliseconds = 1 };

        var workflow = new WorkflowDefinition
        {
            Nodes = [start, loop, body, after],
            Connections =
            [
                new WorkflowConnection { SourceNodeId = start.Id, TargetNodeId = loop.Id },
                new WorkflowConnection { SourceNodeId = loop.Id, TargetNodeId = body.Id, Label = "Corpo" },
                new WorkflowConnection { SourceNodeId = body.Id, TargetNodeId = loop.Id }, // volta pro loop
                new WorkflowConnection { SourceNodeId = loop.Id, TargetNodeId = after.Id, Label = "Fim" }
            ]
        };

        var runner = new WorkflowRunner(new TestExecutor(new HttpClient(new FakeHttpMessageHandler(
            _ => new HttpResponseMessage(System.Net.HttpStatusCode.OK)))));

        var result = await runner.RunAsync(workflow, Array.Empty<EndpointInfo>());

        Assert.True(result.AllPassed);
        Assert.Equal(3, result.NodeResults.Count(r => r.NodeId == body.Id));
        Assert.Equal(1, result.NodeResults.Count(r => r.NodeId == after.Id));
    }

    [Fact]
    public async Task RunAsync_ComConditionDentroDoLoop_SaiAntesDasNVezes()
    {
        var start = new WorkflowNode { Type = WorkflowNodeType.Start, Title = "Início" };
        var loop = new WorkflowNode { Type = WorkflowNodeType.Loop, Title = "Repetir", LoopMaxIterations = 5 };
        var apiCall = new WorkflowNode
        {
            Type = WorkflowNodeType.ApiCall,
            Title = "Chamada",
            ApiCallTestCase = new TestCase { EndpointOperationId = "GetStatus" }
        };
        var condition = new WorkflowNode
        {
            Type = WorkflowNodeType.Condition,
            Title = "Pronto?",
            ConditionJsonPath = "$.status",
            ConditionExpectedValue = "pronto"
        };
        var after = new WorkflowNode { Type = WorkflowNodeType.Delay, Title = "Depois do loop", DelayMilliseconds = 1 };

        var workflow = new WorkflowDefinition
        {
            BaseUrl = "https://api.teste.com",
            Nodes = [start, loop, apiCall, condition, after],
            Connections =
            [
                new WorkflowConnection { SourceNodeId = start.Id, TargetNodeId = loop.Id },
                new WorkflowConnection { SourceNodeId = loop.Id, TargetNodeId = apiCall.Id, Label = "Corpo" },
                new WorkflowConnection { SourceNodeId = apiCall.Id, TargetNodeId = condition.Id },
                new WorkflowConnection { SourceNodeId = condition.Id, TargetNodeId = after.Id, Label = "Sim" }, // "break"
                new WorkflowConnection { SourceNodeId = condition.Id, TargetNodeId = loop.Id, Label = "Não" }, // continua o loop
                new WorkflowConnection { SourceNodeId = loop.Id, TargetNodeId = after.Id, Label = "Fim" }
            ]
        };

        // Responde "pronto" sempre — então a condição já é satisfeita na 1ª volta.
        var endpoint = new EndpointInfo { OperationId = "GetStatus", Path = "/status", Method = HttpMethodType.Get };
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent("{\"status\":\"pronto\"}")
        });

        var runner = new WorkflowRunner(new TestExecutor(new HttpClient(handler)));
        var result = await runner.RunAsync(workflow, new[] { endpoint });

        Assert.True(result.AllPassed);
        Assert.Equal(1, result.NodeResults.Count(r => r.NodeId == apiCall.Id)); // só rodou 1 vez, não 5
        Assert.Equal(1, result.NodeResults.Count(r => r.NodeId == after.Id));
    }    
}