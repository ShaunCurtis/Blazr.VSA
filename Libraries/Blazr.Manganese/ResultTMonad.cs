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
    private bool _hasException => _exception is not null;

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

    public Result<TOut> ApplyTransform<TOut>(Func<T, Result<TOut>> transform)
        => HasValue
            ? transform(_value!)
            : Result<TOut>.Failure(_exception!);

    public Result<T> ApplyTransformOnException(Func<Exception, Result<T>> transform)
        => _hasException
            ? transform(_exception!)
            : this;

    public Result<TOut> ApplyTransform<TOut>(Func<T, TOut> transform)
    {
        if (_exception is not null)
            return Result<TOut>.Failure(_exception!);

        try
        {
            var value = transform.Invoke(_value!);
            return (value is null)
                ? Result<TOut>.Failure(new ResultException("The mapping function returned a null value."))
                : Result<TOut>.Create(value);
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure(ex);
        }
    }

    public Result ApplyTransform(Func<T, Result> transform)
        => HasValue
            ? transform(_value!)
            : Result.Failure(_exception!);

    public Result<T> ApplySideEffect(Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (HasValue)
            hasValue?.Invoke(_value!);
        else
            hasException?.Invoke(_exception!);

        return this;
    }

    public void OutputResult(Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (HasValue)
            hasValue?.Invoke(_value!);
        else
            hasException?.Invoke(_exception!);
    }

    public async Task<Result<TOut>> ApplyTransformAsync<TOut>(Func<T, Task<Result<TOut>>> transform)
        => _hasException
            ? Result<TOut>.Failure(_exception!)
            : await transform(_value!);

    public async Task<Result> ApplyTransformAsync(Func<T, Task<Result>> transform)
        => _hasException
            ? Result.Failure(_exception!)
            : await transform(_value!);

    public ValueTask<Result<T>> CompletedValueTask
        => ValueTask.FromResult(this);

    public Task<Result<T>> CompletedTask
        => Task.FromResult(this);
}

public static class ResultTExtensions
{
    public static Result<T> ApplySideEffect<T>(this Result<T> result, bool test, Action<T> isTrue, Action<T> isFalse)
        => test
            ? result.ApplySideEffect(isTrue, null)
            : result.ApplySideEffect(isFalse, null);

    public static Result<T> ApplySideEffect<T>(this Result<T> result, bool test, Action<T> isTrue)
        => test
            ? result.ApplySideEffect(isTrue, null)
            : result;

    public static Result<T> ApplyTransform<T>(this Result<T> result, bool test, Func<T, Result<T>> trueTransform, Func<T, Result<T>> falseTransform)
        => test
            ? result.ApplyTransform<T>(trueTransform)
            : result.ApplyTransform<T>(falseTransform);

    public static Result<TOut> ApplyTransform<TOut, T>(this Result<T> result, bool test, Func<T, Result<TOut>> trueTransform, Func<T, Result<TOut>> falseTransform)
        => test
            ? result.ApplyTransform<TOut>(trueTransform)
            : result.ApplyTransform<TOut>(falseTransform);

    public static Result<T> ApplyExceptionOnTrue<T>(this Result<T> result, bool test, string message)
        => result.HasValue && test
            ? Result<T>.Failure(message)
            : result;

    public static Result<T> ApplyExceptionOnTrue<T>(this Result<T> result, bool test, Exception exception)
        => result.HasValue && test
            ? Result<T>.Failure(exception)
            : result;
}

public static class ResultTaskTExtensions
{
    public async static Task<Result<TOut>> ApplyTransformAsync<TOut, T>(this Result<T> result, bool test, Func<T, Task<Result<TOut>>> trueTransform, Func<T, Task<Result<TOut>>> falseTransform)
        => test
            ? await result.ApplyTransformAsync<TOut>(trueTransform)
            : await result.ApplyTransformAsync<TOut>(falseTransform);

    public async static Task<Result<T>> ApplyTransformAsync<T>(this Result<T> result, bool test, Func<T, Task<Result<T>>> trueTransform)
        => test
            ? await result.ApplyTransformAsync<T>(trueTransform)
            : result;

    public async static Task<Result> ApplyTransformAsync<T>(this Result<T> result, bool test, Func<T, Task<Result>> trueTransform, Func<T, Task<Result>> falseTransform)
        => test
            ? await result.ApplyTransformAsync(trueTransform)
            : await result.ApplyTransformAsync(falseTransform);

    public async static Task<Result> ApplyTransformAsync<T>(this Result<T> result, bool test, Func<T, Task<Result>> trueTransform)
        => test
            ? await result.ApplyTransformAsync(trueTransform)
            : result.AsResult;
}
