using Aero.Shared.Infrastructure.Services;
using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Extensions;
using Aero.TextMessageParser.Api.Persistence;
using Aero.TextMessageParser.Api.Requests;
using Aero.TextMessageParser.Api.Responses;

namespace Aero.TextMessageParser.Api.Endpoints;

public static class TextMessageEndpoints
{
    public static IEndpointRouteBuilder MapMessagingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/textmessages").WithTags("TextMessages");

        group.MapPost("/", async (
            MessageRequest request,
            LoggerService<TextMessage> logger,
            TextMessageParserDbContext db,
            CancellationToken cancellationToken) =>
        {
            if (request.Message is { Length: > Consts.MaxMessageLength })
            {
                logger.LogWarning("Message has {Length} characters, the limit is {Limit}", request.Message.Length, Consts.MaxMessageLength);
                return Results.Problem(
                    statusCode: StatusCodes.Status413PayloadTooLarge,
                    title: "Message is too long",
                    detail: $"A message can have at most {Consts.MaxMessageLength} characters");
            }
            if (!request.Message.TryGetMessageIdentifier(out var messageHeader, out var messageFooter))
            {
                logger.LogWarning("Message header or footer could not be read");
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Message is not valid",
                    detail: "Message must have a 3 character header, at least one element line and a footer line");
            }
            var expectedFooter = Consts.END + messageHeader.Identifier;
            if (!string.Equals(messageFooter.EndIdentifier, expectedFooter, StringComparison.Ordinal))
            {
                logger.LogWarning("Message {Header} does not end with {ExpectedFooter}", messageHeader.Identifier, expectedFooter);
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Message is not valid",
                    detail: $"Message must end with {expectedFooter}");
            }

            var message = TextMessage.Create(request.Message, messageHeader, messageFooter);
            db.TextMessages.Add(message);
            await db.SaveChangesAsync(cancellationToken);
            return Results.Accepted(value: new TextMessageAcceptedResponse(message.Id));
        })
        .WithName("SubmitTextMessage")
        .WithSummary("Accepts a raw IATA baggage text message for asynchronous parsing")
        .WithDescription("Validates the envelope (3 character header, element lines, matching END line, at most 4000 characters), stores the raw text and returns its id. Parsing happens in the background.")
        .Produces<TextMessageAcceptedResponse>(StatusCodes.Status202Accepted)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status413PayloadTooLarge)
        .AllowAnonymous();
        return app;
    }
}
