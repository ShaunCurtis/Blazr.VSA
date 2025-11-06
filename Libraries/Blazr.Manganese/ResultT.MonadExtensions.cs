/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ResultTMonadExtensions
{
    public static Result ToResult<T>(this Result<T> result) => result.Exception is null
            ? Result.Success()
            : Result.Failure(result.Exception);

    public static Result<T> Output<T>(this Result<T> result, Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (result.HasValue)
            hasValue?.Invoke(result.Value!);
        else
            hasException?.Invoke(result.Exception!);

        return result;
    }

    public static T OutputValue<T>(this Result<T> result, Func<Exception, T> hasException)
        => result.HasException
            ? hasException.Invoke(result.Exception!)
            : result.Value!;

    public static T OutputValue<T>(this Result<T> result, Func<T> exceptionValue)
        => result.HasException
            ? exceptionValue.Invoke()
            : result.Value!;

    public static T OutputValue<T>(this Result<T> result, T exceptionValue)
        => result.HasException
            ? exceptionValue
            : result.Value!;

    public static TOut OutputValue<T, TOut>(this Result<T> result, Func<T, TOut> hasValue, Func<Exception, TOut> hasException)
        => result.HasValue
            ? hasValue.Invoke(result.Value!)
            : hasException.Invoke(result.Exception!);

    public static Result<T> ExecuteTransaction<T>(this Result<T> result, Func<T, Result<T>> function)
        => result.Bind(function);

    public static Result<TOut> ExecuteTransform<T, TOut>(this Result<T> result, Func<T, Result<TOut>> function)
        => result.Bind<T, TOut>(function);

    public static Result ExecuteTransform<T>(this Result<T> result, Func<T, Result> function)
        => result.HasValue
            ? function(result.Value!)
            : Result.Failure(result.Exception!);

    public static Result<TOut> ExecuteFunction<T, TOut>(this Result<T> result, Func<T, TOut> function)
        => result.Map(function);

    public static Result<T> ExecuteSideEffect<T>(this Result<T> result, Action<T>? hasValue = null, Action<Exception>? hasException = null)
        => result.Match(hasValue, hasException);

    public static Result<T> ExecuteSideEffect<T>(this Result<T> result, Action<Result> Action)
    {
        Action.Invoke(result.ToResult());
        return result;
    }

    public static Result<TOut> ToResult<T, TOut>(this Result<T> result, TOut? value)
        => result.HasException
            ? Result<TOut>.Failure(result.Exception!)
            : Result<TOut>.Create(value);
}
