/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolTAsyncExtensions
{
    public static async Task<Bool<TOut>> BindAsync<T, TOut>(this Bool<T> boolMonad, Func<T, Task<Bool<TOut>>> function)
        => boolMonad.HasException
            ? Bool<TOut>.Failure(boolMonad.Exception!)
            : await function(boolMonad.Value!);

    public static async Task<Bool<T>> MapAsync<T>(this Bool<T> boolMonad, Func<T, Task<Bool<T>>> function)
        => boolMonad.HasException
            ? Bool<T>.Failure(boolMonad.Exception!)
            : await function(boolMonad.Value!);

    public static Task<Bool<TOut>> ExecuteTransformAsync<T, TOut>(this Bool<T> boolMonad, Func<T, Task<Bool<TOut>>> function)
        => boolMonad.BindAsync<T,TOut>(function);

    public static Task<Bool<T>> ExecuteTransactionAsync<T>(this Bool<T> boolMonad, Func<T, Task<Bool<T>>> function)
        => boolMonad.MapAsync(function);

    public static async Task<Bool> ExecuteTransformToBoolAsync<T>(this Bool<T> boolMonad, Func<T, Task<Bool>> function)
        => boolMonad.HasException
            ? Bool.Failure(boolMonad.Exception!)
            : await function(boolMonad.Value!);

    public static Bool<TOut> CreateFromFunction<T, TOut>(this Bool<T> boolMonad, Func<TOut> function)
        => Bool.Success().Map(function);

    public async static Task<T> OutputAsync<T>(this Task<Bool<T>> task, Func<Exception, T> ExceptionOutput)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
        .Output(hasException: ExceptionOutput);
    }

    public async static Task<T> OutputAsync<T>(this Task<Bool<T>> task, Func<T> ExceptionOutput)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
        .Output(ExceptionOutput);
    }

    public async static Task<T> OutputAsync<T>(this Task<Bool<T>> task, T ExceptionOutput)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
        .Output(ExceptionOutput);
    }

    public static async Task<Bool<TOut>> BindAsync<T, TOut>(this Task<Bool<T>> task, Func<T, Bool<TOut>> function)
    {
        var boolMonad = await task.ContinueWith(CheckForTaskException);
        return boolMonad.Bind<T, TOut>(function);
    }

    public static async Task<Bool<TOut>> MapAsync<T, TOut>(this Task<Bool<T>> task, Func<T, TOut> function)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
            .Map<T, TOut>(function);
    }

    public async static Task<Bool> ToBoolAsync<T>(this Task<Bool<T>> task)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
            .ToBool();
    }
    public async static Task<Bool<TOut>> ToBoolAsync<T, TOut>(this Task<Bool<T>> task, TOut value)
    {
        var boolMonad = await task;
        return boolMonad.Bind((value) => CheckForTaskException(task))
            .ToBool<T, TOut>(value);
    }

    public static async Task<Bool> OutputAsync(this Task<Bool> task, Action? hasValue = null, Action<Exception>? hasException = null)
        => (await task)
            .Bind(() => CheckForTaskException(task))
            .Output(hasValue, hasException);

    public async static Task<Bool<T>> OutputAsync<T>(this Task<Bool<T>> task, Action<Bool> action)
         => (await task)
            .Bind(value => CheckForTaskException(task))
            .Output(action);

    private static Bool<T> CheckForTaskException<T>(Task<Bool<T>> task)
        => task.IsCompletedSuccessfully
            ? task.Result
            : Bool<T>.Failure(task.Exception
                ?? new Exception("The Task failed to complete successfully"));

    private static Bool CheckForTaskException(Task<Bool> task)
        => task.IsCompletedSuccessfully
            ? task.Result
                : Bool.Failure(task.Exception
                ?? new Exception("The Task failed to complete successfully"));

}