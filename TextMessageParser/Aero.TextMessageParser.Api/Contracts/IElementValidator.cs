namespace Aero.TextMessageParser.Api.Contracts;


public interface IElementValidator
{
    ElementValidationResult Validate(ElementDetail elementDetail);
}
