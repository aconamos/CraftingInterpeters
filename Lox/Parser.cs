namespace Lox;

public static class Parser
{
    
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
    }
}
