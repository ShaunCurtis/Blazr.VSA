/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ResultTAsyncExtensions
{
    public static async Task<Result<TOut>> BindAsync<T, TOut>(this Result<T> result, Func<T, Task<Result<TOut>>> function)
        => result.HasException
            ? Result<TOut>.Failure(result.Exception!)
            : await function(result.Value!);

    public static async Task<Result<T>> MapAsync<T>(this Result<T> result, Func<T, Task<Result<T>>> function)
        => result.HasException
            ? Result<T>.Failure(result.Exception!)
            : await function(result.Value!);

    public static Task<Result<TOut>> ExecuteTransformAsync<T, TOut>(this Result<T> result, Func<T, Task<Result<TOut>>> function)
        => result.BindAsync<T,TOut>(function);

    public static Task<Result<T>> ExecuteTransactionAsync<T>(this Result<T> result, Func<T, Task<Result<T>>> function)
        => result.MapAsync(function);

    public static async Task<Result> ExecuteTransformToResultAsync<T>(this Result<T> result, Func<T, Task<Result>> function)
        => result.HasException
            ? Result.Failure(result.Exception!)
            : await function(result.Value!);

    public static Result<TOut> CreateFromFunction<T, TOut>(this Result<T> result, Func<TOut> function)
        => Result.Success().ExecuteFunction(function);
}