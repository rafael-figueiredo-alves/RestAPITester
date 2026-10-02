using RestAPITester.Core.Execution;
using RestAPITester.Core.Models;
using Xunit;

namespace RestAPITester.Core.Tests.Execution;

public class WorkflowValidatorTests
{
    [Fact]
    public void Validate_LoopSemVoltaParaSiMesmo_GeraAviso()
    {
        var start = new WorkflowNode { Type = WorkflowNodeType.Start, Title = "Início" };
        var loop = new WorkflowNode { Type = WorkflowNodeType.Loop, Title = "Repetir", LoopMaxIterations = 3 };
        var body = new WorkflowNode { Type = WorkflowNodeType.Delay, Title = "Corpo", DelayMilliseconds = 1 };
        var after = new WorkflowNode { Type = WorkflowNodeType.Delay, Title = "Fim do fluxo", DelayMilliseconds = 1 };

        var workflow = new WorkflowDefinition
        {
            Nodes = [start, loop, body, after],
            Connections =
            [
                new WorkflowConnection { SourceNodeId = start.Id, TargetNodeId = loop.Id },
                new WorkflowConnection { SourceNodeId = loop.Id, TargetNodeId = body.Id, Label = "Corpo" },
                new WorkflowConnection { SourceNodeId = loop.Id, TargetNodeId = after.Id, Label = "Fim" }
                // falta: body -> loop (a volta)
            ]
        };

        var issues = new WorkflowValidator().Validate(workflow);

        Assert.Contains(issues, i => i.Message.Contains("não volta para este nó"));
    }
}