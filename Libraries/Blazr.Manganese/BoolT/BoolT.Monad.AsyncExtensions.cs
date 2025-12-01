/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ReturnTAsyncExtensions
{
    public static async Task<Return<TOut>> BindAsync<T, TOut>(this Return<T> boolMonad, Func<T, Task<Return<TOut>>> function)
        => boolMonad.HasException
            ? Return<TOut>.Failure(boolMonad.Exception!)
            : await function(boolMonad.Value!);

    public static async Task<Return<T>> MapAsync<T>(this Return<T> boolMonad, Func<T, Task<Return<T>>> function)
        => boolMonad.HasException
            ? Return<T>.Failure(boolMonad.Exception!)
            : await function(boolMonad.Value!);

    public static Task<Return<T>> BindAsync<T>(this Return<T> boolMonad, Func<T, Task<Return<T>>> function)
        => boolMonad.MapAsync(function);

    public async static Task<T> WriteAsync<T>(this Task<Return<T>> task, Func<Exception, T> ExceptionOutput)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
        .Write(hasException: ExceptionOutput);
    }

    public async static Task<T> WriteAsync<T>(this Task<Return<T>> task, Func<T> ExceptionOutput)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
        .Write(ExceptionOutput);
    }

    public async static Task<T> WriteAsync<T>(this Task<Return<T>> task, T ExceptionOutput)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
        .Write(ExceptionOutput);
    }

    public static async Task<Return<TOut>> BindAsync<T, TOut>(this Task<Return<T>> task, Func<T, Return<TOut>> function)
    {
        var boolMonad = await task.ContinueWith(CheckForTaskException);
        return boolMonad.Bind<T, TOut>(function);
    }

    public static async Task<Return<TOut>> MapAsync<T, TOut>(this Task<Return<T>> task, Func<T, TOut> function)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
            .Map<T, TOut>(function);
    }

    public static async Task<Return<TOut>> TryMapAsync<T, TOut>(this Task<Return<T>> task, Func<T, TOut> function)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
            .TryMap<T, TOut>(function);
    }

    public async static Task<Bool> ToBoolAsync<T>(this Task<Return<T>> task)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
            .ToBool();
    }
    public async static Task<Return<TOut>> ToBoolAsync<T, TOut>(this Task<Return<T>> task, TOut value)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
            .ToBoolT<T, TOut>(value);
    }

    public static async Task<Bool> WriteAsync(this Task<Bool> task, Action? hasValue = null, Action<Exception>? hasException = null)
        => (await task)
            .Bind(() => CheckForTaskException(task))
            .Write(hasValue, hasException);

    public async static Task<Return<T>> WriteAsync<T>(this Task<Return<T>> task, Action<Bool> action)
         => (await task)
            .Bind(value => CheckForTaskException(task))
            .Write(action);

    private static Return<T> CheckForTaskException<T>(Task<Return<T>> task)
        => task.IsCompletedSuccessfully
            ? task.Result
            : Return<T>.Failure(task.Exception
                ?? new Exception("The Task failed to complete successfully"));

    private static Bool CheckForTaskException(Task<Bool> task)
        => task.IsCompletedSuccessfully
            ? task.Result
                : Bool.Failure(task.Exception
                ?? new Exception("The Task failed to complete successfully"));

}