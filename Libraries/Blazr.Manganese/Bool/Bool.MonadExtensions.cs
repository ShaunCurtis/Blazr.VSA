/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolMonadExtensions
{
    public static Bool Output(this Bool boolMonad, Action? hasNoException = null, Action<Exception>? hasException = null)
    {
        if (boolMonad.Failed)
            hasException?.Invoke(boolMonad.Exception!);
        else
            hasNoException?.Invoke();

        return boolMonad;
    }

    public static bool Output(this Bool boolMonad)
        => boolMonad.Succeeded;

    public static Bool<TOut> Map<TOut>(this Bool boolMonad, Func<TOut> function)
    {
        if (boolMonad.Failed)
            return Bool<TOut>.Failure(boolMonad.Exception!);

        try
        {
            var value = function.Invoke();
            return (value is null)
                ? Bool<TOut>.Failure(new BoolException("The transform function returned a null value."))
                : BoolT.Success(value);
        }
        catch (Exception ex)
        {
            return Bool<TOut>.Failure(ex);
        }
    }

    public static Bool Map(this Bool boolMonad, Func<Bool> function)
        => boolMonad.Failed
            ? boolMonad
            : function();
}
