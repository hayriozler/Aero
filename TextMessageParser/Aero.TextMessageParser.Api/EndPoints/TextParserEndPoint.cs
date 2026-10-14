using Aero.Scanner.Shared.Requests;
using Aero.Shared.Infrastructure.Services;
using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Extensions;
using Aero.TextMessageParser.Api.Persistence;

namespace Aero.TextMessageParser.Api.EndPoints;

public static class TextParserEndPoint
{
    public static IEndpointRouteBuilder MapMessagingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/textmessages").WithTags("TextMessages");

        group.MapPost("/", async (MessageRequest request, LoggerService<TextMessage> logger, TextMessageParserDbContext db) =>
        {
            if (request.Message.Length < 9)
            {
                logger.LogError("Message is not correct: {Message}", request.Message);
                return Results.BadRequest("Message is not correct");
            }
            var (messageHeader, messageFooter) = request.Message.GetMessageIdentifier();

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