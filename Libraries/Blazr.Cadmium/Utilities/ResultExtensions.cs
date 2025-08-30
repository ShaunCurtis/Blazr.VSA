/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Cadmium;

public static class ResultExtensions
{
    public static Result<TOut> Dispatch<T, TOut>(this Result<T> result, Func<T, Result<TOut>> function)
        => result.ExecuteTransform<TOut>(function);
}