using Lox;
using Lox.Tools;

namespace Cli;

/// <summary>
///   Command-line entrypoint for the cslox interpreter.
/// </summary>
static class Cli
{
    static void Main(string[] args)
    {
        Console.WriteLine(
        AstGenerator.GenerateAstTypes());
    }

    /// <summary>
    ///   Prints a usage message to the console.
    /// </summary>
    static void PrintUsageMessage()
    {
        Console.WriteLine("Usage: cslox [script]");
    }

    /// <summary>
    ///   Print an example expression using ASTPrinter
    /// </summary>
    static void PrintExampleExpression()
    {
        Expr expression = new Expr.Binary(
            new Expr.Unary(
                new Token(TokenType.Minus, "-", null, 1),
                new Expr.Literal(123)),
            new Token(TokenType.Star, "*", null, 1),
            new Expr.Grouping(
                new Expr.Literal(45.67)));

        Console.WriteLine(new AstPrinter().Print(expression));
    }

    /// <summary>
    ///   CLI entrypoint
    /// </summary>
    /// <param name="args">array of arguments</param>
    static void RunLoop(string[] args) 
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
    ///   Takes a given filename, reads it, and runs it.
    /// </summary>
    /// <param name="filename">Name of a file to run</param>
    static void RunFile(string filename)
    {
        string source = File.ReadAllText(filename);
        
        Run(source);
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
        var result = Lox.Lexer.GetTokens(code);

        foreach (var lineError in result.Item2)
        {
            Console.Error.WriteLine($"[line ${lineError.LineNumber}] Error: ${lineError.Message}");
        }

        Console.WriteLine(String.Join(", ", result.Item1));
        
    }
}