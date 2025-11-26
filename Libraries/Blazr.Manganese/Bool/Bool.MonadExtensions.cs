/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolMonadExtensions
{
    public static Bool Write(this Bool boolMonad, Action? hasNoException = null, Action<Exception>? hasException = null)
    {
        if (boolMonad.Failed)
            hasException?.Invoke(boolMonad.Exception!);
        else
            hasNoException?.Invoke();

        return boolMonad;
    }

    public static bool Write(this Bool boolMonad)
        => boolMonad.Succeeded;
}
