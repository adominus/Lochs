namespace Lochs;

// This is so that we can take out the use of the static hadError

public class ErrorReporter : IErrorReporter
{
    public bool HadError = false;
    
    public void Error(int line, string message)
        => Report(line, string.Empty, message);

    public void Error(Token token, string message)
    {
        if (token.TokenType == TokenType.Eof)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }

    public void Error(RuntimeException ex)
    {
        Console.Error.WriteLine(ex.Message);
        Console.Error.WriteLine($"[Line {ex.Token.Line}]");

        HadError = true;
    }

    private void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[Line {line}] Error {where}: {message}");
        HadError = true;
    }
}