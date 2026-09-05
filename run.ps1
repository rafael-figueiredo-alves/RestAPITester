$ErrorActionPreference = "Stop"

Write-Host "Iniciando proxy (RestAPITester.Proxy) em segundo plano..." -ForegroundColor Cyan
$proxy = Start-Process -FilePath "dotnet" -ArgumentList "run --project src/RestAPITester.Proxy" -PassThru -WindowStyle Minimized

Start-Sleep -Seconds 2

try {
    Write-Host "Iniciando RestAPITester.Web..." -ForegroundColor Cyan
    dotnet run --project src/RestAPITester.Web
}
finally {
    Write-Host "`nEncerrando proxy (PID $($proxy.Id))..." -ForegroundColor Yellow
    if ($proxy -and -not $proxy.HasExited) {
        Stop-Process -Id $proxy.Id -Force -ErrorAction SilentlyContinue
    }
}