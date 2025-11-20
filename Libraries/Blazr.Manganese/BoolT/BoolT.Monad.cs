/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolTMonad
{
    public static Bool<TOut> Bind<T, TOut>(this Bool<T> boolMonad, Func<T, Bool<TOut>> function)
        => boolMonad.HasValue
            ? function(boolMonad.Value!)
            : Bool<TOut>.Failure(boolMonad.Exception!);

    public static Bool<TOut> Map<T, TOut>(this Bool<T> boolMonad, Func<T, TOut> function)
    {
        if (boolMonad.Exception is not null)
            return Bool<TOut>.Failure(boolMonad.Exception!);

        try
        {
            var value = function.Invoke(boolMonad.Value!);
            return (value is null)
                ? Bool<TOut>.Failure(new BoolException("The function returned a null value."))
                : Bool<TOut>.Input(value);
        }
        catch (Exception ex)
        {
            return Bool<TOut>.Failure(ex);
        }
    }

    public static Bool<T> Map<T>(this Bool<T> boolMonad, Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (boolMonad.HasValue)
            hasValue?.Invoke(boolMonad.Value!);
        else
            hasException?.Invoke(boolMonad.Exception!);

        return boolMonad;
    }

    public static IOMonad<TOut> Match<T, TOut>(this Bool<T> boolMonad, Func<TOut> hasNoValue, Func<T, TOut>? hasValue = null, Func<Exception, TOut>? hasException = null)
        => (Tuple.Create(boolMonad.HasValue, boolMonad.HasException)) switch
        {
            (true, false) => IOMonad.Input(hasValue is not null ? hasValue.Invoke(boolMonad.Value!) : default!),
            (false, true) => IOMonad.Input(hasException is not null ? hasException.Invoke(boolMonad.Exception!) : default!),
            _ => IOMonad.Input(hasNoValue.Invoke()),
        };
}
