using Lox.Errors;

namespace Lox;

/// <summary>
///   Class to parse things
/// </summary>
public class Parser
{
    private readonly IList<Token> _tokens;
    private int _current = 0;

    private Parser(IList<Token> tokens) {
        _tokens = tokens;
    }

    /// <summary>
    ///   Parses a list of tokens into an expression.
    /// </summary>
    /// <param name="tokens">List of tokens</param>
    /// <returns>Expression</returns>
    public static Expr? ParseTokens(IList<Token> tokens)
    {
        Parser parser = new Parser(tokens);

        return parser.Parse();
    }

    /// <summary>
    ///   Parses the list of tokens this parser is constructed with.
    /// </summary>
    /// <returns>An expression if it was valid, or null.</returns>
    private Expr? Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseError error)
        {
            return null;
        }
    }

    /// <summary>
    ///   Consumes a token that doesn't match one of the given types.
    /// </summary>
    /// <param name="types">A list of types to match.</param>
    /// <returns>true if any match was made; otherwise, false</returns>
    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///   Checks to see if the token ahead has the same type as the given one.
    /// </summary>
    /// <param name="type">Given token</param>
    /// <returns>true if the next token has the same type as the given one; otherwise, false</returns>
    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;

        return Peek.Type == type;
    }
    
    /// <summary>
    ///   Advances this parser, incrementing the _current index and returning the Token.
    /// </summary>
    /// <returns>The token advanced past</returns>
    private Token Advance() {
        if (!IsAtEnd()) _current++;
        
        return Previous;
    }
    
    /// <summary>
    ///   Is this parser at the end of its tokens?
    /// </summary>
    /// <returns>true if so; otherwise, false</returns>
    private bool IsAtEnd() {
        return Peek.Type == TokenType.Eof;
    }

    /// <summary>
    ///   Peeks at the current token.
    /// </summary>
    /// <returns>Current token</returns>
    private Token Peek => _tokens[_current];

    /// <summary>
    ///   Gets the previous token
    /// </summary>
    /// <returns>Previous token</returns>
    private Token Previous => _tokens[_current - 1];

    /// <summary>
    ///   Reports an error
    /// </summary>
    /// <param name="lineNumber">Line number of the error</param>
    /// <param name="location">Location of the error</param>
    /// <param name="message">Message to display</param>
    private static void Report(int lineNumber, string location, string message)
    {
        Console.Error.WriteLine(
            $"Line {lineNumber}: {location}: {message}"
            );
    }
    
    /// <summary>
    ///   Prints an error to the console
    /// </summary>
    /// <param name="token">Token for the error</param>
    /// <param name="message">Message for the error</param>
    static void PrintError(Token token, String message) {
        if (token.Type == TokenType.Eof) {
            Report(token.Line, " at end", message);
        } else {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
        }
    }

    /// <summary>
    ///   Handles a parse error by printing
    /// </summary>
    /// <param name="token">Token for the error</param>
    /// <param name="message">Message for the error</param>
    /// <returns>A ParseError of the error</returns>
    private ParseError Error(Token token, String message)
    {
        PrintError(token, message);
        return new ParseError(message, token.Line);
    }

    /// <summary>
    ///   Consumes a token if it is of the given type
    /// </summary>
    /// <param name="type">Type to match</param>
    /// <param name="message">Message to show if consuming fails (it doesn't match)</param>
    /// <returns>The token consumed</returns>
    /// <exception cref="ParseError">If there was an error consuming</exception>
    private Token Consume(TokenType type, String message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek, message);
    }

    /// <summary>
    ///   Synchronizes the parser by advancing to the next semicolon
    /// </summary>
    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous.Type == TokenType.Semicolon) return;

            switch (Peek.Type)
            {
                case TokenType.Class:
                case TokenType.Fun:
                case TokenType.Var:
                case TokenType.For:
                case TokenType.If:
                case TokenType.While:
                case TokenType.Print:
                case TokenType.Return:
                    return;
            }

            Advance();
        }
    }
    
    //
    // ------- Rules -------
    //

    /// <summary>
    ///   Matches an expression
    /// </summary>
    /// <returns>The expression</returns>
    private Expr Expression()
    {
        return Ternary();
    }

    /// <summary>
    ///   Matches a ternary
    /// </summary>
    /// <returns>The expression</returns>
    private Expr Ternary()
    {
        Expr condition = Equality();

        if (Match(TokenType.Question))
        {
            Expr trueExpr = Equality();
            Consume(TokenType.Colon, "Expect ':' after ternary");
            Expr falseExpr = Equality();
            
            return new Expr.Ternary(condition, trueExpr, falseExpr);
        }

        return condition;
    }

    /// <summary>
    ///   Executes a common operation of matching a given rule, then matching given types, and returning a binary expression
    /// </summary>
    /// <param name="operandMethod">Method to match the rule</param>
    /// <param name="matchTypes">Types of tokens to match</param>
    /// <returns>The expression matched</returns>
    private Expr LeftAssociativeBinaryHelper(
        Func<Expr> operandMethod,
        params TokenType[] matchTypes
    )
    {
        Expr expr = operandMethod();

        while (Match(matchTypes))
        {
            Token op = Previous;
            Expr right = operandMethod();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    /// <summary>
    ///   Matches an equality rule
    /// </summary>
    /// <returns>The expression in the equality</returns>
    private Expr Equality() => LeftAssociativeBinaryHelper(
        Comparison,
        TokenType.BangEqual, TokenType.EqualEqual
    );

    /// <summary>
    ///   Matches a comparison rule
    /// </summary>
    /// <returns>The expression in the comparison</returns>
    private Expr Comparison() => LeftAssociativeBinaryHelper(
        Term,
        TokenType.Greater,
        TokenType.GreaterEqual,
        TokenType.Less,
        TokenType.LessEqual
    );

    /// <summary>
    ///   Matches a term rule
    /// </summary>
    /// <returns>The expression in the term</returns>
    private Expr Term() => LeftAssociativeBinaryHelper(
        Factor,
        TokenType.Minus,
        TokenType.Plus
    );

    /// <summary>
    ///   Matches a factor rule
    /// </summary>
    /// <returns>The expression in the factor</returns>
    private Expr Factor() => LeftAssociativeBinaryHelper(
        Unary,
        TokenType.Star,
        TokenType.Slash
    );

    /// <summary>
    ///   Matches a unary rule
    /// </summary>
    /// <returns>The expression in the unary</returns>
    private Expr Unary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            Token op = Previous;
            Expr right = Unary();
            return new Expr.Unary(op, right);
        }

        return Primary();
    }

    /// <summary>
    ///   Matches a primary rule
    /// </summary>
    /// <returns>The expression in the primary</returns>
    private Expr Primary()
    {
        if (Match(TokenType.False)) return new Expr.Literal(false);
        if (Match(TokenType.True)) return new Expr.Literal(true);
        if (Match(TokenType.Nil)) return new Expr.Literal(null);

        if (Match(TokenType.Number, TokenType.String))
        {
            return new Expr.Literal(Previous.Literal);
        }

        if (Match(TokenType.LeftParen))
        {
            Expr expr = Expression();
            Consume(TokenType.RightParen, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }

        throw Error(Peek, "Expect expression.");
    }
}

/// <summary>
///   Base expression class
/// </summary>
public abstract record Expr
{
    /// <summary>
    ///   Binary expression
    /// </summary>
    /// <param name="Left">Left expression</param>
    /// <param name="Op">Binary operator</param>
    /// <param name="Right">Right expression</param>
    public record Binary(Expr Left, Token Op, Expr Right) : Expr
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    /// <summary>
    ///   A grouping
    /// </summary>
    /// <param name="Expression">Expression being grouped</param>
    public record Grouping(Expr Expression) : Expr
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    /// <summary>
    ///   A literal value
    /// </summary>
    /// <param name="Value">The literal value</param>
    public record Literal(object? Value) : Expr
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    /// <summary>
    ///   Unary expression
    /// </summary>
    /// <param name="Op">Unary operator</param>
    /// <param name="Right">Expression</param>
    public record Unary(Token Op, Expr Right) : Expr
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }

    /// <summary>
    ///   Unary expression
    /// </summary>
    /// <param name="Conditional">The condition of the ternary</param>
    /// <param name="TrueExpr">The value if the condition is true</param>
    /// <param name="FalseExpr">The value if the condition is false</param>
    public record Ternary(Expr Conditional, Expr TrueExpr, Expr FalseExpr) : Expr
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitTernaryExpr(this);
        }
    }

    /// <summary>
    ///   Accept a visitor; that is, dispatch to the correct method on the visitor
    /// </summary>
    /// <param name="visitor">Visitor object</param>
    /// <typeparam name="T">Return type of visitor</typeparam>
    /// <returns>Whatever the visitor returns</returns>
    public abstract T Accept<T>(IVisitor<T> visitor);

    /// <summary>
    ///   Visitor interface for expressions
    /// </summary>
    /// <typeparam name="T">Return type </typeparam>
    public interface IVisitor<out T>
    {
        T VisitBinaryExpr(Binary expr);
        T VisitGroupingExpr(Grouping expr);
        T VisitLiteralExpr(Literal expr);
        T VisitUnaryExpr(Unary expr);
        T VisitTernaryExpr(Ternary expr);
    }
}
