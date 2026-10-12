namespace Aero.MessageProcessor.TextMessageProcessor.Contracts;


public interface IElementValidator
{
    ElementValidationResult Validate(ElementDetail elementDetail);
}
