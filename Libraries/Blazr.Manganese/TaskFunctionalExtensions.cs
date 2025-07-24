/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Manganese.FunctionalExtensions;
namespace Blazr.Manganese;

public static class TaskFunctionalExtensions
{
    public static async Task<Result<TOut>> TransformAsync<TOut, T>(this Task<Result<T>> task, Func<T, Task<Result<TOut>>> transform)
    {
        var result = await task.HandleTaskCompletionAsync();

        return await result.ApplyTransformAsync<TOut, T>(transform);
    }

    public static async Task<Result> TransformAsync<T>(this Task<Result<T>> task, Func<T, Task<Result>> transform)
    {
        var result = await task.HandleTaskCompletionAsync();

        return await result.ApplyTransformAsync(transform);
    }

    public static async Task OutputAsync<T>(this Task<Result<T>> task, Action<T>? hasValue = null, Action<Exception>? hasException = null)
        => await task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.Output(hasValue: hasValue, hasException: hasException));

    public static async Task OutputAsync<T>(this Task<Result<T>> task, Action<T> hasValue)
        => await task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.Output(hasValue: hasValue));

    public static async Task<Result<TOut>> ApplyTransformAsync<TOut, T>(this Task<Result<T>> task, Func<T, Result<TOut>> transform)
    {
        var result = await task.HandleTaskCompletionAsync();
        return result.ApplyTransform<TOut, T>(transform);
    }

    public static async Task<Result<T>> ApplyTransformAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result<T>>> trueTransform, Func<T, Task<Result<T>>> falseTransform)
    {
        var result = await task.HandleTaskCompletionAsync();
        return await result.ApplyTransformAsync<T, T>(test, trueTransform, falseTransform);
    }

    public static async Task<Result<T>> ApplyTransformAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result<T>>> trueTransform)
    {
        var result = await task.HandleTaskCompletionAsync();
        return await result.ApplyTransformAsync(test, trueTransform);
    }

    public static async Task<Result> ApplyTransformAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result>> trueTransform)
    {
        var result = await task.HandleTaskCompletionAsync();
        return await result.ApplyTransformAsync(test, trueTransform);
    }

    public async static Task<Result> ApplyTransformAsync<T>(this Task<Result<T>> task, Func<T, Task<Result>> transform)
    {
        var result = await task.HandleTaskCompletionAsync();
        return await result.ApplyTransformAsync(transform);
    }

    public static Task<Result> ApplyTransformAsync<T>(this Task<Result<T>> task, Func<T, Result> transform)
        => task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.ApplyTransform(transform));

    public static Task<Result> MapTaskToResultAsync<T>(this Task<Result<T>> task)
        => task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.AsResult);

    public static Task<Result<T>> ApplySideEffectAsync<T>(this Task<Result<T>> task, Action<T>? hasValue = null, Action<Exception>? hasException = null)
        => task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.ApplySideEffect(hasValue, hasException));

    public static Task<Result<T>> ApplySideEffectAsync<T>(this Task<Result<T>> task, bool test, Action<T> trueAction)
        => task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.ApplySideEffect(test, trueAction));

    public static Task<Result> ApplySideEffectAsync(this Task<Result> task, Action? hasValue = null, Action<Exception>? hasException = null)
        => task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.ApplySideEffect(hasValue, hasException));

    private static Task<Result<T>> HandleTaskCompletionAsync<T>(this Task<Result<T>> task)
    {
        // Function to check for task completion and wrap any exceptions into the Result
        Func<Task<Result<T>>, Result<T>> CheckForTaskException = (t)
            => t.IsCompletedSuccessfully
                ? t.Result
                : Result<T>.Failure(t.Exception
                    ?? new Exception("The Task failed to complete successfully"));

        return task
            .ContinueWith(CheckForTaskException);
    }

    private static Task<Result> HandleTaskCompletionAsync(this Task<Result> task)
    {
        // Function to check for task completion and wrap any exceptions into the Result
        Func<Task<Result>, Result> CheckForTaskException = (t)
            => t.IsCompletedSuccessfully
                ? t.Result
                : Result.Failure(t.Exception
                    ?? new Exception("The Task failed to complete successfully"));

        return task
            .ContinueWith(CheckForTaskException);
    }
}
