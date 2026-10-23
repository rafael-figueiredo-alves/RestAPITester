using System.Text.Json;
using RestAPITester.Core.Execution;
using RestAPITester.Core.Models;
using RestAPITester.Core.EnumsAndConstants;

namespace RestAPITester.Cli;

public static class WorkflowCommand
{
    public static async Task<int> RunAsync(CliOptions options, LoadedSpecs specs, CancellationToken cancellationToken)
    {
        var path = options.WorkflowPath!;

        if (!File.Exists(path))
        {
            ConsoleOutput.Failure($"Erro: arquivo de fluxo não encontrado: {path}");
            return ExitCodes.UsageError;
        }

        WorkflowDefinition? workflow;
        try
        {
            workflow = JsonSerializer.Deserialize<WorkflowDefinition>(File.ReadAllText(path));
        }
        catch (JsonException ex)
        {
            ConsoleOutput.Failure($"Erro: o arquivo de fluxo não é um JSON válido ({ex.Message}).");
            return ExitCodes.UsageError;
        }

        if (workflow is null)
        {
            ConsoleOutput.Failure("Erro: não foi possível interpretar o arquivo de fluxo.");
            return ExitCodes.UsageError;
        }

        workflow.BaseUrl = specs.ResolveBaseUrl(options.BaseUrlOverride, workflow.BaseUrl);

        Console.WriteLine($"Fluxo \"{workflow.Name}\" — {workflow.Nodes.Count} nó(s)");

        var issues = new WorkflowValidator().Validate(workflow);
        foreach (var issue in issues)
        {
            if (issue.Severity == ValidationSeverity.Error)
                ConsoleOutput.Failure($"  [erro]  {issue.Message}");
            else
                ConsoleOutput.Warning($"  [aviso] {issue.Message}");
        }

        if (issues.Any(i => i.Severity.ToString() == "Error"))
        {
            ConsoleOutput.Failure("O fluxo tem erros de validação e não será executado.");
            return ExitCodes.UsageError;
        }

        Console.WriteLine();

        using var httpClient = new HttpClient();
        var runner = new WorkflowRunner(new TestExecutor(httpClient));

        var step = 0;
        var result = await runner.RunAsync(
            workflow,
            specs.Endpoints,
            proxyBaseUrl: options.ProxyUrl,
            cancellationToken: cancellationToken,
            onNodeCompleted: node =>
            {
                PrintNode(++step, node);
                return Task.CompletedTask;
            },
            initialVariables: options.Variables);

        var passed = result.NodeResults.Count(r => r.Success);
        var duration = (result.FinishedAt - result.StartedAt).TotalMilliseconds;
        Console.WriteLine();

        if (result.AllPassed)
            ConsoleOutput.Success($"RESULTADO: tudo passou ({passed}/{result.NodeResults.Count} etapas, {duration:0} ms)");
        else
            ConsoleOutput.Failure($"RESULTADO: houve falhas ({passed}/{result.NodeResults.Count} etapas ok, {duration:0} ms)");

        if (options.ReportPath is not null)
        {
            ReportWriter.WriteWorkflow(options.ReportPath, workflow, result);
            ConsoleOutput.Muted($"Relatório gravado em {options.ReportPath}");
        }

        return result.AllPassed ? ExitCodes.Success : ExitCodes.TestFailure;
    }

    private static void PrintNode(int step, WorkflowNodeExecutionResult node)
    {
        var details = new List<string> { $"{node.DurationMs} ms" };

        if (node.ApiCallResult is not null)
            details.Add($"HTTP {node.ApiCallResult.StatusCode}");

        var branch = ReportWriter.BranchLabel(node);
        if (branch is not null)
            details.Add($"ramo: {branch}");

        var line = $"{step,3}. [{(node.Success ? "OK   " : "FALHA")}] {node.NodeTitle} ({node.NodeType}) — {string.Join(" · ", details)}";

        if (node.Success)
            ConsoleOutput.Success(line);
        else
            ConsoleOutput.Failure(line);

        if (node.Success) return;

        if (!string.IsNullOrWhiteSpace(node.ErrorMessage))
            ConsoleOutput.Failure($"       Erro: {node.ErrorMessage}");

        if (node.ApiCallResult is not null)
        {
            foreach (var assertion in node.ApiCallResult.AssertionResults.Where(a => !a.Passed))
                ConsoleOutput.Failure($"       Asserção falhou ({assertion.Type}): {assertion.Message}");
        }
    }
}