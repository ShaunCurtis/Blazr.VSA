/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

/// <summary>
///  Object implementing a functional approach to result management
/// All constructors are private
/// Create instances through the provided static methods
/// </summary>

public partial record Result
{
    public readonly Exception? Exception;
    public bool HasException => Exception is not null;

    private Result(Exception? exception)
        => Exception = exception
            ?? new ResultException("An error occurred. No specific exception provided.");

    private Result() { }

    public static Result Success() => new();
    public static Result Failure(Exception? exception) => new(exception);
    public static Result Failure(string message) => new(new ResultException(message));

    public static Result Success(bool test, Exception failureException)
        => test ? Result.Success() : Result.Failure(failureException);
    public static Result Success(bool test, string failureMessage)
        => test ? Result.Success() : Result.Failure(failureMessage);
    public static Result Failure(bool test, string failureMessage)
        => test ? Result.Failure(failureMessage) : Result.Success();

    public Result Output(Action? hasNoException = null, Action<Exception>? hasException = null)
    {
        if (HasException)
            hasException?.Invoke(Exception!);
        else
            hasNoException?.Invoke();

        return this;
    }
    
    public Result<T> OutputToResult<T>(Func<Result<T>> map)
        => HasException
            ? Result<T>.Failure(this.Exception!)
            : map.Invoke();

    public bool OutputValue()
        => !this.HasException;

    public ValueTask<Result> CompletedValueTask
        => ValueTask.FromResult(this);

    public Task<Result> CompletedTask
        => Task.FromResult(this);
}
