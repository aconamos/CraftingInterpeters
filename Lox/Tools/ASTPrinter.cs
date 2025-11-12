using System.Text;

namespace Lox.Tools;

/// <summary>
///   Class that prints an expression
/// </summary>
public class AstPrinter : Expr.IVisitor<string>
{
    /// <summary>
    ///   Prints an expression
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
        return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
    }

    /// <inheritdoc />
    public string VisitGroupingExpr(Expr.Grouping expr)
    {
        return Parenthesize("group", expr.Expression);
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
        return Parenthesize(expr.Op.Lexeme, expr.Right);
    }

    /// <summary>
    ///   Wrap a bunch of expressions in parentheses
    /// </summary>
    /// <param name="name"></param>
    /// <param name="exprs"></param>
    /// <returns>Parenthesized string</returns>
    private string Parenthesize(string name, params Expr[] exprs)
    {
        StringBuilder ret = new StringBuilder();

        ret.Append("(").Append(name);
        foreach (var expr in exprs)
        {
            ret.Append(" ").Append(expr.Accept(this));
        }
        ret.Append(")");

        return ret.ToString();
    }
}