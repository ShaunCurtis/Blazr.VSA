/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public abstract record Result<T>
{
    private Result() { }

    public sealed record Success(T Value) : Result<T>;
    public sealed record Failed(string message) : Result<T>;
    public sealed record Error(Exception exception) : Result<T>;

    public Result AsResult
        => this switch
        {
            Success => Result.Successful(),
            Failed @failed => Result.Failure(@failed.message),
            Error @error => Result.Failure(@error.exception.Message),
            _ => throw new PatternMatchException()
        };

    public static Result<T> Read(T? value)
        => value is null
            ? ResultT.Failure<T>("Value is null")
            : ResultT.Successful(value);

    public static Result<T> Read(T? value, string errorMessage)
        => value is null
            ? ResultT.Failure<T>(errorMessage)
            : ResultT.Successful(value);

    public static Result<T> Successful(T Value) => new Result<T>.Success(Value);
    public static Result<T> Failure(string message) => new Result<T>.Failed(message);
    public static Result<T> Exception(Exception exception) => new Result<T>.Error(exception);
}

public static class ResultT
{
    public static Result<T> Successful<T>(T value) => Result<T>.Successful(value);
    public static Result<T> Failure<T>(string message) => Result<T>.Failure(message);
    public static Result<T> Exception<T>(Exception exception) => Result<T>.Exception(exception);
}
