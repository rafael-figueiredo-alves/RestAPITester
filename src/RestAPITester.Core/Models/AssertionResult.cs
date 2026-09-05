namespace RestAPITester.Core.Models;

public class AssertionResult
{
    public AssertionType Type { get; set; }
    public bool Passed { get; set; }
    public string? Message { get; set; }
}