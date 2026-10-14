using Aero.TextMessageParser.Api.Contracts;
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
                [Consts.F] = sp.GetRequiredService<ElementF>(),
                [Consts.G] = sp.GetRequiredService<ElementG>(),
                [Consts.H] = sp.GetRequiredService<ElementH>(),
                [Consts.I] = sp.GetRequiredService<ElementI>(),
                [Consts.J] = sp.GetRequiredService<ElementJ>(),
                [Consts.K] = sp.GetRequiredService<ElementK>(),
                [Consts.L] = sp.GetRequiredService<ElementL>(),
                [Consts.N] = sp.GetRequiredService<ElementN>(),
                [Consts.O] = sp.GetRequiredService<ElementO>(),
                [Consts.P] = sp.GetRequiredService<ElementP>(),
                [Consts.Q] = sp.GetRequiredService<ElementQ>(),
                [Consts.R] = sp.GetRequiredService<ElementR>(),
                [Consts.S] = sp.GetRequiredService<ElementS>(),
                [Consts.T] = sp.GetRequiredService<ElementT>(),
                [Consts.U] = sp.GetRequiredService<ElementU>(),
                [Consts.V] = sp.GetRequiredService<ElementV>(),
                [Consts.W] = sp.GetRequiredService<ElementW>(),
                [Consts.X] = sp.GetRequiredService<ElementX>(),
                [Consts.Y] = sp.GetRequiredService<ElementY>(),
            });

        services.AddSingleton<Bsm>();
        services.AddSingleton<BsmChg>();
        services.AddSingleton<BsmDel>();
        services.AddSingleton<Bum>();
        services.AddSingleton<BcmFom>();
        services.AddSingleton<BcmFcm>();
        services.AddSingleton<BcmBam>();
        services.AddSingleton<BcmDbm>();
        services.AddSingleton<IReadOnlyDictionary<string, IMessageHandler>>(sp =>
            new Dictionary<string, IMessageHandler>
            {
                [Consts.BSM]              = sp.GetRequiredService<Bsm>(),
                [Consts.BSM + Consts.CHG] = sp.GetRequiredService<BsmChg>(),
                [Consts.BSM + Consts.DEL] = sp.GetRequiredService<BsmDel>(),
                [Consts.BUM]              = sp.GetRequiredService<Bum>(),
                [Consts.BCM + Consts.FOM] = sp.GetRequiredService<BcmFom>(),
                [Consts.BCM + Consts.FCM] = sp.GetRequiredService<BcmFcm>(),
                [Consts.BCM + Consts.BAM] = sp.GetRequiredService<BcmBam>(),
                [Consts.BCM + Consts.DBM] = sp.GetRequiredService<BcmDbm>(),
            });

        return services;
    }

    public static IServiceCollection InstallElements(this IServiceCollection services)
    {
        services.AddSingleton(sp => new ElementA(new ElementAValidator()));
        services.AddSingleton(sp => new ElementB(new ElementBValidator()));
        services.AddSingleton(sp => new ElementC(new ElementCValidator()));
        services.AddSingleton(sp => new ElementD(new ElementDValidator()));
        services.AddSingleton(sp => new ElementE(new ElementEValidator()));
        services.AddSingleton(sp => new ElementF(new ElementFValidator()));
        services.AddSingleton(sp => new ElementG(new ElementGValidator()));
        services.AddSingleton(sp => new ElementH(new ElementHValidator()));
        services.AddSingleton(sp => new ElementI(new ElementIValidator()));
        services.AddSingleton(sp => new ElementJ(new ElementJValidator()));
        services.AddSingleton(sp => new ElementK(new ElementKValidator()));
        services.AddSingleton(sp => new ElementL(new ElementLValidator()));
        services.AddSingleton(sp => new ElementN(new ElementNValidator()));
        services.AddSingleton(sp => new ElementO(new ElementOValidator()));
        services.AddSingleton(sp => new ElementP(new ElementPValidator()));
        services.AddSingleton(sp => new ElementQ(new ElementQValidator()));
        services.AddSingleton(sp => new ElementR(new ElementRValidator()));
        services.AddSingleton(sp => new ElementS(new ElementSValidator()));
        services.AddSingleton(sp => new ElementT(new ElementTValidator()));
        services.AddSingleton(sp => new ElementU(new ElementUValidator()));
        services.AddSingleton(sp => new ElementV(new ElementVValidator()));
        services.AddSingleton(sp => new ElementW(new ElementWValidator()));
        services.AddSingleton(sp => new ElementX(new ElementXValidator()));
        services.AddSingleton(sp => new ElementY(new ElementYValidator()));

        return services;
    }
}
