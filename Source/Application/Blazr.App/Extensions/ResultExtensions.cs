/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class ResultExtensions
{
    extension<T>(Result<T> @this)
    {
        public bool HasNotSucceeded
            => @this is FailureResult<T>;

        public bool HasSucceeded
            => @this is SuccessResult<T>;

        public Result<TOut> Convert<TOut>(TOut value)
            => @this switch
                {
                    SuccessResult<T> success => ResultT.Read(value),
                    FailureResult<T> failure => ResultT.Read<TOut>(failure.Exception),
                    _ => throw new InvalidOperationException("Unknown result type")
                };
    }
}
