/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ResultAsyncExtensions
{
    public static async Task<Result<TOut>> MapAsync<T, TOut>(this Task<Result<T>> @this, Func<T, TOut> map)
        => await Task<Result<TOut>>.FromResult((await @this.ContinueWith(AsyncHelpers.CheckForTaskException))
        .Map(map));

    public static async Task<Result<TOut>> MapAsync<T, TOut>(this Task<Result<T>> @this, Func<T, Task<TOut>> map)
        => await @this.ContinueWith(AsyncHelpers.CheckForTaskException)
        .MapAsync(map);

    public static async Task<Result<TOut>> BindAsync<T, TOut>(this Task<Result<T>> @this, Func<T, Result<TOut>> bind)
        => await Task<Result<TOut>>.FromResult((await @this.ContinueWith(AsyncHelpers.CheckForTaskException))
        .Bind(bind));

    public static async Task<Result<TOut>> BindAsync<T, TOut>(this Task<Result<T>> @this, Func<T, Task<Result<TOut>>> bind)
        =>  await @this.ContinueWith(AsyncHelpers.CheckForTaskException)
            .BindAsync(bind);

    public static async Task<Result<T>> WriteAsync<T>(this Task<Result<T>> @this, Action<T> writer, T defaultValue)
        => (await @this.ContinueWith(AsyncHelpers.CheckForTaskException))
        .Write(writer, defaultValue);

    public static async Task<T> WriteAsync<T>(this Task<Result<T>> @this, T defaultValue)
        => (await @this.ContinueWith(AsyncHelpers.CheckForTaskException))
        .Write(defaultValue);
}