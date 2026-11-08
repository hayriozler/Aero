namespace Aero.TextMessageParser.Api.Parsers;

public sealed record ParseError(string ErrorCode, string? Description, bool IsElementError, string? Line = null)
{
    public static ParseError ForMessage(string errorCode, string? description) => new(errorCode, description, false);

    public static ParseError ForElement(string errorCode, string? description, string? line = null) => new(errorCode, description, true, line);
}

public sealed class ParseResult<T> where T : class
{
    private ParseResult(T? value, IReadOnlyList<ParseError> errors, IReadOnlyList<ParseError> warnings)
    {
        Value = value;
        Errors = errors;
        Warnings = warnings;
    }

    public T? Value { get; }

    public IReadOnlyList<ParseError> Errors { get; }

    public IReadOnlyList<ParseError> Warnings { get; }

    public bool IsSuccess => Value is not null;

    public static ParseResult<T> Success(T value, IReadOnlyList<ParseError>? warnings = null) => new(value, [], warnings ?? []);

    public static ParseResult<T> Failure(ParseError error) => new(null, [error], []);
}
