/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App;

public static class DomainSpecificLanguageExtensions
{
    public static Bool RollbackOnFailure(this Bool result, Action rollback)
        => result.Write(hasException: (ex) => rollback.Invoke());

    public static Bool<T> RollbackOnFailure<T>(this Bool<T> result, Action rollback)
    => result.Write(hasException: (ex) => rollback.Invoke());

    public static Bool<T> NotifyOnSuccess<T>(this Bool<T> result, Action<T> notify)
        => result.Write(hasValue: (value) => notify.Invoke(value));
}
