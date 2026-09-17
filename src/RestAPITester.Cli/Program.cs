using System.Text.Json;
using RestAPITester.Core.Execution;
using RestAPITester.Core.Models;
using RestAPITester.Core.OpenApi;

var specPath = GetArg(args, "--spec");
var suitePath = GetArg(args, "--suite");
var baseUrlOverride = GetArg(args, "--base-url");
var proxyUrl = GetArg(args, "--proxy");

if (specPath is null || suitePath is null)
{
    Console.WriteLine("Uso: RestAPITester.Cli --spec <spec.json> --suite <suite.json> [--base-url <url>] [--proxy <url-do-proxy>]");
    return 1;
}

if (!File.Exists(specPath))
{
    Console.WriteLine($"Erro: arquivo de spec não encontrado: {specPath}");
    return 1;
}

if (!File.Exists(suitePath))
{
    Console.WriteLine($"Erro: arquivo de suíte não encontrado: {suitePath}");
    return 1;
}

var parser = new OpenApiParser();
var parseResult = parser.Parse(File.ReadAllText(specPath));

if (parseResult.HasErrors)
{
    Console.WriteLine("Erro ao ler a especificação OpenAPI:");
    foreach (var error in parseResult.Errors)
        Console.WriteLine($"  - {error}");
    return 1;
}

var suite = JsonSerializer.Deserialize<TestSuite>(File.ReadAllText(suitePath));
if (suite is null)
{
    Console.WriteLine("Erro: não foi possível interpretar o arquivo de suíte.");
    return 1;
}

using var httpClient = new HttpClient();
var runner = new TestSuiteRunner(new TestExecutor(httpClient));

Console.WriteLine($"Executando suíte \"{suite.Name}\" ({suite.TestCases.Count} teste(s))...\n");

var result = await runner.RunAsync(suite, parseResult.Endpoints, proxyUrl, baseUrlOverride);

foreach (var testResult in result.Results)
{
    var testCase = suite.TestCases.FirstOrDefault(tc => tc.Id == testResult.TestCaseId);
    var status = testResult.Success ? "OK   " : "FALHA";
    Console.WriteLine($"[{status}] {testCase?.Name ?? testResult.TestCaseId.ToString()} — status {testResult.StatusCode} — {testResult.DurationMs} ms");

    if (!testResult.Success)
    {
        if (testResult.ErrorMessage is not null)
            Console.WriteLine($"        Erro: {testResult.ErrorMessage}");

        foreach (var assertion in testResult.AssertionResults.Where(a => !a.Passed))
            Console.WriteLine($"        Asserção falhou ({assertion.Type}): {assertion.Message}");
    }
}

Console.WriteLine();
Console.WriteLine(result.AllPassed
    ? $"✅ Tudo passou ({result.Results.Count(r => r.Success)}/{result.Results.Count})"
    : $"❌ Houve falhas ({result.Results.Count(r => r.Success)}/{result.Results.Count} passaram)");

return result.AllPassed ? 0 : 1;

static string? GetArg(string[] args, string name)
{
    var index = Array.IndexOf(args, name);
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}