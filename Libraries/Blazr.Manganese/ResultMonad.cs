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

    private Result(Exception? exception)
        => _exception = exception
            ?? new ResultException("An error occurred. No specific exception provided.");

    private Result() { }

    public static Result Success() => new();
    public static Result Failure(Exception? exception) => new(exception);
    public static Result Failure(string message) => new(new ResultException(message));

    public Result ApplyTransform(Func<Result> mapping)
        => _exception is null
            ? mapping()
            : this;
}

public partial record Result
{
    public Result<T> ApplyTransform<T>(Func<Result<T>> onResult, Func<Exception, Result<T>>? onException = null)
    {
        if (_exception is null)
            return onResult();

        if (_exception is not null && failure != null)
            return onException(_exception);

        return Result<T>.Failure(_exception!);
    }

    public Result ApplyTransform(bool test, Func<Result> isTrue, Func<Result> isFalse)
    {
        if (_exception is not null)
            return this;

        if (test)
            return isTrue();

        return isFalse();
    }

    public Result ApplyTransformOnException(bool test, string message)
        => this.ApplyTransformOnException(test, new ResultException(message));

    public Result ApplyTransformOnException(bool test, Exception exception)
    {
        if (_exception is not null)
            return this;

        if (test)
            return Result.Failure(exception);

        return this;
    }

    public async Task<Result> ApplyTransformOnException(bool test, Func<Task<Result>> isTrue, Func<Task<Result>> isFalse)
    {
        if (_exception is not null)
            return this;

        return test ? await isTrue() : await isFalse();
    }

    public async Task<Result> ApplyTransformOnException(Func<Task<Result>> mapping)
    {
        if (_exception is not null)
            return this;

        return await mapping();
    }

    public Result ApplySideEffect(Action success)
    {
        Output(success, null);

        return this;
    }

    public Result ApplySideEffect(Action? success = null, Action<Exception>? failure = null)
    {
        Output(success, failure);

        return this;
    }

    public Result ApplySideEffect(bool test, Action? isTrue = null, Action? isFalse = null)
    {
        if (_exception is not null)
            return this;

        if (test)
            isTrue?.Invoke();
        else
            isFalse?.Invoke();

        return this;
    }

    public void Output(Action? success = null, Action<Exception>? failure = null)
    {
        if (_exception is null)
            success?.Invoke();
        else
            failure?.Invoke(_exception);
    }

    public ValueTask<Result> CompletedValueTask
        => ValueTask.FromResult(this);

    public Task<Result> CompletedTask
        => Task.FromResult(this);
}
