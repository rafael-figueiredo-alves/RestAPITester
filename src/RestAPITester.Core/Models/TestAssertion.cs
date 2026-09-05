namespace RestAPITester.Core.Models;

public enum AssertionType
{
    StatusCode,
    BodyContains,
    JsonPathEquals,
    HeaderEquals,
    ResponseTimeBelowMs
}

/// <summary>
/// Uma verificação simples aplicada sobre o resultado da execução de um TestCase.
/// </summary>
public class TestAssertion
{
    public AssertionType Type { get; set; }
    public string? JsonPath { get; set; }     // usado quando Type == JsonPathEquals
    public string? HeaderName { get; set; }   // usado quando Type == HeaderEquals
    public string? ExpectedValue { get; set; }
}