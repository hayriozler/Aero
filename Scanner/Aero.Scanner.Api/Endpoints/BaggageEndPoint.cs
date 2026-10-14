using Aero.Scanner.Api.Services.BaggageMessageResolvers;
using Aero.Scanner.Shared.Requests;

namespace Aero.Scanner.Api.Endpoints;

public static class BaggageEndPoint
{
    public static IEndpointRouteBuilder MapBaggagesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/baggagemessages").WithTags("Baggages");

        group.MapPost("/", async (MessageRequest request, IBaggageMessageHandlerResolver<MessageRequest> resolver) =>
        {
            var handler = resolver.Resolve();
            await handler.HandleAsync(request, default);

        })
        .WithName("Baggages")
        .Produces<bool>()
        .ProducesProblem(404)
        .AllowAnonymous();
        return app;
    }
}
