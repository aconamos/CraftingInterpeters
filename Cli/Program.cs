using Lox.Errors;

namespace Cli;

/// <summary>
///   Command-line entrypoint for the cslox interpreter.
/// </summary>
static class Cli
{
    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            PrintUsageMessage();
            Environment.Exit(64);
        } else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    /// <summary>
    ///   Prints a usage message to the console.
    /// </summary>
    static void PrintUsageMessage()
    {
        Console.WriteLine("Usage: cslox [script]");
    }

    /// <summary>
    ///   Takes a given filename, reads it, and runs it.
    /// </summary>
    /// <param name="filename">Name of a file to run</param>
    static void RunFile(string filename)
    {
        string source = File.ReadAllText(filename);
        
        Lox.Interpreter.Run(source);
    }

    /// <summary>
    ///   Initiates a REPL loop to run Lox code line-by-line.
    /// </summary>
    static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();
            
            if (line is null)
            {
                return;
            }
            
            Run(line);
        }
    }

    /// <summary>
    ///   Runs code in the Lox interpreter.
    /// </summary>
    static void Run(string code)
    {
        var result = Lox.Interpreter.Run(code);

        if (result is LineError lineError)
        {
            Console.Error.WriteLine($"[line ${lineError.LineNumber}] Error: ${lineError.Message}");
        }
        
        
    }
}