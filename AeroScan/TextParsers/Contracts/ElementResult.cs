using IataText.Parser.Parsers.Elements;

namespace IataText.Parser.Contracts;


public record ElementResult(Element Element, ElementValidationResult? ValidationResult);
