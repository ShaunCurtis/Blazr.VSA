/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class NullableFunctionalExtensions
{
    public static Nullable<T> ApplySideEffect<T>(this Nullable<T> value, Action<T>? IsNotNull = null, Action? IsNull = null)
        where T : struct
    {
        if (value.HasValue && IsNotNull != null)
            IsNotNull(value.Value);

        if (!value.HasValue && IsNull != null)
            IsNull();

        return value;
    }
}
