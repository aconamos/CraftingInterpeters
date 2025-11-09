namespace Lox.Errors;

/// <summary>
///   Represents a generic Lox error.
/// </summary>
public class LoxError
{
    /// <summary>
    ///   Constructs a new LoxError with the given message.
    /// </summary>
    /// <param name="message">Error message</param>
    public LoxError(string message)
    {
        Message = message;
    }

    /// <summary>
    ///   Constructs a new LoxError with the name of the current type as the message.
    /// </summary>
    protected LoxError()
    {
        Message = this.GetType().Name;
    }
    
    /// <summary>
    ///   Gets the error message.
    /// </summary>
    public string Message { get; init; }
};

/// <summary>
///   An error type that represents an error with a line number associated.
/// </summary>
public abstract class LoxLineError : LoxError
{
    /// <summary>
    ///   Constructs a new LoxLineError with a message and a line number.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="line"></param>
    protected LoxLineError(string message, int line) : base(message)
    {
        LineNumber = line;
    }

    /// <summary>
    ///   Gets the line number for this error.
    /// </summary>
    public int LineNumber { get; init; }
}

/// <summary>
///   An error that represents a failure lexing.
/// </summary>
public class LoxLexError(string message, int line) : LoxLineError(message, line);

/// <summary>
///   An error that represents a failure parsing.
/// </summary>
public class LoxParseError(string message, int line) : LoxLineError(message, line);

/// <summary>
///   An error that represents a generic internal failure.
/// </summary>
public class LoxInternalError : LoxError;
