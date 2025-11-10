using Lox.Errors;

namespace Lox;

/// <summary>
///   Contains code for the lexer.
/// </summary>
public static class Lexer
{
    /// <summary>
    ///   A map of strings to keyword tokens.
    /// </summary>
    private static readonly Dictionary<string, TokenType> Keywords = new();

    /// <summary>
    ///   Initializes the Keywords map.
    /// </summary>
    static Lexer() {
        Keywords.Add("and",    TokenType.And);
        Keywords.Add("class",  TokenType.Class);
        Keywords.Add("else",   TokenType.Else);
        Keywords.Add("false",  TokenType.False);
        Keywords.Add("for",    TokenType.For);
        Keywords.Add("fun",    TokenType.Fun);
        Keywords.Add("if",     TokenType.If);
        Keywords.Add("nil",    TokenType.Nil);
        Keywords.Add("or",     TokenType.Or);
        Keywords.Add("print",  TokenType.Print);
        Keywords.Add("return", TokenType.Return);
        Keywords.Add("super",  TokenType.Super);
        Keywords.Add("this",   TokenType.This);
        Keywords.Add("true",   TokenType.True);
        Keywords.Add("var",    TokenType.Var);
        Keywords.Add("while",  TokenType.While);
    }
    
    /// <summary>
    ///   Gets tokens from given code
    /// </summary>
    /// <param name="code">Code</param>
    /// <returns>
    ///   A tuple of Token and LexErrors, including all properly parsed tokens,
    ///   and a list of any errors.
    /// </returns>
    static (List<Token>, List<LexError>) GetTokens(string code)
    {
        List<Token> tokens = [];
        List<LexError> errors = [];

        int start = 0, current = 0, line = 1;

        bool IsAtEnd()
        {
            return current >= code.Length;
        }

        bool IsDigit(char maybeDigit)
        {
            return maybeDigit is >= '0' and <= '9';
        }

        bool IsAlpha(char maybeAlpha)
        {
            return maybeAlpha is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
        }

        bool IsAlphanumeric(char maybeAlphanumeric)
        {
            return IsAlpha(maybeAlphanumeric) || IsDigit(maybeAlphanumeric);
        }

        char Peek()
        {
            return IsAtEnd()
                ? '\0'
                : code[current];
        }

        char PeekNext()
        {
            if (current + 1 >= code.Length)
            {
                return '\0';
            }

            return code[current + 1];
        }

        bool MatchAhead(char expected)
        {
            if (IsAtEnd())
            {
                return false;
            }

            if (code[current] != expected)
            {
                return false;
            }

            current++;
            return true;
        }

        void AddNumber()
        {
            while (IsDigit(Peek()))
            {
                current++;
            }

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                current++;

                while (IsDigit(Peek()))
                {
                    current++;
                }
            }
            
            AddTokenLiteral(TokenType.Number, Double.Parse(
                code.Substring(start, current)
                ));
        }

        void AddString()
        {
            // Move ahead until we don't hit a quote.
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    line++;
                }

                current++;
            }
            
            if (IsAtEnd())
            {
                errors.Add(new LexError("Unterminated string.", line));
            }

            // Handles the closing '"'
            current++;

            string value = code.Substring(start + 1, current - 1);
            AddTokenLiteral(TokenType.String, value);
        }

        void AddBlockComment()
        {
            while (Peek() != '*' && PeekNext() != '/' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    line++;
                }
                
                current++;
            }
            
            if (IsAtEnd())
            {
                errors.Add(new LexError("Unterminated block comment.", line));
            }

            // Account for last */
            current += 2;
        }

        void AddIdentifier()
        {
            while (IsAlphanumeric(Peek()))
            {
                current++;
            }

            string lexeme = code.Substring(start, current);
            TokenType type = Keywords.GetValueOrDefault(lexeme, TokenType.Identifier);
            AddToken(type);
        }
        
        void AddTokenLiteral(TokenType type, object? literal)
        {
            string lexeme = code.Substring(start, current);
            tokens.Add(new Token(type, lexeme, literal, line));
        }

        void AddToken(TokenType type)
        {
            AddTokenLiteral(type, null);
        }
        
        void ScanToken()
        {
            char token = code[current++];

            switch (token)
            {
                case '(': AddToken(TokenType.LeftParen); break;
                case ')': AddToken(TokenType.RightParen); break;
                case '{': AddToken(TokenType.LeftBrace); break;
                case '}': AddToken(TokenType.RightBrace); break;
                case ',': AddToken(TokenType.Comma); break;
                case '.': AddToken(TokenType.Dot); break;
                case '-': AddToken(TokenType.Minus); break;
                case '+': AddToken(TokenType.Plus); break;
                case ';': AddToken(TokenType.Semicolon); break;
                case '*': AddToken(TokenType.Star); break; 
                
                case '!':
                    AddToken(MatchAhead('=') ? TokenType.BangEqual : TokenType.Bang);
                    break;
                case '=':
                    AddToken(MatchAhead('=') ? TokenType.EqualEqual : TokenType.Equal);
                    break;
                case '<':
                    AddToken(MatchAhead('=') ? TokenType.LessEqual : TokenType.Less);
                    break;
                case '>':
                    AddToken(MatchAhead('=') ? TokenType.GreaterEqual : TokenType.Greater);
                    break; 
                
                case '/':
                    if (MatchAhead('/')) {
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            current++;
                        }
                    } else {
                        AddToken(TokenType.Slash);
                    }
                    break;
                
                case ' ':
                case '\r':
                case '\t':
                    break;

                case '\n':
                    line++;
                    break;

                case '"':
                    AddString();
                    break;
                
                default:
                    if (IsDigit(token))
                    {
                        AddNumber();
                    }
                    else if (IsAlpha(token))
                    {
                        AddIdentifier();
                    }
                    else
                    {
                        errors.Add(new LexError($"Unexpected Character: {token}", line));
                    }
                    break;
            }
        }

        while (!IsAtEnd())
        {
            ScanToken();
            start = current;
        }

        return (tokens, errors);
    }
}