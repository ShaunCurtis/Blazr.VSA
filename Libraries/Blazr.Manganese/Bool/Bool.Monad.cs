/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolMonad
{
    public static Bool Bind(this Bool boolMonad, Func<Bool> function)
        => boolMonad.Succeeded
            ? function()
            : Bool.Failure(boolMonad.Exception!);

    public static T Match<T>(this Bool boolMonad, Func<T> Succeeded, Func<Exception, T> Failed)
        => boolMonad.Succeeded
        ? Succeeded.Invoke()
        : Failed.Invoke(boolMonad.Exception!);
}

