/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ResultMonadAsyncExtensions
{
    public static async Task<Result> ExecuteFunctionAsync(this Result result,  Func<Task<Result>> function)
        => result.Failed
            ? result
            : await function();

    public static async Task<Result<T>> ExecuteFunctionAsync<T>(this Result result, Func<Task<Result<T>>> function)
        => result.Failed
            ? Result<T>.Failure(result.Exception!)
            : await function();
}

