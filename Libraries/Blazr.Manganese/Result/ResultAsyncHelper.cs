/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class AsyncHelpers
{
    public static Result<TOut> CheckForTaskException<TOut>(Task<Result<TOut>> @this)
        => @this.IsCompletedSuccessfully
            ? @this.Result
            : @this.Exception is null
                ? Result<TOut>.Failure("The Task did not complete successfully")
                : Result<TOut>.Exception(@this.Exception);

    public static Result<TOut> CheckForTaskException<TOut>(Task<TOut> @this)
        => @this.IsCompletedSuccessfully
            ? Result<TOut>.Successful(@this.Result)
            : @this.Exception is null
                ? Result<TOut>.Failure("The Task did not complete successfully")
                : Result<TOut>.Exception(@this.Exception);
}
