namespace Aero.Fids.Infrastructure.Flights;

public static class ResourceKinds
{
    public const string Gate = "Gate";
    public const string Belt = "Belt";
    public const string Counter = "Counter";
    public const string Spot = "Spot";

    public static readonly IReadOnlyList<string> All = [Gate, Belt, Counter, Spot];
}
