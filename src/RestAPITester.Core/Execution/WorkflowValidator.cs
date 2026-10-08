using RestAPITester.Core.Models;

namespace RestAPITester.Core.Execution;

/// <summary>
/// Validates a workflow definition for potential issues, such as missing start nodes, unreachable nodes, and misconfigured nodes.
/// </summary>
public class WorkflowValidator
{
    public List<WorkflowValidationIssue> Validate(WorkflowDefinition workflow)
    {
        var issues = new List<WorkflowValidationIssue>();

        var startNodes = workflow.Nodes.Where(n => n.Type == WorkflowNodeType.Start).ToList();
        if (startNodes.Count == 0)
            issues.Add(Error("O fluxo não tem um nó de Início."));
        else if (startNodes.Count > 1)
            issues.Add(Error("O fluxo tem mais de um nó de Início — só o primeiro será usado."));

        foreach (var node in workflow.Nodes)
        {
            var outgoing = workflow.Connections.Where(c => c.SourceNodeId == node.Id).ToList();

            switch (node.Type)
            {
                case WorkflowNodeType.ApiCall:
                    if (node.ApiCallTestCase is null)
                        issues.Add(Error($"\"{node.Title}\": nenhum endpoint configurado.", node.Id));
                    if (outgoing.Count == 0)
                        issues.Add(Warning($"\"{node.Title}\": não tem conexão de saída — o fluxo vai parar aqui.", node.Id));
                    break;

                case WorkflowNodeType.Condition:
                    if (string.IsNullOrWhiteSpace(node.ConditionJsonPath))
                        issues.Add(Error($"\"{node.Title}\": condição sem JSONPath configurado.", node.Id));
                    if (!outgoing.Any(c => c.Label == "Sim"))
                        issues.Add(Warning($"\"{node.Title}\": falta a conexão do ramo \"Sim\".", node.Id));
                    if (!outgoing.Any(c => c.Label == "Não"))
                        issues.Add(Warning($"\"{node.Title}\": falta a conexão do ramo \"Não\".", node.Id));
                    break;

                case WorkflowNodeType.Loop:
                    if (node.LoopMaxIterations <= 0)
                        issues.Add(Error($"\"{node.Title}\": número de repetições precisa ser maior que zero.", node.Id));

                    var bodyConnection = outgoing.FirstOrDefault(c => c.Label == "Corpo");
                    if (bodyConnection is null)
                        issues.Add(Warning($"\"{node.Title}\": falta a conexão do \"Corpo\" do loop.", node.Id));
                    else if (!CanReach(bodyConnection.TargetNodeId, node.Id, workflow))
                        issues.Add(Warning($"\"{node.Title}\": o corpo do loop não volta para este nó — ele vai rodar só uma vez em vez de repetir.", node.Id));

                    if (!outgoing.Any(c => c.Label == "Fim"))
                        issues.Add(Warning($"\"{node.Title}\": falta a conexão de \"Fim\" do loop.", node.Id));
                    break;

                case WorkflowNodeType.SetVariable:
                    if (string.IsNullOrWhiteSpace(node.SetVariableName))
                        issues.Add(Error($"\"{node.Title}\": nome da variável não configurado.", node.Id));
                    if (outgoing.Count == 0)
                        issues.Add(Warning($"\"{node.Title}\": não tem conexão de saída — o fluxo vai parar aqui.", node.Id));
                    break;

                case WorkflowNodeType.Delay:
                case WorkflowNodeType.Start:
                    if (outgoing.Count == 0)
                        issues.Add(Warning($"\"{node.Title}\": não tem conexão de saída — o fluxo vai parar aqui.", node.Id));
                    break;
            }
        }

        if (startNodes.Count > 0)
        {
            var reachable = new HashSet<Guid> { startNodes[0].Id };
            var queue = new Queue<Guid>();
            queue.Enqueue(startNodes[0].Id);

            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                foreach (var conn in workflow.Connections.Where(c => c.SourceNodeId == currentId))
                {
                    if (reachable.Add(conn.TargetNodeId))
                        queue.Enqueue(conn.TargetNodeId);
                }
            }

            foreach (var node in workflow.Nodes.Where(n => !reachable.Contains(n.Id)))
                issues.Add(Warning($"\"{node.Title}\": não é alcançável a partir do Início.", node.Id));
        }

        return issues;
    }

    private static bool CanReach(Guid fromNodeId, Guid targetNodeId, WorkflowDefinition workflow)
    {
        var visited = new HashSet<Guid> { fromNodeId };
        var queue = new Queue<Guid>();
        queue.Enqueue(fromNodeId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            if (currentId == targetNodeId) return true;

            foreach (var conn in workflow.Connections.Where(c => c.SourceNodeId == currentId))
            {
                if (visited.Add(conn.TargetNodeId))
                    queue.Enqueue(conn.TargetNodeId);
            }
        }

        return false;
    }

    private static WorkflowValidationIssue Error(string message, Guid? nodeId = null) =>
        new() { Severity = ValidationSeverity.Error, Message = message, NodeId = nodeId };

    private static WorkflowValidationIssue Warning(string message, Guid? nodeId = null) =>
        new() { Severity = ValidationSeverity.Warning, Message = message, NodeId = nodeId };
}