using Aero.Scanner.Api.Services.BaggageMessagesHandlers;

namespace Aero.Scanner.Api.Services.BaggageMessageResolvers;

public interface IBaggageMessageHandlerResolver<T>
{
    IBaggageMessageHandler<T> Resolve();
}
