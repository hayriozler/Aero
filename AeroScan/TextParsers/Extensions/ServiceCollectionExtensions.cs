using Contracts.Dtos;
using IataText.Parser.Contracts;
using IataText.Parser.Parsers.Elements;
using IataText.Parser.Parsers.Elements.Validators;
using IataText.Parser.Parsers.Messages;
using IataText.Parser.Parsers.Messages.Bcms;
using IataText.Parser.Parsers.Messages.Bsms;
using Microsoft.Extensions.DependencyInjection;

namespace IataText.Parser.Extensions;

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
        services.AddSingleton<ElementAValidator>();
        services.AddSingleton<ElementBValidator>();
        services.AddSingleton<ElementCValidator>();
        services.AddSingleton<ElementDValidator>();
        services.AddSingleton<ElementEValidator>();
        services.AddSingleton<ElementFValidator>();
        services.AddSingleton<ElementGValidator>();
        services.AddSingleton<ElementHValidator>();
        services.AddSingleton<ElementIValidator>();
        services.AddSingleton<ElementJValidator>();
        services.AddSingleton<ElementKValidator>();
        services.AddSingleton<ElementLValidator>();
        services.AddSingleton<ElementNValidator>();
        services.AddSingleton<ElementOValidator>();
        services.AddSingleton<ElementPValidator>();
        services.AddSingleton<ElementQValidator>();
        services.AddSingleton<ElementRValidator>();
        services.AddSingleton<ElementSValidator>();
        services.AddSingleton<ElementTValidator>();
        services.AddSingleton<ElementUValidator>();
        services.AddSingleton<ElementVValidator>();
        services.AddSingleton<ElementWValidator>();
        services.AddSingleton<ElementXValidator>();
        services.AddSingleton<ElementYValidator>();

        services.AddSingleton(sp => new ElementA(sp.GetRequiredService<ElementAValidator>()));
        services.AddSingleton(sp => new ElementB(sp.GetRequiredService<ElementBValidator>()));
        services.AddSingleton(sp => new ElementC(sp.GetRequiredService<ElementCValidator>()));
        services.AddSingleton(sp => new ElementD(sp.GetRequiredService<ElementDValidator>()));
        services.AddSingleton(sp => new ElementE(sp.GetRequiredService<ElementEValidator>()));
        services.AddSingleton(sp => new ElementF(sp.GetRequiredService<ElementFValidator>()));
        services.AddSingleton(sp => new ElementG(sp.GetRequiredService<ElementGValidator>()));
        services.AddSingleton(sp => new ElementH(sp.GetRequiredService<ElementHValidator>()));
        services.AddSingleton(sp => new ElementI(sp.GetRequiredService<ElementIValidator>()));
        services.AddSingleton(sp => new ElementJ(sp.GetRequiredService<ElementJValidator>()));
        services.AddSingleton(sp => new ElementK(sp.GetRequiredService<ElementKValidator>()));
        services.AddSingleton(sp => new ElementL(sp.GetRequiredService<ElementLValidator>()));
        services.AddSingleton(sp => new ElementN(sp.GetRequiredService<ElementNValidator>()));
        services.AddSingleton(sp => new ElementO(sp.GetRequiredService<ElementOValidator>()));
        services.AddSingleton(sp => new ElementP(sp.GetRequiredService<ElementPValidator>()));
        services.AddSingleton(sp => new ElementQ(sp.GetRequiredService<ElementQValidator>()));
        services.AddSingleton(sp => new ElementR(sp.GetRequiredService<ElementRValidator>()));
        services.AddSingleton(sp => new ElementS(sp.GetRequiredService<ElementSValidator>()));
        services.AddSingleton(sp => new ElementT(sp.GetRequiredService<ElementTValidator>()));
        services.AddSingleton(sp => new ElementU(sp.GetRequiredService<ElementUValidator>()));
        services.AddSingleton(sp => new ElementV(sp.GetRequiredService<ElementVValidator>()));
        services.AddSingleton(sp => new ElementW(sp.GetRequiredService<ElementWValidator>()));
        services.AddSingleton(sp => new ElementX(sp.GetRequiredService<ElementXValidator>()));
        services.AddSingleton(sp => new ElementY(sp.GetRequiredService<ElementYValidator>()));

        return services;
    }
}
