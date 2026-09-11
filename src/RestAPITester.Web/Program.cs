using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using RestAPITester.Web;
using IndexedDB.Blazor;
using RestAPITester.Core.Persistence;
using RestAPITester.Storage.IndexedDb;
using RestAPITester.Storage.FileExport;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
builder.Services.AddScoped<ISpecRepository, IndexedDbSpecRepository>();
builder.Services.AddScoped<ITestSuiteRepository, IndexedDbTestSuiteRepository>();
builder.Services.AddScoped<IEnvironmentRepository, IndexedDbEnvironmentRepository>();
builder.Services.AddScoped<IExecutionHistoryRepository, IndexedDbExecutionHistoryRepository>();
builder.Services.AddScoped<JsonFileExportService>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

await builder.Build().RunAsync();
