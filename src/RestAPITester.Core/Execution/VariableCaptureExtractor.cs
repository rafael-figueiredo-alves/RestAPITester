using System.Text.Json;
using RestAPITester.Core.Models;

namespace RestAPITester.Core.Execution;

/// <summary>
/// Extrai valores da resposta (via JSONPath simplificado) para injetar
/// como variáveis de sessão nas próximas chamadas da mesma TestSuite.
/// </summary>
public class VariableCaptureExtractor
{
    public Dictionary<string, string> Extract(TestCase testCase, ExecutionResult result)
    {
        var captured = new Dictionary<string, string>();

        foreach (var capture in testCase.CapturedVariables)
        {
            var value = JsonPathHelper.EvaluateFirst(result.ResponseBody, capture.JsonPath);
            if (value is not null)
                captured[capture.VariableName] = value;
        }

        return captured;
    }
}