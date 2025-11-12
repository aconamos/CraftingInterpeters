using System.Text;

namespace Lox.Tools;

public static class AstGenerator
{
    public static readonly List<string> CurrentGrammar = [
        "Binary   : Expr Left, Token Op, Expr Right",
        "Grouping : Expr Expression",
        "Literal  : object? Value",
        "Unary    : Token Op, Expr Right"
    ];

    public static string GenerateAstTypes()
    {
        List<(string, List<string>)> records = [];

        foreach (var record in CurrentGrammar)
        {
            var sides = record.Split(":");

            var name = sides[0].Trim();
            var right = sides[1].Trim();

            var parameters = right.Split(", ");

            records.Add((name, parameters.ToList()));
        }
        
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("public abstract record Expr");
        builder.AppendLine("{");
        // builder.WriteRecord("Binary", ["Expr Left", "Token Op", "Expr Right"]);
        foreach (var record in records)
        {
            builder.WriteRecord(record.Item1, record.Item2);
            builder.AppendLine();
        }
        builder.WriteAccept();
        builder.WriteVisitor(records.Select(item => item.Item1).ToList());
        builder.AppendLine("}");

        return builder.ToString();
    }

}

file static class SbExtensions
{
    extension(StringBuilder builder) 
    {
        public void WriteRecord(string name, List<string> parameters)
        {
            builder.Tab().Append("public record ").Append(name).Append("(").AppendJoin(", ", parameters).AppendLine(") : Expr");
            builder.Tab().AppendLine("{");
            builder.WriteAcceptImpl(name);
            builder.Tab().AppendLine("}");
        }

        public void WriteVisitor(List<string> visitors)
        {
            builder.Tab().AppendLine("public interface IVisitor<out T>");
            builder.Tab().AppendLine("{");
            foreach (var visitor in visitors)
            {
                builder.Tab2().Append("T Visit").Append(visitor).Append("Expr(").Append(visitor).AppendLine(" expr);");
            }
            builder.Tab().AppendLine("}");
        }

        public void WriteAccept()
        {
            builder.Tab().AppendLine("public abstract T Accept<T>(IVisitor<T> visitor);").AppendLine();
        }

        private void WriteAcceptImpl(string name)
        {
            builder.Tab2().AppendLine("public override T Accept<T>(IVisitor<T> visitor)");
            builder.Tab2().AppendLine("{");
            builder.Tab2().Tab().Append("return visitor.Visit").Append(name).AppendLine("Expr(this);");
            builder.Tab2().AppendLine("}");
        }

        private StringBuilder Tab()
        {
            return builder.Append("    ");
        }

        private StringBuilder Tab2()
        {
            return builder.Append("        ");
        }
    }
}
