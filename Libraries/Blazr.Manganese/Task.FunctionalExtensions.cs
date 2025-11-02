/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class TaskFunctionalExtensions
{
    public async static Task<T> OutputValueAsync<T>(this Task<Result<T>> task, Func<Exception, T> ExceptionOutput)
    {
        var result = await task;
        return result.ExecuteTransaction((value) => CheckForTaskException(task))
        .OutputValue(hasException: ExceptionOutput);
    }

    public async static Task<T> OutputValueAsync<T>(this Task<Result<T>> task, Func<T> ExceptionOutput)
    {
        var result = await task;
        return result.ExecuteTransaction((value) => CheckForTaskException(task))
        .OutputValue(ExceptionOutput);
    }

    public async static Task<T> OutputValueAsync<T>(this Task<Result<T>> task, T ExceptionOutput)
    {
        var result = await task;
        return result.ExecuteTransaction((value) => CheckForTaskException(task))
        .OutputValue(ExceptionOutput);
    }

    public static async Task<Result<TOut>> ExecuteTransformAsync<T, TOut>(this Task<Result<T>> task, Func<T, Result<TOut>> function)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return result.ExecuteTransform<T, TOut>(function);
    }

    public static async Task<Result<T>> ExecuteTransactionAsync<T>(this Task<Result<T>> task, Func<T, Result<T>> function)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return result.ExecuteTransaction(function);
    }

    public static async Task<Result<TOut>> ExecuteFunctionAsync<T, TOut>(this Task<Result<T>> task, Func<T, TOut> function)
    {
        var result = await task;
        return result.ExecuteTransform((value) => CheckForTaskException(task))
            .ExecuteFunction<T, TOut>(function);
    }

    public async static Task<Result> ToResultAsync<T>(this Task<Result<T>> task)
    {
        var result = await task;
        return result.ExecuteTransaction((value) => CheckForTaskException(task))
            .ToResult();
    }
    public async static Task<Result<TOut>> ToResultAsync<T, TOut>(this Task<Result<T>> task, TOut value)
    {
        var result = await task;
        return result.ExecuteTransaction((value) => CheckForTaskException(task))
            .ToResult<T, TOut>(value);
    }

    public static Task<Result> ExecuteSideEffectAsync(this Task<Result> task, Action? hasValue = null, Action<Exception>? hasException = null)
        => task.ExecuteSideEffectAsync(hasValue, hasException);

    public async static Task<Result<T>> ExecuteSideEffectAsync<T>(this Task<Result<T>> task, Action<Result> action)
         => (await task)
            .ExecuteTransaction((value) => CheckForTaskException(task))
            .ExecuteSideEffect(action);

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
