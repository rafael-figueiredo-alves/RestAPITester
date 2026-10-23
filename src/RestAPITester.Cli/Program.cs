using System.Text;
using RestAPITester.Cli;

Console.OutputEncoding = Encoding.UTF8;

var (options, parseError) = CliOptions.Parse(args);

if (options is null)
{
    ConsoleOutput.Failure($"Erro: {parseError}");
    Console.WriteLine();
    Console.WriteLine(CliOptions.UsageText);
    return ExitCodes.UsageError;
}

if (options.ShowHelp)
{
    Console.WriteLine(CliOptions.UsageText);
    return ExitCodes.Success;
}

if (!SpecLoader.TryLoad(options.SpecPaths, out var specs, out var specError))
{
    ConsoleOutput.Failure($"Erro: {specError}");
    return ExitCodes.UsageError;
}

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    return options.WorkflowPath is not null
        ? await WorkflowCommand.RunAsync(options, specs, cts.Token)
        : await SuiteCommand.RunAsync(options, specs, cts.Token);
}
catch (OperationCanceledException)
{
    ConsoleOutput.Warning("Execução cancelada pelo usuário.");
    return ExitCodes.Cancelled;
}