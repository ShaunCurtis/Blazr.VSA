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

    public Result Output(Action? hasNoException = null, Action<Exception>? hasException = null)
    {
        if (HasException)
            hasException?.Invoke(Exception!);
        else
            hasNoException?.Invoke();

        return this;
    }

    public bool OutputValue()
        => !this.HasException;

    public ValueTask<Result> CompletedValueTask
        => ValueTask.FromResult(this);

    public Task<Result> CompletedTask
        => Task.FromResult(this);
}
