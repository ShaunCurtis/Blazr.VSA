/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class TaskFunctionalExtensions
{
    public static async Task<Result<TOut>> MapTaskAsync<T, TOut>(this Task<Result<T>> task, Func<T, Task<Result<TOut>>> mapping)
    {
        var result = await task.HandleTaskCompletionAsync();

        return await result.MapToResultAsync<TOut>(mapping);
    }

    public static async Task<Result> MapTaskAsync<T>(this Task<Result<T>> task, Func<T, Task<Result>> mapping)
    {
        var result = await task.HandleTaskCompletionAsync();

        return await result.MapToResultAsync(mapping);
    }

    public static async Task OutputTaskAsync<T>(this Task<Result<T>> task, Action<T>? success = null, Action<Exception>? failure = null)
        => await task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.OutputResult(success: success, failure: failure));

    public static async Task OutputTaskAsync<T>(this Task<Result<T>> task, Action<T> success)
        => await task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.OutputResult(success: success));

    public static async Task<Result<T>> MapTaskToResultAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result<T>>> isTrue, Func<T, Task<Result<T>>> isFalse)
    {
        var result = await task.HandleTaskCompletionAsync();
        return await result.MapToResultAsync<T>(test, isTrue, isFalse);
    }

    public static async Task<Result<T>> MapTaskToResultAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result<T>>> isTrue)
    {
        var result = await task.HandleTaskCompletionAsync();
        return await result.MapToResultAsync(test, isTrue);
    }

    public static async Task<Result> MapTaskToResultAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result>> isTrue)
    {
        var result = await task.HandleTaskCompletionAsync();
        return await result.MapToResultAsync(test, isTrue);
    }

    public async static Task<Result> MapTaskToResultAsync<T>(this Task<Result<T>> task, Func<T, Task<Result>> mapping)
    {
        var result = await task.HandleTaskCompletionAsync();
        return await result.MapToResultAsync(mapping);
    }

    public static Task<Result> MapTaskToResultAsync<T>(this Task<Result<T>> task, Func<T, Result> mapping)
        => task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.MapToResult(mapping));

    public static Task<Result> MapTaskToResultAsync<T>(this Task<Result<T>> task)
        => task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.MapToResult());

    public static Task<Result<T>> TaskSideEffectAsync<T>(this Task<Result<T>> task, Action<T>? success = null, Action<Exception>? failure = null)
        => task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.ExecuteSideEffect(success, failure));

    public static Task<Result<T>> TaskSideEffectAsync<T>(this Task<Result<T>> task, bool test, Action<T> isTrue)
        => task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.ExecuteSideEffect(test, isTrue));

    public static Task<Result> TaskSideEffectAsync(this Task<Result> task, Action? success = null, Action<Exception>? failure = null)
        => task.HandleTaskCompletionAsync()
            .ContinueWith((t) => t.Result.SideEffect(success, failure));

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
