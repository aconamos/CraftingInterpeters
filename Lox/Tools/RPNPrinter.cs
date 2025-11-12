using System.Text;

namespace Lox.Tools;

public class RpnPrinter : Expr.IVisitor<string>
{
    /// <summary>
    ///   Prints an expression in Reverse Polish Notation
    /// </summary>
    /// <param name="expr">expression to print</param>
    /// <returns>string representation of expression</returns>
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    /// <inheritdoc />
    public string VisitBinaryExpr(Expr.Binary expr)
    {
        return $"{expr.Left.Accept(this)} {expr.Right.Accept(this)} {expr.Op.Lexeme}";
    }

    /// <inheritdoc />
    public string VisitGroupingExpr(Expr.Grouping expr)
    {
        return $"{expr.Expression.Accept(this)}";
    }

    /// <inheritdoc />
    public string VisitLiteralExpr(Expr.Literal expr)
    {
        if (expr.Value is null)
        {
            return "nil";
        }
        
        return expr.Value.ToString() ?? throw new NullReferenceException("Expression value ToString is null!");
    }

    /// <inheritdoc />
    public string VisitUnaryExpr(Expr.Unary expr)
    {
        return $"{expr.Right.Accept(this)} {expr.Op.Lexeme}";
    }
}