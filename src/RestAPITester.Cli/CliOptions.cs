namespace RestAPITester.Cli;

public sealed class CliOptions
{
    public const string UsageText = """
        Uso:
          RestAPITester.Cli --spec <spec.json> [--spec <outra.json> ...] --suite <suite.json> [opções]
          RestAPITester.Cli --spec <spec.json> [--spec <outra.json> ...] --workflow <fluxo.json> [opções]

        Opções:
          --spec <arquivo>      Especificação OpenAPI (.json). Pode repetir: os endpoints de todas são usados.
          --suite <arquivo>     Suíte exportada pelo app (*.testsuite.json).
          --workflow <arquivo>  Fluxo exportado pelo app (*.workflow.json).
          --base-url <url>      Sobrepõe a URL base salva na suíte/fluxo.
          --proxy <url>         Passa as chamadas pelo proxy local (ex: http://localhost:5220).
          --var <nome=valor>    Variável inicial, usada como {{nome}}. Pode repetir (ideal pra segredos no CI).
          --report <arquivo>    Grava um resumo JSON (sem corpos de resposta, headers nem cURL).
          -h, --help            Mostra esta ajuda.

        Códigos de saída:
          0    tudo passou
          1    houve falhas na execução
          2    uso incorreto ou arquivo inválido
          130  cancelado (Ctrl+C)
        """;

    public List<string> SpecPaths { get; } = new();
    public string? SuitePath { get; private set; }
    public string? WorkflowPath { get; private set; }
    public string? BaseUrlOverride { get; private set; }
    public string? ProxyUrl { get; private set; }
    public string? ReportPath { get; private set; }
    public Dictionary<string, string> Variables { get; } = new();
    public bool ShowHelp { get; private set; }

    public static (CliOptions? Options, string? Error) Parse(string[] args)
    {
        var options = new CliOptions();

        try
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-h":
                    case "--help":
                        options.ShowHelp = true;
                        break;
                    case "--spec":
                        options.SpecPaths.Add(ReadValue(args, ref i));
                        break;
                    case "--suite":
                        options.SuitePath = ReadValue(args, ref i);
                        break;
                    case "--workflow":
                        options.WorkflowPath = ReadValue(args, ref i);
                        break;
                    case "--base-url":
                        options.BaseUrlOverride = ReadValue(args, ref i);
                        break;
                    case "--proxy":
                        options.ProxyUrl = ReadValue(args, ref i);
                        break;
                    case "--report":
                        options.ReportPath = ReadValue(args, ref i);
                        break;
                    case "--var":
                        AddVariable(options, ReadValue(args, ref i));
                        break;
                    default:
                        throw new ArgumentException($"Argumento desconhecido: {args[i]}");
                }
            }
        }
        catch (ArgumentException ex)
        {
            return (null, ex.Message);
        }

        if (options.ShowHelp)
            return (options, null);

        if (options.SuitePath is not null && options.WorkflowPath is not null)
            return (null, "Use --suite OU --workflow, não os dois ao mesmo tempo.");

        if (options.SuitePath is null && options.WorkflowPath is null)
            return (null, "Informe --suite <arquivo> ou --workflow <arquivo>.");

        if (options.SpecPaths.Count == 0)
            return (null, "Informe pelo menos uma --spec <arquivo>.");

        return (options, null);
    }

    private static string ReadValue(string[] args, ref int index)
    {
        var name = args[index];
        if (index + 1 >= args.Length)
            throw new ArgumentException($"O argumento {name} exige um valor.");

        return args[++index];
    }

    private static void AddVariable(CliOptions options, string text)
    {
        var separator = text.IndexOf('=');

        // Não ecoa o texto recebido: um segredo digitado sem o nome iria parar no log do CI.
        if (separator <= 0)
            throw new ArgumentException("--var espera o formato nome=valor.");

        options.Variables[text[..separator].Trim()] = text[(separator + 1)..];
    }
}