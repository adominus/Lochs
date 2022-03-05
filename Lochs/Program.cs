namespace Lochs;

class Program
{
    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: lochs: [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            new LochsRunner().RunFile(args[0]);
        }
        else
        {
            new LochsRunner().RunPrompt();
        }
    }
}