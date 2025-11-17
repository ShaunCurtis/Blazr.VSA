/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolTMonadExtensions
{
    public static Bool<T> Output<T>(this Bool<T> boolMonad, Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (boolMonad.HasValue)
            hasValue?.Invoke(boolMonad.Value!);
        else
            hasException?.Invoke(boolMonad.Exception!);

        return boolMonad;
    }

    public static T Output<T>(this Bool<T> boolMonad, Func<Exception, T> hasException)
        => boolMonad.HasException
            ? hasException.Invoke(boolMonad.Exception!)
            : boolMonad.Value!;

    public static T Output<T>(this Bool<T> boolMonad, Func<T> exceptionValue)
        => boolMonad.HasException
            ? exceptionValue.Invoke()
            : boolMonad.Value!;

    public static T Output<T>(this Bool<T> boolMonad, T exceptionValue)
        => boolMonad.HasException
            ? exceptionValue
            : boolMonad.Value!;

    public static TOut Output<T, TOut>(this Bool<T> boolMonad, Func<T, TOut> hasValue, Func<Exception, TOut> hasException)
        => boolMonad.HasValue
            ? hasValue.Invoke(boolMonad.Value!)
            : hasException.Invoke(boolMonad.Exception!);

    public static Bool<T> Output<T>(this Bool<T> boolMonad, Action<Bool> Action)
    {
        Action.Invoke(boolMonad.ToBool());
        return boolMonad;
    }

    public static Bool<TOut> ToBool<T, TOut>(this Bool<T> boolMonad, TOut? value)
        => boolMonad.HasException
            ? Bool<TOut>.Failure(boolMonad.Exception!)
            : Bool<TOut>.Create(value);

    public static Bool ToBool<T>(this Bool<T> boolMonad) 
        => boolMonad.Exception is null
            ? Bool.Success()
            : Bool.Failure(boolMonad.Exception);

}
