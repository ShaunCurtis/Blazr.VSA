/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class TaskFunctionalExtensions
{
    public async static Task<Result<T>> OutputAsync<T>(this Task<Result<T>> task, Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        var result = await task;
        return result.ExecuteTransaction((value) => CheckForTaskException(task))
            .Output(hasValue: hasValue, hasException: hasException);
    }

    public async static Task<Result> OutputAsync(this Task<Result> task, Action? hasValue = null, Action<Exception>? hasException = null)
    {
        var result = await task;
        return result.ExecuteFunction(() => CheckForTaskException(task))
         .Output(hasValue, hasException);
    }

    public async static Task<Result<T>> OutputAsync<T>(this Task<Result<T>> task, Action<T> hasValue)
    {
        var result = await task;
        return result.ExecuteTransaction((value) => CheckForTaskException(task))
       .Output(hasValue: hasValue);
    }
    public async static Task<T> OutputValueAsync<T>(this Task<Result<T>> task, Func<Exception, T> ExceptionOutput)
    {
        var result = await task;
        return result.ExecuteTransaction((value) => CheckForTaskException(task))
        .OutputValue(hasException: ExceptionOutput);
    }
    public static async Task<Result<TOut>> ExecuteTransformAsync<TOut, T>(this Task<Result<T>> task, Func<T, Task<Result<TOut>>> function)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ExecuteTransformAsync<TOut>(function);
    }

    public static async Task<Result<TOut>> ExecuteTransformAsync<TOut, T>(this Task<Result<T>> task, Func<T, Result<TOut>> function)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return result.ExecuteTransform<TOut>(function);
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
            .ExecuteFunction<TOut>(function);
    }

    public static async Task<Result<T>> ExecuteConditionalTransactionAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result<T>>> truefunction, Func<T, Task<Result<T>>> falsefunction)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ExecuteFunctionAsync<T>(test, truefunction, falsefunction);
    }

    public static async Task<Result<T>> ExecuteTransactionOnAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result<T>>> truefunction)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ExecuteFunctionAsync(test, truefunction);
    }

    public static async Task<Result> ExecuteTransformOnTrueAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result>> truefunction)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ExecuteFunctionAsync(test, truefunction);
    }

    public async static Task<Result> ExecuteTransformAsync<T>(this Task<Result<T>> task, Func<T, Task<Result>> function)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ExecuteTransformAsync(function);
    }

    public async static Task<Result> ExecuteTransformAsync<T>(this Task<Result<T>> task, Func<T, Result> function)
    {
        var result = await task;
        return result.ExecuteTransform((value) => CheckForTaskException(task))
        .ExecuteTransform(function);
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
            .ToResult<TOut>(value);
    }

    public async static Task<Result<T>> ExecuteActionAsync<T>(this Task<Result<T>> task, Action<T>? hasValue = null, Action<Exception>? hasException = null)
        => (await task)
            .ExecuteTransaction((value) => CheckForTaskException(task))
            .ExecuteAction(hasValue, hasException);

    public async static Task<Result<T>> ExecuteActionOnSuccessAsync<T>(this Task<Result<T>> task, Action<T> successAction)
        => (await task)
            .ExecuteTransaction((value) => CheckForTaskException(task))
            .ExecuteAction(successAction, null);

    public async static Task<Result> ExecuteActionOnSuccessAsync(this Task<Result> task, Action successAction)
        => (await task)
            .ExecuteFunction(() => CheckForTaskException(task))
            .ExecuteAction(successAction, null);

    public static Task<Result<T>> ExecuteActionOnTrueAsync<T>(this Task<Result<T>> task, bool test, Action<T> trueAction)
        => task.OutputAsync(trueAction);

    public static Task<Result> ExecuteActionAsync(this Task<Result> task, Action? hasValue = null, Action<Exception>? hasException = null)
        => task.OutputAsync(hasValue, hasException);

    public async static Task<Result<T>> ExecuteActionAsync<T>(this Task<Result<T>> task, Action<Result> action)
         => (await task)
            .ExecuteTransaction((value) => CheckForTaskException(task))
            .ExecuteAction(action);

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
