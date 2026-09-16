# RestAPITester

Ferramenta local (Blazor WebAssembly + MudBlazor) para importar uma especificação
OpenAPI, navegar pelos endpoints, gerar dados fictícios e testar chamadas reais —
com suporte a autenticação encadeada, suítes de teste reutilizáveis, ambientes
nomeados e um proxy local para contornar CORS.

## Arquitetura

- **RestAPITester.Core** — lógica pura, sem dependência de UI: parsing de OpenAPI
  (`Microsoft.OpenApi`), geração de dados fictícios (`Bogus`), motor de execução
  de requisições e avaliação de asserções (`JsonPath.Net`).
- **RestAPITester.Storage** — persistência local via IndexedDB (specs, suítes,
  ambientes e histórico de execuções) e export/import de arquivos JSON.
- **RestAPITester.Web** — a interface (Blazor WASM + MudBlazor): Workspace,
  Suítes de Teste e Dados Locais.
- **RestAPITester.Proxy** — um pequeno servidor ASP.NET Core local que repassa
  chamadas HTTP pra contornar CORS (a chamada real acontece servidor-a-servidor,
  fora do navegador).
- **RestAPITester.Core.Tests** — testes unitários (xUnit) do parser, gerador de
  dados, motor de execução e avaliador de asserções.

## Como rodar

```powershell
./run.ps1
```
Isso sobe o proxy em segundo plano e o app Web em primeiro plano — `Ctrl+C` encerra os dois.

Se preferir rodar manualmente em dois terminais:
```powershell
dotnet run --project src/RestAPITester.Proxy
dotnet run --project src/RestAPITester.Web
```

## Principais funcionalidades

- Importar OpenAPI de arquivo local ou URL remota (JSON, OpenAPI 3.0/3.1)
- Árvore de endpoints com busca
- Geração de dados fictícios (Bogus) a partir do schema
- Execução manual de endpoints com parâmetros, headers extras e captura de variáveis (corpo ou header)
- Suítes de teste com sequência ordenada, asserções (status, corpo, header, JSONPath, tempo de resposta) e propagação automática de variáveis (ex: token de login)
- Ambientes nomeados (Dev/Homolog/Produção) reutilizáveis
- Proxy local para contornar CORS, com controle de redirecionamento por chamada
- Persistência via IndexedDB + backup/restore completo em JSON
- Histórico das últimas 20 execuções por suíte
- Modo escuro, copiar resposta/cURL, atalho Ctrl+Enter

## Limitações conhecidas

- `multipart/form-data` suporta apenas campos de texto (`chave=valor` por linha) — sem upload de arquivo binário real.
- O vínculo suíte↔spec usa o `Id` da spec salva; se a spec de origem for excluída, a suíte perde a referência (mensagem de erro clara ao tentar rodar).