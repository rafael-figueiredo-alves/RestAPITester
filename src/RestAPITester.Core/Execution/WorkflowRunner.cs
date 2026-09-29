using System.Diagnostics;
using RestAPITester.Core.Models;

namespace RestAPITester.Core.Execution;

/// <summary>
/// Executa um WorkflowDefinition percorrendo o grafo a partir do nó Start,
/// seguindo as conexões em sequência.
/// </summary>
public class WorkflowRunner
{
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

        var visited = new HashSet<Guid>();

        while (currentNode is not null)
        {
            if (!visited.Add(currentNode.Id))
            {
                result.NodeResults.Add(new WorkflowNodeExecutionResult
                {
                    NodeId = currentNode.Id,
                    NodeTitle = currentNode.Title,
                    NodeType = currentNode.Type,
                    Success = false,
                    ErrorMessage = "Loop detectado no fluxo — execução interrompida."
                });
                break;
            }

            if (onNodeStarting is not null)
                await onNodeStarting(currentNode.Id);

            var nodeResult = await ExecuteNodeAsync(currentNode, workflow, endpoints, sessionVariables, proxyBaseUrl, cancellationToken);
            result.NodeResults.Add(nodeResult);

            if (!nodeResult.Success)
                break; // por enquanto, uma falha interrompe o fluxo inteiro

            var nextConnection = workflow.Connections.FirstOrDefault(c => c.SourceNodeId == currentNode.Id);
            currentNode = nextConnection is null
                ? null
                : workflow.Nodes.FirstOrDefault(n => n.Id == nextConnection.TargetNodeId);
        }

        result.FinishedAt = DateTime.UtcNow;
        return result;
    }

    private async Task<WorkflowNodeExecutionResult> ExecuteNodeAsync(
        WorkflowNode node,
        WorkflowDefinition workflow,
        IReadOnlyList<EndpointInfo> endpoints,
        Dictionary<string, string> sessionVariables,
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
                stopwatch.Stop();
                return Fail(node, stopwatch, "Nós de Condição ainda não são executáveis — vem num passo futuro.");

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