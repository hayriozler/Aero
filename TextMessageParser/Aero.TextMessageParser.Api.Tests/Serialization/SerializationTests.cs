using System.Text.Json;
using Aero.TextMessageParser.Api.Dtos;
using Aero.TextMessageParser.Api.Requests;
using Aero.TextMessageParser.Api.Tests.Infrastructure;

namespace Aero.TextMessageParser.Api.Tests.Serialization;

public class SerializationTests
{
    private static readonly List<(string AirlineCode, string FlightNumber, string FlightIataDate, string Destination, char? Class)> Onwards =
        [("LH", "401", "13NOV", "MUC", 'Y'), ("EW", "9021", "14NOV", "BER", 'C')];

    private static readonly string[] Expected = ["LH", "401", "13NOV", "MUC", "EW", "9021", "14NOV", "BER"];


    public static TheoryData<string> OptionNames => new() { "default", "web" };

    private static JsonSerializerOptions OptionsFor(string name) =>
        name == "web" ? new JsonSerializerOptions(JsonSerializerDefaults.Web) : new JsonSerializerOptions();

    private static void AssertAllPresent(string json, string kind)
    {
        var missing = Expected.Where(e => !json.Contains(e)).ToList();
        Check.True(missing.Count == 0,
            $"{kind}: onward flights serialized as {json}. Missing: {string.Join(", ", missing)}. ValueTuple members are public FIELDS and System.Text.Json (also under HttpClient.PostAsJsonAsync) ignores fields by default, so every onward becomes {{}}. Use a record/class for onwards or IncludeFields.");
    }

    [Theory(DisplayName = "Bug 10: TextMessageDepartureBagRequest serializes every onward flight (airline, number, date, destination)")]
    [Trait("Bug", "10")]
    [MemberData(nameof(OptionNames))]
    public void Request_onwards_are_serialized(string options)
    {
        var dto = new TextMessageDepartureBagRequest(1, null, true, true, 'L', "FRA", "LH", "400", "12NOV", "JFK",
            true, ["0220123456"], Onwards, 'B', "JOHN DOE", null, "001", 'Y', "12A");
        AssertAllPresent(JsonSerializer.Serialize(dto, OptionsFor(options)), "TextMessageDepartureBagRequest");
    }

    [Theory(DisplayName = "Bug 10: TextMessageDepartureBagChgDto serializes every onward flight (airline, number, date, destination)")]
    [Trait("Bug", "10")]
    [MemberData(nameof(OptionNames))]
    public void Chg_onwards_are_serialized(string options)
    {
        var dto = new TextMessageDepartureBagChgDto(1, null, true, true, 'L', "FRA", "LH", "400", "12NOV", "JFK",
            true, ["0220123456"], Onwards, 'B', "JOHN DOE", null, "001", 'Y', "12A");
        AssertAllPresent(JsonSerializer.Serialize(dto, OptionsFor(options)), "TextMessageDepartureBagChgDto");
    }

    [Fact(DisplayName = "Bug 10 (current behavior): onward flights serialize as empty JSON objects")]
    [Trait("Bug", "10")]
    public void Onwards_currently_serialize_as_empty_objects()
    {
        var dto = new TextMessageDepartureBagRequest(1, null, true, true, 'L', "FRA", "LH", "400", "12NOV", "JFK",
            true, ["0220123456"], Onwards, 'B', "JOHN DOE", null, "001", 'Y', "12A");
        using var doc = JsonDocument.Parse(JsonSerializer.Serialize(dto, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
        var first = doc.RootElement.GetProperty("onwards")[0];
        Check.True(first.ValueKind == JsonValueKind.Object && !first.EnumerateObject().Any(),
            $"Documents today's behavior; first onward serialized as {first.GetRawText()}.");
    }

    [Fact(DisplayName = "Baseline: scalar properties and bag tags of the request DTO serialize")]
    [Trait("Kind", "Baseline")]
    public void Scalars_are_serialized()
    {
        var dto = new TextMessageDepartureBagRequest(1, null, true, true, 'L', "FRA", "LH", "400", "12NOV", "JFK",
            true, ["0220123456"], [], 'B', "JOHN DOE", null, "001", 'Y', "12A");
        var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Check.True(json.Contains("0220123456") && json.Contains("JOHN DOE") && json.Contains("\"flightNumber\":\"400\""), "Got " + json);
    }
}
