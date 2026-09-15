using System.Diagnostics;
using RestAPITester.Core.Models;

namespace RestAPITester.Core.Execution;

/// <summary>
/// Orquestra a execução de um TestCase: monta a requisição, dispara via HttpClient,
/// avalia asserções e extrai variáveis capturadas.
/// </summary>
public class TestExecutor
{
    private readonly HttpClient _httpClient;
    private readonly RequestBuilder _requestBuilder = new();
    private readonly AssertionEvaluator _assertionEvaluator = new();
    private readonly VariableCaptureExtractor _variableExtractor = new();

    public TestExecutor(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(ExecutionResult Result, Dictionary<string, string> CapturedVariables)> ExecuteAsync(
        EndpointInfo endpoint,
        TestCase testCase,
        string baseUrl,
        IReadOnlyDictionary<string, string> sessionVariables,
        string? proxyBaseUrl = null,
        CancellationToken cancellationToken = default)
    {
        var result = new ExecutionResult { TestCaseId = testCase.Id };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var curlPreview = _requestBuilder.Build(endpoint, testCase, baseUrl, sessionVariables);
            result.CurlCommand = RequestBuilder.BuildCurlCommand(curlPreview, testCase.RequestBodyJson);

            using var request = _requestBuilder.Build(endpoint, testCase, baseUrl, sessionVariables, proxyBaseUrl);
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            stopwatch.Stop();

            result.StatusCode = (int)response.StatusCode;
            result.ResponseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            result.DurationMs = stopwatch.ElapsedMilliseconds;

            foreach (var header in response.Headers)
                result.ResponseHeaders[header.Key] = string.Join(",", header.Value);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.DurationMs = stopwatch.ElapsedMilliseconds;
            result.ErrorMessage = ex.Message;
        }

        result.AssertionResults = _assertionEvaluator.Evaluate(testCase, result);
        result.Success = result.ErrorMessage is null && result.AssertionResults.All(a => a.Passed);

        var captured = _variableExtractor.Extract(testCase, result);

        return (result, captured);
    }
}