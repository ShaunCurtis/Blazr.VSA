/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result<T>
{
    public static Result<T> Return(T? value) =>
        value is null
            ? new(new ResultException("T was null."))
            : new(value);
}

public static class ResultTMonad
{
    public static Result<TOut> Bind<T,TOut>(this Result<T> result,Func<T, Result<TOut>> function)
        => result.HasValue
            ? function(result.Value!)
            : Result<TOut>.Failure(result.Exception!);

    public static Result<TOut> Map<T,TOut>(this Result<T> result, Func<T, TOut> function)
    {
        if (result.Exception is not null)
            return Result<TOut>.Failure(result.Exception!);

        try
        {
            var value = function.Invoke(result.Value!);
            return (value is null)
                ? Result<TOut>.Failure(new ResultException("The function returned a null value."))
                : Result<TOut>.Create(value);
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure(ex);
        }
    }

    public static Result<T> Map<T>( this Result<T> result, Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (result.HasValue)
            hasValue?.Invoke(result.Value!);
        else
            hasException?.Invoke(result.Exception!);

        return result;
    }

    public static Result<T> Match<T>(this Result<T> result, Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (result.HasValue)
            hasValue?.Invoke(result.Value!);
        else
            hasException?.Invoke(result.Exception!);

        return result;
    }
}
