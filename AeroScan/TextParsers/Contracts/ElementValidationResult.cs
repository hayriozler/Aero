namespace IataText.Parser.Contracts;


public record ElementValidationResult
{
    public string ErrorCode { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;
    public bool IsValid => string.IsNullOrEmpty(ErrorCode);
    public void AddError(string errorCode, string? description = null)
    {
        ErrorCode = errorCode;
        Description = description;
    }
}