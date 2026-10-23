namespace RestAPITester.Cli;

public static class ConsoleOutput
{
    public static void Success(string text) => Write(text, ConsoleColor.Green);
    public static void Failure(string text) => Write(text, ConsoleColor.Red);
    public static void Warning(string text) => Write(text, ConsoleColor.Yellow);
    public static void Muted(string text) => Write(text, ConsoleColor.DarkGray);

    private static void Write(string text, ConsoleColor color)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = previous;
    }
}