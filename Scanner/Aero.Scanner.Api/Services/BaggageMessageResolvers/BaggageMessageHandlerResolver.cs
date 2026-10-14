using Aero.Scanner.Api.Services.BaggageMessagesHandlers;

namespace Aero.Scanner.Api.Services.BaggageMessageResolvers;

public sealed class BaggageMessageHandlerResolver<T>(IServiceProvider serviceProvider)
        : IBaggageMessageHandlerResolver<T>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IBaggageMessageHandler<T> Resolve()
    {
        var handlerType = typeof(IBaggageMessageHandler<>).MakeGenericType(typeof(T));

        return (IBaggageMessageHandler<T>)_serviceProvider.GetRequiredService(handlerType);
    }
}