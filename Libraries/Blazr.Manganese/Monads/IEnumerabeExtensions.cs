/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class IEnumerableFunctionalExtensions
{
    public static Bool<IEnumerable<T>> ToBoolMonad<T>(this IEnumerable<T> value)
        => BoolT.Success(value);
}
