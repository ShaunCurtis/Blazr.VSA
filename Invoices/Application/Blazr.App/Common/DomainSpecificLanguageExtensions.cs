/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App;

public static class DomainSpecificLanguageExtensions
{
    public static Result RollbackOnFailure(this Result result, Action rollback)
        => result.ExecuteSideEffect(hasException: (ex) => rollback.Invoke());

    public static Result<T> RollbackOnFailure<T>(this Result<T> result, Action rollback)
    => result.ExecuteSideEffect(hasException: (ex) => rollback.Invoke());

    public static Result<T> NotifyOnSuccess<T>(this Result<T> result, Action<T> notify)
        => result.ExecuteSideEffect(hasValue: (value) => notify.Invoke(value));
}
