using Aero.TextMessageParser.Api.Parsers.Elements;

namespace Aero.TextMessageParser.Api.Contracts;


public record ElementResult(Element Element, ElementValidationResult? ValidationResult);
