namespace IataText.Parser.ValueObjects;

public sealed record MessageValidationResult(Message Msg)
{
    public string? ErrorCode  { get; private set; }
    public string? Description { get; private set; }
    public bool IsValid => string.IsNullOrEmpty(ErrorCode);

    public void AddError(string errorCode, string description)
    {
        ErrorCode   = errorCode;
        Description = description;
    }
}
