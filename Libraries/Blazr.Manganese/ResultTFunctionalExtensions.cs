/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese.FunctionalExtensions;

public static class ResultTFunctionalExtensions
{
    public static Result<T> ApplyTransform<T>(this Result<T> result, Func<T, Result<T>> transform)
        => result.HasValue
            ? transform(result._value!)
            : Result<T>.Failure(result._exception!);
    
    public static Result<TOut> ApplyTransform<TOut, T>(this Result<T> result, Func<T, Result<TOut>> transform)
        => result.HasValue
            ? transform(result._value!)
            : Result<TOut>.Failure(result._exception!);

    public static Result<T> ApplyTransformOnException<T>(this Result<T> result, Func<Exception, Result<T>> transform)
        => result.HasException
            ? transform(result._exception!)
            : result;

    public static Result<TOut> ApplyTransform<TOut, T>(this Result<T> result, Func<T, TOut> transform)
    {
        if (result._exception is not null)
            return Result<TOut>.Failure(result._exception!);

        try
        {
            var value = transform.Invoke(result._value!);
            return (value is null)
                ? Result<TOut>.Failure(new ResultException("The mapping function returned a null value."))
                : Result<TOut>.Create(value);
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure(ex);
        }
    }

    public static Result ApplyTransform<T>(this Result<T> result, Func<T, Result> transform)
        => result.HasValue
            ? transform(result._value!)
            : Result.Failure(result._exception!);

    public static Result<T> ApplySideEffect<T>(this Result<T> result, Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (result.HasValue)
            hasValue?.Invoke(result._value!);
        else
            hasException?.Invoke(result._exception!);

        return result;
    }

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
            ? result.ApplyTransform<TOut, T>(trueTransform)
            : result.ApplyTransform<TOut, T>(falseTransform);

    public static Result<T> ApplyExceptionOnTrue<T>(this Result<T> result, bool test, string message)
        => result.HasValue && test
            ? Result<T>.Failure(message)
            : result;

    public static Result<T> ApplyExceptionOnTrue<T>(this Result<T> result, bool test, Exception exception)
        => result.HasValue && test
            ? Result<T>.Failure(exception)
            : result;
}
