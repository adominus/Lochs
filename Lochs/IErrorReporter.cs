namespace Lochs;

public interface IErrorReporter
{
    void Error(int line, string message);
}