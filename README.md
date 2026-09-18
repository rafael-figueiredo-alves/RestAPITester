
**Tecnologias principais:**

| Tecnologia | Uso |
|---|---|
| **Blazor WebAssembly** | Interface do app (roda no navegador, sem servidor) |
| **MudBlazor** | Componentes visuais (Material Design) |
| **Microsoft.OpenApi** | Parsing de especificações OpenAPI 3.0/3.1 |
| **Bogus** | Geração de dados fictícios a partir do schema |
| **JsonPath.Net** | Avaliação de JSONPath (asserções e captura de variáveis) |
| **IndexedDB.Blazor** | Persistência local no navegador |
| **ASP.NET Core (minimal API)** | Proxy local / host de distribuição final |
| **xUnit** | Testes unitários do `Core` |

## Como rodar no dia a dia (desenvolvimento)

```powershell
./run.ps1
```
Isso sobe o `RestAPITester.Proxy` em segundo plano e o `RestAPITester.Web` em
primeiro plano (com hot reload) — `Ctrl+C` encerra os dois.

Se preferir rodar manualmente, em dois terminais:
```powershell
dotnet run --project src/RestAPITester.Proxy
dotnet run --project src/RestAPITester.Web
```

Rodar os testes:
```powershell
dotnet test
```

## Como publicar (gerar o executável final)

```powershell
./publish.ps1
```

Por trás dos panos, o script:
1. Publica o `RestAPITester.Web` (gera os arquivos estáticos do Blazor).
2. Copia esses arquivos pra dentro de `src/RestAPITester/wwwroot`.
3. Publica `src/RestAPITester` como um `.exe` único e autocontido (não precisa
   de .NET instalado na máquina de destino).

O resultado fica em `dist\RestAPITester.exe`.

## Como usar a versão final (distribuição)

Basta copiar `dist\RestAPITester.exe` pra qualquer máquina Windows e dar
**duplo-clique**:
- Uma janela de console mínima abre (só status).
- O navegador padrão abre sozinho em `http://localhost:5218`.
- Tudo funciona: import, execução, proxy embutido, suítes, persistência.
- Pra encerrar, é só fechar a janela do console (ou `Ctrl+C`).

Não precisa de PowerShell, SDK do .NET, nem configuração nenhuma — é o modo
pensado pra outros devs usarem sem fricção.

## Rodando suítes sem abrir o app (CLI / CI)

```powershell
dotnet run --project src/RestAPITester.Cli -- --spec minha-spec.json --suite minha-suite.testsuite.json --proxy http://localhost:5220
```
Retorna código de saída `0` (tudo passou) ou `1` (houve falha) — útil pra
scripts e pipelines.

## Principais funcionalidades

- Importar OpenAPI de arquivo local ou URL remota (JSON, OpenAPI 3.0/3.1)
- Árvore de endpoints com busca
- Geração de dados fictícios (Bogus) a partir do schema
- Execução manual de endpoints — parâmetros, headers extras, form-data/x-www-form-urlencoded
- Captura de variáveis (corpo ou header) e reuso via `{{variavel}}`
- Suítes de teste com sequência ordenada, asserções (status, corpo, header, JSONPath, tempo de resposta) e propagação automática de variáveis (ex: token de login)
- Ambientes nomeados (Dev/Homolog/Produção) com variáveis fixas
- Proxy local para contornar CORS, com controle de redirecionamento por chamada
- Persistência via IndexedDB + backup/restore completo em JSON
- Histórico das últimas 20 execuções por suíte, com exportação de relatório
- Validação de campos obrigatórios e visualização do schema esperado da resposta
- Modo escuro, copiar resposta/cURL, atalho `Ctrl+Enter`, continuar de onde parou

## Limitações conhecidas

- `multipart/form-data` suporta apenas campos de texto (`chave=valor` por linha) — sem upload de arquivo binário real.
- O vínculo suíte↔spec usa o `Id` da spec salva; se a spec de origem for excluída, a suíte perde a referência (mensagem de erro clara ao tentar rodar).
- O executável publicado (`dist/RestAPITester.exe`) atualmente é gerado para `win-x64`.