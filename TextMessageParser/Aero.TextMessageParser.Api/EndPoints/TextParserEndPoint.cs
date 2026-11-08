using Aero.Shared.Infrastructure.Services;
using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Extensions;
using Aero.TextMessageParser.Api.Persistence;
using Aero.TextMessageParser.Api.Requests;

namespace Aero.TextMessageParser.Api.EndPoints;

public static class TextParserEndPoint
{
    public static IEndpointRouteBuilder MapMessagingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/textmessages").WithTags("TextMessages");

        group.MapPost("/", async (MessageRequest request, LoggerService<TextMessage> logger, TextMessageParserDbContext db) =>
        {
            if (!request.Message.TryGetMessageIdentifier(out var messageHeader, out var messageFooter))
            {
                logger.LogWarning("Message header or footer could not be read");
                return Results.BadRequest("Message must have a 3 character header, at least one element line and a footer line");
            }
            var expectedFooter = Consts.END + messageHeader.Identifier;
            if (!string.Equals(messageFooter.EndIdentifier, expectedFooter, StringComparison.Ordinal))
            {
                logger.LogWarning("Message {Header} does not end with {ExpectedFooter}", messageHeader.Identifier, expectedFooter);
                return Results.BadRequest($"Message must end with {expectedFooter}");
            }

            var message = TextMessage.Create(request.Message, messageHeader, messageFooter);
            db.TextMessagesSet.Add(message);
            await db.SaveChangesAsync();
            return Results.Ok(true);
        })
        .WithName("TextMessages")
        .Produces<bool>()
        .ProducesProblem(404)
        .AllowAnonymous();
        return app;
    }
}