using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Parsers;
using Aero.TextMessageParser.Api.Parsers.Messages;
using Aero.TextMessageParser.Api.Persistence;

namespace Aero.TextMessageParser.Api.Services;

public static class MessageParserExtensions
{
    private const int MaxDescriptionLength = 250;

    public static TResult? ProcessMessage<TResult>(
        this MessageParser<TResult> parser,
        TextMessage message,
        TextMessageParserDbContext dbContext)
        where TResult : class
    {
        var result = parser.Parse(message);
        foreach (var error in result.Errors)
        {
            if (error.IsElementError)
                dbContext.ElementErrors.Add(ElementError.Create(message.Id, error.ErrorCode, Describe(error)));
            else
                dbContext.MessageErrors.Add(TextMessageError.Create(message.Id, error.ErrorCode, error.Description));
        }
        foreach (var warning in result.Warnings)
            dbContext.ElementErrors.Add(ElementError.Create(message.Id, warning.ErrorCode, Describe(warning)));
        return result.Value;
    }

    private static string? Describe(ParseError error)
    {
        var description = string.IsNullOrEmpty(error.Line)
            ? error.Description
            : $"{error.Description} | {error.Line}";
        return description is { Length: > MaxDescriptionLength } ? description[..MaxDescriptionLength] : description;
    }
}
