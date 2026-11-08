using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Persistence;
using Aero.TextMessageParser.Api.Requests;
using Aero.TextMessageParser.Api.Services;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.Parsers.Elements.Validators;
using Aero.TextMessageParser.Api.Parsers.Messages;
using Aero.TextMessageParser.Api.Parsers.Messages.Bcms;
using Aero.TextMessageParser.Api.Parsers.Messages.Bsms;

namespace Aero.TextMessageParser.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection InstallMessages(this IServiceCollection services)
    {
        // Element map — built once from all registered element singletons
        services.AddSingleton<IReadOnlyDictionary<string, Element>>(sp =>
            new Dictionary<string, Element>
            {
                [Consts.A] = sp.GetRequiredService<ElementA>(),
                [Consts.B] = sp.GetRequiredService<ElementB>(),
                [Consts.C] = sp.GetRequiredService<ElementC>(),
                [Consts.D] = sp.GetRequiredService<ElementD>(),
                [Consts.E] = sp.GetRequiredService<ElementE>(),
                [Consts.F] = sp.GetRequiredService<OutboundFlightElement>(),
                [Consts.G] = sp.GetRequiredService<ElementG>(),
                [Consts.H] = sp.GetRequiredService<ElementH>(),
                [Consts.I] = sp.GetRequiredService<InboundFlightElement>(),
                [Consts.J] = sp.GetRequiredService<ElementJ>(),
                [Consts.K] = sp.GetRequiredService<ElementK>(),
                [Consts.L] = sp.GetRequiredService<ElementL>(),
                [Consts.N] = sp.GetRequiredService<BagTagElement>(),
                [Consts.O] = sp.GetRequiredService<OnwardFlightElement>(),
                [Consts.P] = sp.GetRequiredService<PassengerNameElement>(),
                [Consts.Q] = sp.GetRequiredService<ElementQ>(),
                [Consts.R] = sp.GetRequiredService<ElementR>(),
                [Consts.S] = sp.GetRequiredService<PassengerStatusElement>(),
                [Consts.T] = sp.GetRequiredService<ElementT>(),
                [Consts.U] = sp.GetRequiredService<ElementU>(),
                [Consts.V] = sp.GetRequiredService<BaggageSourceElement>(),
                [Consts.W] = sp.GetRequiredService<BaggageWeightElement>(),
                [Consts.X] = sp.GetRequiredService<ElementX>(),
                [Consts.Y] = sp.GetRequiredService<ElementY>(),
            });

        services.AddSingleton<BsmParser>();
        services.AddSingleton<BsmChgParser>();
        services.AddSingleton<BsmDelParser>();
        services.AddSingleton<BumParser>();
        services.AddSingleton<BcmFomParser>();
        services.AddSingleton<BcmFcmParser>();
        services.AddSingleton<BcmDbmParser>();
        services.AddSingleton<IReadOnlyDictionary<string, IMessageHandler>>(sp =>
            new Dictionary<string, IMessageHandler>
            {
                [Consts.BSM]              = sp.GetRequiredService<BsmParser>(),
                [Consts.BSM + Consts.CHG] = sp.GetRequiredService<BsmChgParser>(),
                [Consts.BSM + Consts.DEL] = sp.GetRequiredService<BsmDelParser>(),
                [Consts.BUM]              = sp.GetRequiredService<BumParser>(),
                [Consts.BCM + Consts.FOM] = sp.GetRequiredService<BcmFomParser>(),
                [Consts.BCM + Consts.FCM] = sp.GetRequiredService<BcmFcmParser>(),
                [Consts.BCM + Consts.DBM] = sp.GetRequiredService<BcmDbmParser>(),
            });

        return services;
    }

    public static IServiceCollection InstallProcessors(this IServiceCollection services)
    {
        services.AddProcessor<BsmParser, DepartureBagDto>(Consts.BSM);
        services.AddProcessor<BsmChgParser, DepartureBagDto>(Consts.BSM + Consts.CHG);
        services.AddProcessor<BsmDelParser, BagRemovalDto>(Consts.BSM + Consts.DEL);
        services.AddProcessor<BumParser, BagRemovalDto>(Consts.BUM);
        services.AddProcessor<BcmFomParser, BcmDto>(Consts.BCM + Consts.FOM);
        services.AddProcessor<BcmFcmParser, BcmDto>(Consts.BCM + Consts.FCM);
        services.AddProcessor<BcmDbmParser, BcmDto>(Consts.BCM + Consts.DBM);

        return services;
    }

    private static void AddProcessor<TParser, TResult>(this IServiceCollection services, string key)
        where TParser : MessageParser<TResult>
        where TResult : class
        => services.AddKeyedScoped<IMessageProcessor>(key, (sp, _) =>
            new MessageProcessor<TResult>(
                sp.GetRequiredService<TParser>(),
                sp.GetRequiredService<TextMessageParserDbContext>()));

    public static IServiceCollection InstallElements(this IServiceCollection services)
    {
        services.AddSingleton(sp => new ElementA(new ElementAValidator()));
        services.AddSingleton(sp => new ElementB(new ElementBValidator()));
        services.AddSingleton(sp => new ElementC(new ElementCValidator()));
        services.AddSingleton(sp => new ElementD(new ElementDValidator()));
        services.AddSingleton(sp => new ElementE(new ElementEValidator()));
        services.AddSingleton(sp => new OutboundFlightElement(new OutboundFlightElementValidator()));
        services.AddSingleton(sp => new ElementG(new ElementGValidator()));
        services.AddSingleton(sp => new ElementH(new ElementHValidator()));
        services.AddSingleton(sp => new InboundFlightElement(new InboundFlightElementValidator()));
        services.AddSingleton(sp => new ElementJ(new ElementJValidator()));
        services.AddSingleton(sp => new ElementK(new ElementKValidator()));
        services.AddSingleton(sp => new ElementL(new ElementLValidator()));
        services.AddSingleton(sp => new BagTagElement(new BagTagElementValidator()));
        services.AddSingleton(sp => new OnwardFlightElement(new OnwardFlightElementValidator()));
        services.AddSingleton(sp => new PassengerNameElement(new PassengerNameElementValidator()));
        services.AddSingleton(sp => new ElementQ(new ElementQValidator()));
        services.AddSingleton(sp => new ElementR(new ElementRValidator()));
        services.AddSingleton(sp => new PassengerStatusElement(new PassengerStatusElementValidator()));
        services.AddSingleton(sp => new ElementT(new ElementTValidator()));
        services.AddSingleton(sp => new ElementU(new ElementUValidator()));
        services.AddSingleton(sp => new BaggageSourceElement(new BaggageSourceElementValidator()));
        services.AddSingleton(sp => new BaggageWeightElement(new BaggageWeightElementValidator()));
        services.AddSingleton(sp => new ElementX(new ElementXValidator()));
        services.AddSingleton(sp => new ElementY(new ElementYValidator()));

        return services;
    }
}
