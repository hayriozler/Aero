using BaggageService.Services.MessageHandlers;
using Contracts.Dtos;
using IataText.Parser.Contracts;
using IataText.Parser.Parsers;
using IataText.Parser.Parsers.Messages;

namespace BaggageService.Extensions;

public static class MessageDispatcherExtensions
{
    public static IServiceCollection InstallMessageDispatchers(this IServiceCollection services)
    {
        services.AddSingleton<IMessageResultHandler<TextMessageDepartureBagDto>,    BsmMessageResultHandler>();
        services.AddSingleton<IMessageResultHandler<TextMessageDepartureBagChgDto>, BsmMessageChgHandler>();
        services.AddSingleton<IMessageResultHandler<TextMessageDepartureBagDeleteDto>, BsmMessageDeleteHandler>();
        services.AddSingleton<IMessageResultHandler<TextMessageBcmDto>,             BcmMessageResultHandler>();
        services.AddSingleton<IMessageResultHandler<TextMessageBumDto>,             BumMessageResultHandler>();

        services.AddSingleton<IReadOnlyDictionary<string, IMessageParserDispatcher>>(sp =>
        {
            var parsers = sp.GetRequiredService<IReadOnlyDictionary<string, IMessageHandler>>();

            return new Dictionary<string, IMessageParserDispatcher>
            {
                [Consts.BSM] = new MessageDispatcher<TextMessageDepartureBagDto>(
                    (MessageBase<TextMessageDepartureBagDto>)parsers[Consts.BSM],
                    sp.GetRequiredService<IMessageResultHandler<TextMessageDepartureBagDto>>()),

                [Consts.BSM + Consts.CHG] = new MessageDispatcher<TextMessageDepartureBagChgDto>(
                    (MessageBase<TextMessageDepartureBagChgDto>)parsers[Consts.BSM + Consts.CHG],
                    sp.GetRequiredService<IMessageResultHandler<TextMessageDepartureBagChgDto>>()),

                [Consts.BSM + Consts.DEL] = new MessageDispatcher<TextMessageDepartureBagDeleteDto>(
                    (MessageBase<TextMessageDepartureBagDeleteDto>)parsers[Consts.BSM + Consts.DEL],
                    sp.GetRequiredService<IMessageResultHandler<TextMessageDepartureBagDeleteDto>>()),

                [Consts.BUM] = new MessageDispatcher<TextMessageBumDto>(
                    (MessageBase<TextMessageBumDto>)parsers[Consts.BUM],
                    sp.GetRequiredService<IMessageResultHandler<TextMessageBumDto>>()),

                [Consts.BCM + Consts.FOM] = new MessageDispatcher<TextMessageBcmDto>(
                    (MessageBase<TextMessageBcmDto>)parsers[Consts.BCM + Consts.FOM],
                    sp.GetRequiredService<IMessageResultHandler<TextMessageBcmDto>>()),

                [Consts.BCM + Consts.FCM] = new MessageDispatcher<TextMessageBcmDto>(
                    (MessageBase<TextMessageBcmDto>)parsers[Consts.BCM + Consts.FCM],
                    sp.GetRequiredService<IMessageResultHandler<TextMessageBcmDto>>()),

                [Consts.BCM + Consts.BAM] = new MessageDispatcher<TextMessageBcmDto>(
                    (MessageBase<TextMessageBcmDto>)parsers[Consts.BCM + Consts.BAM],
                    sp.GetRequiredService<IMessageResultHandler<TextMessageBcmDto>>()),

                [Consts.BCM + Consts.DBM] = new MessageDispatcher<TextMessageBcmDto>(
                    (MessageBase<TextMessageBcmDto>)parsers[Consts.BCM + Consts.DBM],
                    sp.GetRequiredService<IMessageResultHandler<TextMessageBcmDto>>()),
            };
        });

        return services;
    }
}
