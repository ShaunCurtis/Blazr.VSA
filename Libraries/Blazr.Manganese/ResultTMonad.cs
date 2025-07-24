/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result<T>
{
    internal readonly Exception? _exception;
    internal readonly T? _value;
 
    private ResultException _defaultException => new ResultException("An error occurred. No specific exception provided.");
    
    public bool HasException => _exception is not null;

    public bool HasValue => _exception is null;

    private Result(T? value)
        => _value = value;

    private Result(Exception? exception)
        => _exception = exception ?? _defaultException;

    private Result()
        => _exception = _defaultException;

    public static Result<T> Create(T? value) =>
        value is null
            ? new(new ResultException("T was null."))
            : new(value);

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Exception exception) => new(exception);

    public static Result<T> Failure(string message) => new(new ResultException(message));

    public Result AsResult => this._exception is null
            ? Result.Success()
            : Result.Failure(_exception);

    public Result Output(Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (HasValue)
            hasValue?.Invoke(_value!);
        else
            hasException?.Invoke(_exception!);

        return this.AsResult;
    }

    public ValueTask<Result<T>> CompletedValueTask
        => ValueTask.FromResult(this);

    public Task<Result<T>> CompletedTask
        => Task.FromResult(this);
}
