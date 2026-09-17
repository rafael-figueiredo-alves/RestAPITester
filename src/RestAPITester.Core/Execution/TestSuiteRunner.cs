using RestAPITester.Core.Models;

namespace RestAPITester.Core.Execution;

public class TestSuiteRunner
{
    private readonly TestExecutor _executor;

    public TestSuiteRunner(TestExecutor executor)
    {
        _executor = executor;
    }

    public async Task<TestSuiteExecutionResult> RunAsync(
        TestSuite suite,
        IReadOnlyList<EndpointInfo> endpoints,
        string? proxyBaseUrl = null,
        string? baseUrlOverride = null,
        IReadOnlyDictionary<string, string>? initialVariables = null,
        CancellationToken cancellationToken = default)
    {
        var effectiveBaseUrl = string.IsNullOrWhiteSpace(baseUrlOverride) ? suite.BaseUrl : baseUrlOverride;

        var suiteResult = new TestSuiteExecutionResult
        {
            TestSuiteId = suite.Id,
            StartedAt = DateTime.UtcNow
        };

        var sessionVariables = initialVariables is not null
            ? new Dictionary<string, string>(initialVariables)
            : new Dictionary<string, string>();

        foreach (var testCase in suite.TestCases)
        {
            var endpoint = endpoints.FirstOrDefault(e => e.OperationId == testCase.EndpointOperationId);
            if (endpoint is null)
            {
                suiteResult.Results.Add(new ExecutionResult
                {
                    TestCaseId = testCase.Id,
                    Success = false,
                    ErrorMessage = $"Endpoint '{testCase.EndpointOperationId}' não encontrado na spec"
                });
                continue;
            }

            var (result, captured) = await _executor.ExecuteAsync(
                endpoint, testCase, effectiveBaseUrl, sessionVariables,
                proxyBaseUrl: proxyBaseUrl,
                cancellationToken: cancellationToken);

            suiteResult.Results.Add(result);

            foreach (var (key, value) in captured)
                sessionVariables[key] = value;
        }

        suiteResult.FinishedAt = DateTime.UtcNow;
        return suiteResult;
    }
}