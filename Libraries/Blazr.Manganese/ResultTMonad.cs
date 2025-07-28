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

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Exception exception) => new(exception);

    public static Result<T> Failure(string message) => new(new ResultException(message));

    public Result ToResult => this.Exception is null
            ? Result.Success()
            : Result.Failure(Exception);

    public Result<T> Output(Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (HasValue)
            hasValue?.Invoke(Value!);
        else
            hasException?.Invoke(Exception!);

        return this;
    }

    public T OutputValue(Func<Exception, T> hasException)
    {
        if (this.HasException)
            return hasException.Invoke(Exception!);

        return this.Value!;
    }

    public ValueTask<Result<T>> CompletedValueTask
        => ValueTask.FromResult(this);

    public Task<Result<T>> CompletedTask
        => Task.FromResult(this);
}
