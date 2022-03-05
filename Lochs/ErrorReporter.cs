namespace Lochs;

// This is so that we can take out the use of the static hadError

public class ErrorReporter : IErrorReporter
{
    public bool HadError = false;
    
    public void Error(int line, string message)
        => Report(line, string.Empty, message);

    private void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[Line {line}] Error {where}: {message}");
        HadError = true;
    }
}