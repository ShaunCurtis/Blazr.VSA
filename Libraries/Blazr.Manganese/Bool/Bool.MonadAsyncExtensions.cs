/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolMonadAsyncExtensions
{
    public static async Task<Bool> BindAsync(this Bool result,  Func<Task<Bool>> function)
        => result.Failed
            ? result
            : await function();

    public static async Task<Result<T>> BindAsync<T>(this Bool result, Func<Task<Result<T>>> function)
        => result.Failed
            ? Result<T>.Failure(result.Exception!)
            : await function();
}

