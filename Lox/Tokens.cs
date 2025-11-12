namespace Lox;

/// <summary>
///   Represents a Token with information about its type, literal value, and line.
/// </summary>
/// <param name="type">Type of this token</param>
/// <param name="lexeme">Lexeme, or raw text of this token</param>
/// <param name="literal">The literal value of this token</param>
/// <param name="line">The line number of this token</param>
public readonly struct Token(TokenType type, string lexeme, object? literal, int line)
{
    /// <summary>
    ///   Gets the type of this token.
    /// </summary>
    public TokenType Type { get; init; } = type;

    /// <summary>
    ///   Gets the lexeme of this token.
    /// </summary>
    public string Lexeme { get; init; } = lexeme;

    /// <summary>
    ///   Gets the literal value of this token.
    /// </summary>
    public object? Literal { get; init; } = literal;

    /// <summary>
    ///   Gets the line this token is on.
    /// </summary>
    public int Line { get; init; } = line;

    /// <inheritdoc />
    public override string ToString() {
        
        return Type + " " + Lexeme + "{" + Literal + "}";
    }
}

/// <summary>
///   Represents a token's type.
/// </summary>
public enum TokenType {
    // Single-character tokens.
    LeftParen, RightParen, LeftBrace, RightBrace,
    Comma, Dot, Minus, Plus, Semicolon, Slash, Star,

    // One or two character tokens.
    Bang, BangEqual,
    Equal, EqualEqual,
    Greater, GreaterEqual,
    Less, LessEqual,

    // Literals.
    Identifier, String, Number,

    // Keywords.
    And, Class, Else, False, Fun, For, If, Nil, Or,
    Print, Return, Super, This, True, Var, While,

    Eof
}
