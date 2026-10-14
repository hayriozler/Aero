namespace Aero.TextMessageParser.Api.Parsers.Elements;

public record ElementSequence(string ElementName, ElementRequirement Requirement = ElementRequirement.Optional,
    bool IsMultiple = false);
