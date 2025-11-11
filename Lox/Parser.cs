namespace Lox;

public static class Parser
{
    
}

/// <summary>
///   Base expression class
/// </summary>
public record Expr
{
    /// <summary>
    ///   Binary expression
    /// </summary>
    /// <param name="Left">Left expression</param>
    /// <param name="Op">Binary operator</param>
    /// <param name="Right">Right expression</param>
    public record Binary(Expr Left, Token Op, Expr Right) : Expr;

    /// <summary>
    ///   A grouping
    /// </summary>
    /// <param name="Expression">Expression being grouped</param>
    public record Grouping(Expr Expression) : Expr;

    /// <summary>
    ///   A literal value
    /// </summary>
    /// <param name="Value">The literal value</param>
    public record Literal(object Value) : Expr;

    /// <summary>
    ///   Unary expression
    /// </summary>
    /// <param name="Op">Unary operator</param>
    /// <param name="Right">Expression</param>
    public record Unary(Token Op, Expr Right) : Expr;
}
