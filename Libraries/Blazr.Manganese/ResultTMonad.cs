/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result<T>
{
    private readonly Exception? _exception;
    private readonly T? _value;
    private ResultException _defaultException => new ResultException("An error occurred. No specific exception provided.");

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

    public static Result<T> NoValueFailure() => new(new ResultException("No value was returned"));

    public static Result<T> Failure(string message) => new(new ResultException(message));

    public Result<T> ExecuteSideEffect(bool test, Action<T> isTrue, Action<T> isFalse)
    {
        if (_exception is not null)
            return this;

        if (test)
            isTrue(_value!);
        else
            isFalse(_value!);

        return this;
    }

    public Result<T> ExecuteSideEffect(bool test, Action<T> isTrue)
    {
        if (_exception is not null)
            return this;

        if (test)
            isTrue(_value!);

        return this;
    }

    public Result<T> ExecuteSideEffect(Action<T>? success = null, Action<Exception>? failure = null)
    {
        if (_exception is null)
            success?.Invoke(_value!);
        else
            failure?.Invoke(_exception!);

        return this;
    }

    public Result<TOut> MapToResult<TOut>(Func<T, Result<TOut>> success)
    {
        if (_exception is null)
            return success(_value!);

        return Result<TOut>.Failure(_exception!);
    }

    public Result<TOut> MapToResult<TOut>(bool test, Func<T, Result<TOut>> isTrue, Func<T, Result<TOut>> isFalse)
    {
        if (_exception is not null)
            return Result<TOut>.Failure(_exception!);

        return test ? isTrue(_value!) : isFalse(_value!);
    }

    public Result<TOut> MapToResult<TOut>(Func<T, TOut> mapping)
    {
        if (_exception is not null)
            return Result<TOut>.Failure(_exception!);

        try
        {
            var result = mapping.Invoke(_value!);
            if (result is null)
                return Result<TOut>.Failure(new ResultException("The mapping function returned a null value."));

            return Result<TOut>.Create(mapping(_value!));
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure(ex);
        }
    }

    public Result MapToResult(Func<T, Result>? mapping = null)
    {
        if (_exception is null && mapping != null)
            return mapping(_value!);

        if (_value is not null)
            return Result.Success();

        return Result.Failure(_exception ?? _defaultException);
    }

    public async Task<Result> MapToResultAsync(Func<T, Task<Result>> success, Func<Exception, Task<Result>>? failure = null)
    {
        if (_exception is null && success != null)
            return await success(_value!);

        if (_exception is not null && failure != null)
            return await failure(_exception!);

        return Result.Failure(_exception ?? _defaultException);
    }

    public async Task<Result<TOut>> MapToResultAsync<TOut>(Func<T, Task<Result<TOut>>> success)
    {
        if (_exception is null)
            return await success(_value!);

        return Result<TOut>.Failure(_exception ?? _defaultException);
    }

    public async Task<Result<TOut>> MapToResultAsync<TOut>(bool test, Func<T, Task<Result<TOut>>> isTrue, Func<T, Task<Result<TOut>>> isFalse)
    {
        if (_exception is not null)
            return Result<TOut>.Failure(_exception!);

        return test ? await isTrue(_value!) : await isFalse(_value!);
    }

    public async Task<Result> MapToResultAsync(bool test, Func<T, Task<Result>> isTrue)
    {
        if (_exception is not null)
            return Result.Failure(_exception!);
        if (test)
            return await isTrue(_value!);

        return Result.Success();
    }

    public async Task<Result<T>> MapToResultAsync(bool test, Func<T, Task<Result<T>>> isTrue)
    {
        if (_exception is not null)
            return Result<T>.Failure(_exception!);
        if (test)
            return await isTrue(_value!);

        return Result<T>.Success(_value!);
    }

    public async Task<Result> MapToResultAsync(bool test, Func<T, Task<Result>> isTrue, Func<T, Task<Result>> isFalse)
    {
        if (_exception is not null)
            return Result.Failure(_exception!);

        return test ? await isTrue(_value!) : await isFalse(_value!);
    }

    public async Task<Result<T>> MapToResultAsync(bool test, Func<T, Task<Result<T>>> isTrue, Func<T, Task<Result<T>>> isFalse)
    {
        if (_exception is not null)
            return Result<T>.Failure(_exception!);

        return test ? await isTrue(_value!) : await isFalse(_value!);
    }

    public async Task<Result<T>> MapExceptionAsync(Func<Exception, Task<Result<T>>> failure)
    {
        if (_exception is not null)
            return await failure(_exception!);

        return this;
    }

    public Result<T> MapToException(bool test, string message)
    {
        if (_exception is null && test)
            return Result<T>.Failure(message);

        return this;
    }

    public Result<T> MapToException(bool test, Exception exception)
    {
        if (_exception is null && test)
            return Result<T>.Failure(exception);

        return this;
    }

    public void OutputResult(Action<T>? success = null, Action<Exception>? failure = null)
    {
        if (_exception is null && success != null)
            success(_value!);

        if (_exception is not null && failure != null)
            failure(_exception!);
    }

    public ValueTask<Result<T>> CompletedValueTask
        => ValueTask.FromResult(this);

    public Task<Result<T>> CompletedTask
        => Task.FromResult(this);
}
