namespace Lochs;

public interface IErrorReporter
{
    void Error(int line, string message);
    void Error(Token token, string message);
}