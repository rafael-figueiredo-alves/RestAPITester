using System.Diagnostics;
using RestAPITester.Core.Models;

namespace RestAPITester.Core.Execution;

public class WorkflowRunner
{
    private const int MaxTotalSteps = 500;

    private readonly TestExecutor _executor;

    public WorkflowRunner(TestExecutor executor)
    {
        _executor = executor;
    }

    public async Task<WorkflowExecutionResult> RunAsync(
        WorkflowDefinition workflow,
        IReadOnlyList<EndpointInfo> endpoints,
        string? proxyBaseUrl = null,
        CancellationToken cancellationToken = default,
        Func<Guid, Task>? onNodeStarting = null)
    {
        var result = new WorkflowExecutionResult
        {
            WorkflowId = workflow.Id,
            StartedAt = DateTime.UtcNow
        };

        var sessionVariables = new Dictionary<string, string>();
        var loopIterationCounts = new Dictionary<Guid, int>();
        var currentNode = workflow.Nodes.FirstOrDefault(n => n.Type == WorkflowNodeType.Start);

        if (currentNode is null)
        {
            result.NodeResults.Add(new WorkflowNodeExecutionResult
            {
                Success = false,
                ErrorMessage = "O fluxo não tem um nó de Início."
            });
            result.FinishedAt = DateTime.UtcNow;
            return result;
        }

        var totalSteps = 0;

        while (currentNode is not null)
        {
            totalSteps++;
            if (totalSteps > MaxTotalSteps)
            {
                result.NodeResults.Add(new WorkflowNodeExecutionResult
                {
                    NodeId = currentNode.Id,
                    NodeTitle = currentNode.Title,
                    NodeType = currentNode.Type,
                    Success = false,
                    ErrorMessage = $"Limite de {MaxTotalSteps} passos excedido — possível loop sem controle de saída."
                });
                break;
            }

            if (onNodeStarting is not null)
                await onNodeStarting(currentNode.Id);

            var lastApiResult = result.NodeResults.LastOrDefault(r => r.ApiCallResult is not null)?.ApiCallResult;

            var nodeResult = await ExecuteNodeAsync(
                currentNode, workflow, endpoints, sessionVariables, lastApiResult, loopIterationCounts, proxyBaseUrl, cancellationToken);
            result.NodeResults.Add(nodeResult);

            if (!nodeResult.Success)
                break;

            currentNode = GetNextNode(currentNode, nodeResult, workflow);
        }

        result.FinishedAt = DateTime.UtcNow;
        return result;
    }

    /// <summary>
    /// Decide qual nó vem a seguir. Condition e Loop usam o mesmo mecanismo de
    /// ramificação por Label — a diferença é só o par de rótulos usado.
    /// </summary>
    private static WorkflowNode? GetNextNode(WorkflowNode currentNode, WorkflowNodeExecutionResult currentResult, WorkflowDefinition workflow)
    {
        var outgoing = workflow.Connections.Where(c => c.SourceNodeId == currentNode.Id).ToList();

        if (currentNode.Type == WorkflowNodeType.Condition && currentResult.ConditionResult.HasValue)
        {
            var branchLabel = currentResult.ConditionResult.Value ? "Sim" : "Não";
            var branchConnection = outgoing.FirstOrDefault(c => string.Equals(c.Label, branchLabel, StringComparison.OrdinalIgnoreCase));
            return branchConnection is null ? null : workflow.Nodes.FirstOrDefault(n => n.Id == branchConnection.TargetNodeId);
        }

        if (currentNode.Type == WorkflowNodeType.Loop && currentResult.ConditionResult.HasValue)
        {
            var branchLabel = currentResult.ConditionResult.Value ? "Corpo" : "Fim";
            var branchConnection = outgoing.FirstOrDefault(c => string.Equals(c.Label, branchLabel, StringComparison.OrdinalIgnoreCase));
            return branchConnection is null ? null : workflow.Nodes.FirstOrDefault(n => n.Id == branchConnection.TargetNodeId);
        }

        var next = outgoing.FirstOrDefault(c => string.IsNullOrEmpty(c.Label));
        return next is null ? null : workflow.Nodes.FirstOrDefault(n => n.Id == next.TargetNodeId);
    }

    private async Task<WorkflowNodeExecutionResult> ExecuteNodeAsync(
        WorkflowNode node,
        WorkflowDefinition workflow,
        IReadOnlyList<EndpointInfo> endpoints,
        Dictionary<string, string> sessionVariables,
        ExecutionResult? lastApiResult,
        Dictionary<Guid, int> loopIterationCounts,
        string? proxyBaseUrl,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        switch (node.Type)
        {
            case WorkflowNodeType.Start:
                stopwatch.Stop();
                return Ok(node, stopwatch);

            case WorkflowNodeType.Delay:
                await Task.Delay(node.DelayMilliseconds, cancellationToken);
                stopwatch.Stop();
                return Ok(node, stopwatch);

            case WorkflowNodeType.Loop:
            {
                loopIterationCounts.TryGetValue(node.Id, out var currentCount);
                currentCount++;
                loopIterationCounts[node.Id] = currentCount;

                var shouldContinue = currentCount <= node.LoopMaxIterations;

                stopwatch.Stop();
                return new WorkflowNodeExecutionResult
                {
                    NodeId = node.Id,
                    NodeTitle = $"{node.Title} (iteração {currentCount}/{node.LoopMaxIterations})",
                    NodeType = node.Type,
                    Success = true,
                    ConditionResult = shouldContinue, // true = segue pro "Corpo", false = segue pro "Fim"
                    DurationMs = stopwatch.ElapsedMilliseconds
                };
            }

            case WorkflowNodeType.ApiCall:
            {
                if (node.ApiCallTestCase is null)
                {
                    stopwatch.Stop();
                    return Fail(node, stopwatch, "Nenhum endpoint configurado neste nó.");
                }

                var endpoint = endpoints.FirstOrDefault(e => e.OperationId == node.ApiCallTestCase.EndpointOperationId);
                if (endpoint is null)
                {
                    stopwatch.Stop();
                    return Fail(node, stopwatch, $"Endpoint '{node.ApiCallTestCase.EndpointOperationId}' não encontrado na spec.");
                }

                var (apiResult, captured) = await _executor.ExecuteAsync(
                    endpoint, node.ApiCallTestCase, workflow.BaseUrl, sessionVariables,
                    proxyBaseUrl: proxyBaseUrl,
                    cancellationToken: cancellationToken);

                foreach (var (key, value) in captured)
                    sessionVariables[key] = value;

                stopwatch.Stop();
                return new WorkflowNodeExecutionResult
                {
                    NodeId = node.Id,
                    NodeTitle = node.Title,
                    NodeType = node.Type,
                    Success = apiResult.Success,
                    ErrorMessage = apiResult.ErrorMessage,
                    ApiCallResult = apiResult,
                    DurationMs = stopwatch.ElapsedMilliseconds
                };
            }

            case WorkflowNodeType.Condition:
            {
                if (string.IsNullOrWhiteSpace(node.ConditionJsonPath))
                {
                    stopwatch.Stop();
                    return Fail(node, stopwatch, "Nó de condição sem JSONPath configurado.");
                }

                var actualValue = JsonPathHelper.EvaluateFirst(lastApiResult?.ResponseBody, node.ConditionJsonPath);
                var conditionMet = string.Equals(actualValue, node.ConditionExpectedValue, StringComparison.Ordinal);

                stopwatch.Stop();
                return new WorkflowNodeExecutionResult
                {
                    NodeId = node.Id,
                    NodeTitle = node.Title,
                    NodeType = node.Type,
                    Success = true,
                    ConditionResult = conditionMet,
                    DurationMs = stopwatch.ElapsedMilliseconds
                };
            }

            default:
                stopwatch.Stop();
                return Fail(node, stopwatch, "Tipo de nó desconhecido.");
        }
    }

    private static WorkflowNodeExecutionResult Ok(WorkflowNode node, Stopwatch stopwatch) => new()
    {
        NodeId = node.Id,
        NodeTitle = node.Title,
        NodeType = node.Type,
        Success = true,
        DurationMs = stopwatch.ElapsedMilliseconds
    };

    private static WorkflowNodeExecutionResult Fail(WorkflowNode node, Stopwatch stopwatch, string error) => new()
    {
        NodeId = node.Id,
        NodeTitle = node.Title,
        NodeType = node.Type,
        Success = false,
        ErrorMessage = error,
        DurationMs = stopwatch.ElapsedMilliseconds
    };
}