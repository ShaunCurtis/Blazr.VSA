/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result<T>
{
    public readonly Exception? Exception;
    public readonly T? Value;

    private ResultException _defaultException => new ResultException("An error occurred. No specific exception provided.");

    public bool HasException => Exception is not null;

    public bool HasValue => Exception is null;

    private Result(T? value)
        => Value = value;

    private Result(Exception? exception)
        => Exception = exception ?? _defaultException;

    private Result()
        => Exception = _defaultException;

    public static Result<T> Create(T? value) =>
    value is null
        ? new(new ResultException("T was null."))
        : new(value);

    public static Result<T> Create(T? value, string errorMessage) =>
        value is null
            ? new(new ResultException(errorMessage))
            : new(value);

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Exception exception) => new(exception);

    public static Result<T> Failure(string message) => new(new ResultException(message));
}
