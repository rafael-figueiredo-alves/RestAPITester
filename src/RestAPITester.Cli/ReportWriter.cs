using System.Text.Json;
using RestAPITester.Core.Models;
using RestAPITester.Core.EnumsAndConstants;

namespace RestAPITester.Cli;

/// <summary>
/// Grava um resumo da execução. De propósito NÃO inclui corpos de resposta, headers nem cURL:
/// eles podem conter tokens e outros segredos, e o arquivo costuma virar artefato de CI.
/// </summary>
public static class ReportWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static void WriteSuite(string path, TestSuite suite, TestSuiteExecutionResult result)
    {
        var report = new
        {
            kind = "suite",
            name = suite.Name,
            startedAt = result.StartedAt,
            finishedAt = result.FinishedAt,
            allPassed = result.AllPassed,
            steps = result.Results.Select(r => new
            {
                name = suite.TestCases.FirstOrDefault(tc => tc.Id == r.TestCaseId)?.Name ?? r.TestCaseId.ToString(),
                success = r.Success,
                httpStatus = r.StatusCode,
                durationMs = r.DurationMs,
                error = r.ErrorMessage,
                failedAssertions = r.AssertionResults.Where(a => !a.Passed).Select(a => $"{a.Type}: {a.Message}")
            })
        };

        Write(path, report);
    }

    public static void WriteWorkflow(string path, WorkflowDefinition workflow, WorkflowExecutionResult result)
    {
        var report = new
        {
            kind = "workflow",
            name = workflow.Name,
            startedAt = result.StartedAt,
            finishedAt = result.FinishedAt,
            allPassed = result.AllPassed,
            steps = result.NodeResults.Select(n => new
            {
                name = n.NodeTitle,
                type = n.NodeType.ToString(),
                success = n.Success,
                httpStatus = n.ApiCallResult?.StatusCode,
                branch = BranchLabel(n),
                durationMs = n.DurationMs,
                error = n.ErrorMessage,
                failedAssertions = n.ApiCallResult?.AssertionResults
                    .Where(a => !a.Passed)
                    .Select(a => $"{a.Type}: {a.Message}")
            })
        };

        Write(path, report);
    }

    public static string? BranchLabel(WorkflowNodeExecutionResult node)
    {
        if (!node.ConditionResult.HasValue) return null;

        return node.NodeType == WorkflowNodeType.Loop
            ? (node.ConditionResult.Value ? "Corpo" : "Fim")
            : (node.ConditionResult.Value ? "Sim" : "Não");
    }

    private static void Write(string path, object report)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(path, JsonSerializer.Serialize(report, JsonOptions));
    }
}