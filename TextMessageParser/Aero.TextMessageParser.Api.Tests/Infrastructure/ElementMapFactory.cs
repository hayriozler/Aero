using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.Parsers.Elements.Validators;

namespace Aero.TextMessageParser.Api.Tests.Infrastructure;

internal static class ElementMapFactory
{
    public static Dictionary<string, Element> Create() => new()
    {
        [Consts.A] = new ElementA(new ElementAValidator()),
        [Consts.B] = new ElementB(new ElementBValidator()),
        [Consts.C] = new ElementC(new ElementCValidator()),
        [Consts.D] = new ElementD(new ElementDValidator()),
        [Consts.E] = new ElementE(new ElementEValidator()),
        [Consts.F] = new OutboundFlightElement(new OutboundFlightElementValidator()),
        [Consts.G] = new ElementG(new ElementGValidator()),
        [Consts.H] = new ElementH(new ElementHValidator()),
        [Consts.I] = new InboundFlightElement(new InboundFlightElementValidator()),
        [Consts.J] = new ElementJ(new ElementJValidator()),
        [Consts.K] = new ElementK(new ElementKValidator()),
        [Consts.L] = new ElementL(new ElementLValidator()),
        [Consts.N] = new BagTagElement(new BagTagElementValidator()),
        [Consts.O] = new OnwardFlightElement(new OnwardFlightElementValidator()),
        [Consts.P] = new PassengerNameElement(new PassengerNameElementValidator()),
        [Consts.Q] = new ElementQ(new ElementQValidator()),
        [Consts.R] = new ElementR(new ElementRValidator()),
        [Consts.S] = new PassengerStatusElement(new PassengerStatusElementValidator()),
        [Consts.T] = new ElementT(new ElementTValidator()),
        [Consts.U] = new ElementU(new ElementUValidator()),
        [Consts.V] = new BaggageSourceElement(new BaggageSourceElementValidator()),
        [Consts.W] = new BaggageWeightElement(new BaggageWeightElementValidator()),
        [Consts.X] = new ElementX(new ElementXValidator()),
        [Consts.Y] = new ElementY(new ElementYValidator()),
    };

    public static T Get<T>(IReadOnlyDictionary<string, Element> map) where T : Element
        => map.Values.OfType<T>().Single();
}
