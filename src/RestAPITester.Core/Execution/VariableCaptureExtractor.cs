using RestAPITester.Core.Models;

namespace RestAPITester.Core.Execution;

/// <summary>
/// Extracts captured variables from the execution result based on the test case configuration.
/// </summary>
public class VariableCaptureExtractor
{
    public Dictionary<string, string> Extract(TestCase testCase, ExecutionResult result)
    {
        var captured = new Dictionary<string, string>();

        foreach (var capture in testCase.CapturedVariables)
        {
            string? value;

            if (!string.IsNullOrWhiteSpace(capture.SourceHeaderName))
            {
                result.ResponseHeaders.TryGetValue(capture.SourceHeaderName, out value);
            }
            else
            {
                value = JsonPathHelper.EvaluateFirst(result.ResponseBody, capture.JsonPath);
            }

            if (value is not null)
                captured[capture.VariableName] = value;
        }

        return captured;
    }
}