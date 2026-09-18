$ErrorActionPreference = "Stop"

Write-Host "1/3 Publicando RestAPITester.Web (arquivos estáticos)..." -ForegroundColor Cyan
dotnet publish src/RestAPITester.Web -c Release -o publish-web-temp

Write-Host "2/3 Copiando arquivos estáticos para RestAPITester/wwwroot..." -ForegroundColor Cyan
if (Test-Path src/RestAPITester/wwwroot) {
    Remove-Item src/RestAPITester/wwwroot -Recurse -Force
}
Copy-Item publish-web-temp/wwwroot src/RestAPITester/wwwroot -Recurse
Remove-Item publish-web-temp -Recurse -Force

Write-Host "3/3 Publicando o executável único RestAPITester.exe..." -ForegroundColor Cyan
dotnet publish src/RestAPITester -c Release -r win-x64 --self-contained true `
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o dist

Write-Host "Pronto! Executável em: dist\RestAPITester.exe" -ForegroundColor Green