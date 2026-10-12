
namespace IataText.Parser.Contracts;


public interface IElementValidator
{
    ElementValidationResult Validate(ElementDetail elementDetail);
}
