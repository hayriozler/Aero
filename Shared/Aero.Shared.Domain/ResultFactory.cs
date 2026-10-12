namespace Aero.Shared.Domain;

public static class ResultFactory
{
    public static Result<T> Success<T>(T value)
       => new(value);

    public static Result<T> Failure<T>(string error)
        => new(error);

    public static Result Success()
        => Result.Success();

    public static Result Failure(string error)
        => Result.Failure(error);
}
