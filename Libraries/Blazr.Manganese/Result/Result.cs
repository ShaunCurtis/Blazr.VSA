/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public abstract record Result<T>
{
    private Result() { }

    public sealed record SuccessWithValue(T Value) : Result<T>;
    public sealed record SuccessWithoutValue() : Result<T>;
    public sealed record Failed(string message) : Result<T>;
    public sealed record Error(Exception exception) : Result<T>;

    public static Result<T> Successful(T Value) => new Result<T>.SuccessWithValue(Value);
    public static Result<T> Successful() => new Result<T>.SuccessWithoutValue();
    public static Result<T> Failure(string message) => new Result<T>.Failed(message);
    public static Result<T> Exception(Exception exception) => new Result<T>.Error(exception);
}
