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
    private readonly Exception? _exception;
    public bool HasException => _exception is not null;

    private Result(Exception? exception)
        => _exception = exception
            ?? new ResultException("An error occurred. No specific exception provided.");

    private Result() { }

    public static Result Success() => new();
    public static Result Failure(Exception? exception) => new(exception);
    public static Result Failure(string message) => new(new ResultException(message));

    public Result ApplyTransform(Func<Result> transform)
        => _exception is null
            ? transform()
            : this;

    public Result<T> ApplyTransform<T>(Func<Result<T>> transform)
        => _exception is null
            ? transform()
            : Result<T>.Failure(_exception);

    public Result ApplySideEffect(Action? hasNoException = null, Action<Exception>? hasException = null)
    {
        Output(hasNoException, hasException);
        return this;
    }

    public Result Output(Action? hasNoException = null, Action<Exception>? hasException = null)
    {
        if (HasException)
            hasException?.Invoke(_exception!);
        else
            hasNoException?.Invoke();

        return this;
    }
    public ValueTask<Result> CompletedValueTask
        => ValueTask.FromResult(this);

    public Task<Result> CompletedTask
        => Task.FromResult(this);
}

public static class ResultExtensions
{
    public static Result ApplyTransform(this Result result, bool test, Func<Result> trueTransform, Func<Result> falseTransform)
        => test
            ? result.ApplyTransform(trueTransform)
            : result.ApplyTransform(falseTransform);

    public static Result ApplyTransformOnException(this Result result, bool test, string message)
        => result.HasException && test
            ? Result.Failure(message)
            : result;

    public static Result ApplyTransformOnException(this Result result, bool test, Exception exception)
        => result.HasException && test
                ? Result.Failure(exception)
                : result;

    public static Result ApplySideEffect(this Result result, Action hasNoException)
    {
        result.Output(hasNoException, null);
        return result;
    }

    public static Result ApplySideEffect(this Result result, bool test, Action? trueAction = null, Action? falseAction = null)
    {

        if (!result.HasException)
            if (test)
                trueAction?.Invoke();
            else
                falseAction?.Invoke();

        return result;
    }
}

public static class ResultTaskExtensions
{
    public async static Task<Result> ApplyTransformAsync(this Result result, Func<Task<Result>> transform)
        => result.HasException
            ? result
            : await transform();

    public async static Task<Result> ApplyTransformAsync(this Result result, bool test, Func<Task<Result>> trueTransform, Func<Task<Result>> falseTransform)
        => result.HasException
        ? result
        : test
            ? await trueTransform()
            : await falseTransform();
}

