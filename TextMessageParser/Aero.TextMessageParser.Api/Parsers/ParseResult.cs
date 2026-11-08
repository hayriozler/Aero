namespace Aero.TextMessageParser.Api.Parsers;

public sealed record ParseError(string ErrorCode, string? Description, bool IsElementError, string? Line = null)
{
    public static ParseError ForMessage(string errorCode, string? description) => new(errorCode, description, false);

    public static ParseError ForElement(string errorCode, string? description, string? line = null) => new(errorCode, description, true, line);
}

public sealed class ParseResult<T> where T : class
{
    private ParseResult(T? value, IReadOnlyList<ParseError> errors)
    {
        Value = value;
        Errors = errors;
    }

    public T? Value { get; }

    public IReadOnlyList<ParseError> Errors { get; }

    public bool IsSuccess => Value is not null;

    public static ParseResult<T> Success(T value) => new(value, []);

    public static ParseResult<T> Failure(ParseError error) => new(null, [error]);
}
