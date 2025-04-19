public class Logger : ILogger
{
    public void LogInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"INFO: {message}");
        Console.ResetColor();
    }

    public void LogWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"WARNING: {message}");
        Console.ResetColor();
    }

    public void LogError(string message, Exception ex = null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"ERROR: {message}");
        if (ex != null)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        }
        Console.ResetColor();
    }
}
