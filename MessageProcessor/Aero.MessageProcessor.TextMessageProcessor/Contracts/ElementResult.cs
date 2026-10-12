using Aero.MessageProcessor.TextMessageProcessor.Parsers.Elements;

namespace Aero.MessageProcessor.TextMessageProcessor.Contracts;


public record ElementResult(Element Element, ElementValidationResult? ValidationResult);
