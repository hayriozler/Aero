using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.Parsers.Elements.Validators;
using Aero.TextMessageParser.Api.Parsers.Messages.Bsms;
using Aero.TextMessageParser.Api.Services;
using Aero.TextMessageParser.Api.Tests.Infrastructure;
using static Aero.TextMessageParser.Api.Tests.Infrastructure.SampleMessages;

namespace Aero.TextMessageParser.Api.Tests.Parsing;

public class ErrorCodeTests
{
    private static ElementValidationResult Validate(IElementValidator validator, string line)
        => validator.Validate(ElementDetails.Make(line, ElementRequirement.Conditional));

    [Theory(DisplayName = "Error codes: a validator reports the kind of problem (length, value, format, missing field) instead of one generic length code")]
    [InlineData(".N/022012345", ErrorCodes.ELEMENT_LENGTH_NOT_CORRECT, "N")]
    [InlineData(".N/022012345600A", ErrorCodes.ELEMENT_FORMAT_NOT_CORRECT, "N")]
    [InlineData(".N/0220123456000", ErrorCodes.ELEMENT_VALUE_NOT_CORRECT, "N")]
    [InlineData(".W/X/001/20", ErrorCodes.ELEMENT_VALUE_NOT_CORRECT, "W")]
    [InlineData(".P/DOE//JOHN", ErrorCodes.ELEMENT_FIELD_MISSING, "P")]
    public void Validators_report_the_kind_of_problem(string line, string expectedCode, string element)
    {
        const string Why = "Before the fix every length, digit, letter and allowed-value check used the same code 1001 (named length-not-correct), so an error row could not tell what was wrong.";
        IElementValidator validator = element switch
        {
            "N" => new ElementNValidator(),
            "W" => new ElementWValidator(),
            _ => new ElementPValidator()
        };
        var result = Validate(validator, line);
        Check.True(!result.IsValid, $"'{line}' must be rejected.");
        Check.Equal(expectedCode, result.ErrorCode, $"'{line}' got the wrong code ({result.Description}). {Why}");
    }

    [Fact(DisplayName = "ElementValidationResult: the validated element line is kept on the result, for invalid and valid elements")]
    public void Result_carries_the_element_line()
    {
        var bad = Validate(new ElementNValidator(), ".N/022012345600A");
        Check.Equal(".N/022012345600A", bad.Line, "The result must carry the line that failed.");
        var good = Validate(new ElementNValidator(), ".N/0220123456001");
        Check.True(good.IsValid, "A well-formed .N/ line is valid.");
        Check.Equal(".N/0220123456001", good.Line, "The line is available for valid results as well.");
    }

    [Fact(DisplayName = "Persistence: an element error row keeps the failing line in its description")]
    public void Element_error_row_contains_the_line()
    {
        using var db = TestDbContextFactory.Create();
        var dto = new BsmParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(BsmText(V, ".F/LH400", N1, P)), db);
        Check.True(dto is null, "A malformed .F/ line must not produce a DTO.");
        var error = db.ElementErrors.Local.Single();
        Check.True(error.ErrorDescription is not null && error.ErrorDescription.Contains(".F/LH400"),
            $"The element error must name the failing line; got '{error.ErrorDescription}'.");
    }

    [Fact(DisplayName = "Persistence: an element error description never exceeds the 250 character column limit")]
    public void Element_error_description_is_truncated()
    {
        using var db = TestDbContextFactory.Create();
        var longLine = ".R/" + new string('X', 400);
        var dto = new BsmParser(ElementMapFactory.Create()).ProcessMessage(TextMessages.From(BsmText(V, F, N1, longLine, P)), db);
        Check.True(dto is null, "An over-long .R/ free text must be rejected.");
        var error = db.ElementErrors.Local.Single();
        Check.True(error.ErrorDescription is { Length: <= 250 }, $"Description length was {error.ErrorDescription?.Length}.");
    }

    [Fact(DisplayName = "Error codes: every code is a compile-time constant, unique, and fits the 50 character error code column")]
    public void Codes_are_constants_unique_and_short()
    {
        var fields = typeof(ErrorCodes).GetFields().Where(f => f.FieldType == typeof(string)).ToList();
        Check.True(fields.Count > 0, "ErrorCodes must expose string codes.");
        Check.True(fields.All(f => f.IsLiteral), "Codes must be const, not static readonly: " + string.Join(", ", fields.Where(f => !f.IsLiteral).Select(f => f.Name)));
        var values = fields.Select(f => (string)f.GetRawConstantValue()!).ToList();
        var duplicates = values.GroupBy(v => v).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        Check.True(duplicates.Count == 0, "Two codes share a value: " + string.Join(", ", duplicates));
        Check.True(values.All(v => v.Length is > 0 and <= 50), "Every code must be 1 to 50 characters.");
    }

    [Fact(DisplayName = "Error codes: message level codes are built in one place from the message name")]
    public void Message_codes_are_centralised()
    {
        Check.Equal("MSG_BSM_END_INVALID", ErrorCodes.MessageEndInvalid("BSM"), "END code.");
        Check.Equal("MSG_BSM_CHG_EMPTY", ErrorCodes.MessageEmpty("BSM_CHG"), "Empty code.");
        Check.Equal("MSG_BCM_FOM_MISSING_MANDATORY", ErrorCodes.MessageMissingMandatory("BCM_FOM"), "Missing mandatory code.");
        Check.Equal("MSG_BSM_DUPLICATE", ErrorCodes.MessageDuplicate("BSM"), "Duplicate code.");
        Check.True(ErrorCodes.MessageMissingMandatory("BSM_CHG").Length <= 50, "Longest generated code must fit the column.");
    }
}
