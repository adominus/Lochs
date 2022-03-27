namespace Lochs;

public class LochsRunner
{
    private ErrorReporter _errorReporter = new(); 
    
    public void RunFile(string file)
    {
        var source = File.ReadAllLines(file);
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

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}