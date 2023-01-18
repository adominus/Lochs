namespace Lochs;

public class LochsRunner
{
    private ErrorReporter _errorReporter = new();
    private static IInterpreter _interpreter;

    public LochsRunner()
    {
        _interpreter = new Interpreter(_errorReporter);
    }

    public void RunFile(string file)
    {
        var source = File.ReadAllLines(file);
        Run(source);
        if (_errorReporter.HadError)
        {
            Environment.Exit(65);
        }
    }

    public void RunSource(string source)
    {
        Run(source);
        if (_errorReporter.HadError)
        {
            Environment.Exit(65);
        }
    }

    public void RunPrompt()
    {
        while (true)
        {
            Console.WriteLine("> ");
            var line = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
            {
                break;
            }

            Run(line);
            _errorReporter.HadError = false;
        }
    }

    private void Run(params string[] source)
    {
        var scanner = new Scanner(string.Join(Environment.NewLine, source), _errorReporter);
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens, _errorReporter);
        var expression = parser.Parse();

        if (expression != null)
        {
            _interpreter.Interpret(expression);
        }

        if (_errorReporter.HadError)
        {
            return;
        }
    }
}