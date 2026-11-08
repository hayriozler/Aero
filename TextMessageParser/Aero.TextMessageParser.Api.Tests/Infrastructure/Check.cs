using Xunit.Sdk;

namespace Aero.TextMessageParser.Api.Tests.Infrastructure;

internal static class Check
{
    public static void True(bool condition, string because)
    {
        if (!condition) throw new XunitException(because);
    }

    public static void Equal<T>(T expected, T actual, string because)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new XunitException($"Expected: <{expected}> but actual: <{actual}>. {because}");
    }

    public static void SequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string because)
    {
        var e = expected.ToArray();
        var a = actual.ToArray();
        if (!e.SequenceEqual(a))
            throw new XunitException($"Expected: [{string.Join(", ", e)}] but actual: [{string.Join(", ", a)}]. {because}");
    }

    public static void Fail(string because) => throw new XunitException(because);
}
