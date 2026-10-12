using BaggageService.Persistence;
using Contracts.Dtos;
using Contracts.Requests;
using IataText.Parser.Entities;
using IataText.Parser.Extensions;
using Infrastructure.Services;

namespace BaggageService.Endpoints;

public static class TextParserEndPoint
{
    public static IEndpointRouteBuilder MapMessagingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/messages").WithTags("Messages");

        group.MapPost("/", async (MessageRequest request, LoggerService<TextMessage> logger, AeroScanDataContext db) =>
        {
            if (request.Message.Length < 9)
            {
                logger.LogError("Message is not correct: {Message}", request.Message);
                return Results.BadRequest("Message is not correct");
            }
            var (MessageHeader, MessageFooter) = request.Message.GetMessageIdentifier();

            var message = TextMessage.Create(request.Message, MessageHeader, MessageFooter);
            db.TextMessagesSet.Add(message);
            await db.SaveChangesAsync();
            return Results.Ok(true);
        })
        .WithName("Messages")
        .Produces<bool>()
        .ProducesProblem(404)
        .AllowAnonymous();
        return app;
    }
}
