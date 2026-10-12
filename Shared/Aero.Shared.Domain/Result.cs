namespace Aero.Shared.Domain;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T? Value { get; }
    public string? Error { get; }

    internal Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
    }

    internal Result(string error)
    {
        IsSuccess = false;
        Error = error;
        Value = default;
    }

    public TOut Match<TOut>(
        Func<T, TOut> onSuccess,
        Func<string, TOut> onFailure) => IsSuccess
            ? onSuccess(Value!)
            : onFailure(Error!);
}