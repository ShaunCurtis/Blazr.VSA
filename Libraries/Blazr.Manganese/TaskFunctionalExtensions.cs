/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class TaskFunctionalExtensions
{
    public static async Task<Result<TOut>> ApplyTransformAsync<TOut, T>(this Task<Result<T>> task, Func<T, Task<Result<TOut>>> transform)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ApplyTransformAsync<TOut>(transform);
    }

    public static Task<Result<TOut>> ApplyTransformAsync<T, TOut>(this Task<Result<T>> task, Func<T, TOut> transform)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.ApplyTransform<TOut>(transform));

    public static Task OutputAsync<T>(this Task<Result<T>> task, Action<T>? hasValue = null, Action<Exception>? hasException = null)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.Output(hasValue: hasValue, hasException: hasException));

    public static Task OutputAsync<T>(this Task<Result<T>> task, Action<T> hasValue)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.Output(hasValue: hasValue));

    public static async Task<Result<T>> ApplyTransformAsync<T>(this Task<Result<T>> task, Func<T, Result<T>> transform)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return result.ApplyTransform<T>(transform);
    }

    public static async Task<Result<TOut>> ApplyTransformAsync<TOut, T>(this Task<Result<T>> task, Func<T, Result<TOut>> transform)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return result.ApplyTransform<TOut>(transform);
    }

    public static async Task<Result<T>> ApplyTransformAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result<T>>> trueTransform, Func<T, Task<Result<T>>> falseTransform)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ApplyTransformAsync<T>(test, trueTransform, falseTransform);
    }

    public static async Task<Result<T>> ApplyTransformAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result<T>>> trueTransform)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ApplyTransformAsync(test, trueTransform);
    }

    public static async Task<Result> ApplyTransformAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result>> trueTransform)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ApplyTransformAsync(test, trueTransform);
    }

    public async static Task<Result> ApplyTransformAsync<T>(this Task<Result<T>> task, Func<T, Task<Result>> transform)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ApplyTransformAsync(transform);
    }

    public static Task<Result> ApplyTransformAsync<T>(this Task<Result<T>> task, Func<T, Result> transform)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.ApplyTransform(transform));

    public static Task<Result> ToResultAsync<T>(this Task<Result<T>> task)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.ToResult);

    public static Task<Result<T>> ApplySideEffectAsync<T>(this Task<Result<T>> task, Action<T>? hasValue = null, Action<Exception>? hasException = null)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.ApplySideEffect(hasValue, hasException));

    public static Task<Result<T>> ApplySideEffectAsync<T>(this Task<Result<T>> task, bool test, Action<T> trueAction)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.ApplySideEffect(test, trueAction));

    public static Task<Result> ApplySideEffectAsync(this Task<Result> task, Action? hasValue = null, Action<Exception>? hasException = null)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.ApplySideEffect(hasValue, hasException));

    public static Task<Result> AsResultAsync<T>(this Task<Result<T>> task)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.ToResult);

    private static Result<T> CheckForTaskException<T>(Task<Result<T>> task)
        => task.IsCompletedSuccessfully
            ? task.Result
            : Result<T>.Failure(task.Exception
                ?? new Exception("The Task failed to complete successfully"));

    private static Result CheckForTaskException(Task<Result> task)
        => task.IsCompletedSuccessfully
            ? task.Result
            : Result.Failure(task.Exception
                ?? new Exception("The Task failed to complete successfully"));
}
