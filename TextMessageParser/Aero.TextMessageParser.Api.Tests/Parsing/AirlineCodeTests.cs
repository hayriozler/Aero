using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.Parsers.Elements.Validators;
using Aero.TextMessageParser.Api.Tests.Infrastructure;

namespace Aero.TextMessageParser.Api.Tests.Parsing;

public class AirlineCodeTests
{
    [Theory(DisplayName = "Airline code: two character designators with a digit (4U, 9W, 3K, A1) and three letter codes are valid flight identifiers")]
    [InlineData("LH400")]
    [InlineData("EW9021")]
    [InlineData("9W123")]
    [InlineData("4U1234")]
    [InlineData("3K567")]
    [InlineData("A1234")]
    [InlineData("BAW123")]
    public void Valid_flight_identifiers(string value)
    {
        Check.True(ValidationHelper.ValidateAirlineFlightNumber(value.AsMemory(), out _),
            $"'{value}' is a valid airline designator plus flight number. Digit and letter designators such as 4U, 9W and 3K exist.");
    }

    [Theory(DisplayName = "Airline code: a designator without a letter, with other characters, or a flight number with letters is rejected")]
    [InlineData("12345")]
    [InlineData("L-400")]
    [InlineData("LH40A")]
    [InlineData("LH")]
    public void Invalid_flight_identifiers(string value)
    {
        Check.True(!ValidationHelper.ValidateAirlineFlightNumber(value.AsMemory(), out _), $"'{value}' must be rejected.");
    }

    [Fact(DisplayName = "Airline code: OutboundFlightElement parses 9W123 into airline 9W and flight number 123")]
    public void Element_f_splits_a_digit_airline()
    {
        var detail = ElementDetails.Make(".F/9W123/12NOV/JFK/Y", ElementRequirement.Conditional);
        var result = new OutboundFlightElement(new OutboundFlightElementValidator()).Parse(detail);
        Check.True(result.ValidationResult is { IsValid: true }, $"Rejected: {result.ValidationResult?.Description}");
        var element = (OutboundFlightElement)result.Element;
        Check.Equal("9W", element.AirlineCode, "Airline code.");
        Check.Equal("123", element.FlightNumber, "Flight number.");
    }

    [Theory(DisplayName = "Airline code: the inbound and onward flight elements accept digit airline codes too")]
    [InlineData(".I/9W123/05NOV/FRA/Y")]
    [InlineData(".O/4U1234/13NOV/MUC/Y")]
    public void Inbound_and_onward_accept_digit_airlines(string line)
    {
        IElementValidator validator = line.StartsWith(".I") ? new InboundFlightElementValidator() : new OnwardFlightElementValidator();
        var result = validator.Validate(ElementDetails.Make(line, ElementRequirement.Conditional));
        Check.True(result.IsValid, $"'{line}' was rejected: {result.ErrorCode} {result.Description}");
    }
}
