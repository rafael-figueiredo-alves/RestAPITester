using System.Text.Json;
using RestAPITester.Core.Execution;
using RestAPITester.Core.Models;

namespace RestAPITester.Cli;

public static class SuiteCommand
{
    public static async Task<int> RunAsync(CliOptions options, LoadedSpecs specs, CancellationToken cancellationToken)
    {
        var path = options.SuitePath!;

        if (!File.Exists(path))
        {
            ConsoleOutput.Failure($"Erro: arquivo de suíte não encontrado: {path}");
            return ExitCodes.UsageError;
        }

        TestSuite? suite;
        try
        {
            suite = JsonSerializer.Deserialize<TestSuite>(File.ReadAllText(path));
        }
        catch (JsonException ex)
        {
            ConsoleOutput.Failure($"Erro: o arquivo de suíte não é um JSON válido ({ex.Message}).");
            return ExitCodes.UsageError;
        }

        if (suite is null)
        {
            ConsoleOutput.Failure("Erro: não foi possível interpretar o arquivo de suíte.");
            return ExitCodes.UsageError;
        }

        Console.WriteLine($"Suíte \"{suite.Name}\" — {suite.TestCases.Count} teste(s)");
        Console.WriteLine();

        using var httpClient = new HttpClient();
        var runner = new TestSuiteRunner(new TestExecutor(httpClient));

        var result = await runner.RunAsync(
            suite,
            specs.Endpoints,
            proxyBaseUrl: options.ProxyUrl,
            baseUrlOverride: specs.ResolveBaseUrl(options.BaseUrlOverride, suite.BaseUrl),
            initialVariables: options.Variables,
            cancellationToken: cancellationToken);

        var step = 0;
        foreach (var testResult in result.Results)
        {
            var testCase = suite.TestCases.FirstOrDefault(tc => tc.Id == testResult.TestCaseId);
            var name = testCase?.Name ?? testResult.TestCaseId.ToString();
            var line = $"{++step,3}. [{(testResult.Success ? "OK   " : "FALHA")}] {name} — HTTP {testResult.StatusCode} — {testResult.DurationMs} ms";

            if (testResult.Success)
                ConsoleOutput.Success(line);
            else
                ConsoleOutput.Failure(line);

            if (!testResult.Success)
            {
                if (testResult.ErrorMessage is not null)
                    ConsoleOutput.Failure($"       Erro: {testResult.ErrorMessage}");

                foreach (var assertion in testResult.AssertionResults.Where(a => !a.Passed))
                    ConsoleOutput.Failure($"       Asserção falhou ({assertion.Type}): {assertion.Message}");
            }
        }

        var passed = result.Results.Count(r => r.Success);
        Console.WriteLine();

        if (result.AllPassed)
            ConsoleOutput.Success($"RESULTADO: tudo passou ({passed}/{result.Results.Count})");
        else
            ConsoleOutput.Failure($"RESULTADO: houve falhas ({passed}/{result.Results.Count} passaram)");

        if (options.ReportPath is not null)
        {
            ReportWriter.WriteSuite(options.ReportPath, suite, result);
            ConsoleOutput.Muted($"Relatório gravado em {options.ReportPath}");
        }

        return result.AllPassed ? ExitCodes.Success : ExitCodes.TestFailure;
    }
}