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
}