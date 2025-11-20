/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolMonadAsyncExtensions
{
    public static async Task<Bool> BindAsync(this Bool boolMonad,  Func<Task<Bool>> function)
        => boolMonad.Failed
            ? boolMonad
            : await function();

    public static async Task<Bool<T>> BindAsync<T>(this Bool boolMonad, Func<Task<Bool<T>>> function)
        => boolMonad.Failed
            ? Bool<T>.Failure(boolMonad.Exception!)
            : await function();
}

