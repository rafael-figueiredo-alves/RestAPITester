using IndexedDB.Blazor;
using Microsoft.JSInterop;
using RestAPITester.Core.Models;

namespace RestAPITester.Storage.IndexedDb;

public class RestApiTesterDb : IndexedDB.Blazor.IndexedDb
{
    public RestApiTesterDb(IJSRuntime jsRuntime, string name, int version)
        : base(jsRuntime, name, version) { }

    public IndexedSet<SavedSpec> Specs { get; set; } = null!;
    public IndexedSet<TestSuite> TestSuites { get; set; } = null!;
    public IndexedSet<ApiEnvironment> Environments { get; set; } = null!;
    public IndexedSet<TestSuiteExecutionResult> ExecutionHistory { get; set; } = null!;
}