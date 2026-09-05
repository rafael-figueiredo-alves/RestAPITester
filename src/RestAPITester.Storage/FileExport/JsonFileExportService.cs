using Microsoft.JSInterop;

namespace RestAPITester.Storage.FileExport;

public class JsonFileExportService
{
    private readonly IJSRuntime _js;

    public JsonFileExportService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task DownloadJsonAsync(string fileName, string json)
    {
        await _js.InvokeVoidAsync("downloadFileFromText", fileName, json, "application/json");
    }
}