/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class TaskFunctionalExtensions
{
    public static Task<Result<T>> OutputAsync<T>(this Task<Result<T>> task, Action<T>? hasValue = null, Action<Exception>? hasException = null)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.Output(hasValue: hasValue, hasException: hasException));

    public static Task<Result<T>> OutputAsync<T>(this Task<Result<T>> task, Action<T> hasValue)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.Output(hasValue: hasValue));

    public static Task<T> OutputValueAsync<T>(this Task<Result<T>> task, Func<Exception, T> ExceptionOutput)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.OutputValue(hasException: ExceptionOutput));

    public static async Task<Result<TOut>> ExecuteFunctionAsync<TOut, T>(this Task<Result<T>> task, Func<T, Task<Result<TOut>>> function)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ExecuteFunctionAsync<TOut>(function);
    }

    public static Task<Result<TOut>> ExecuteFunctionAsync<T, TOut>(this Task<Result<T>> task, Func<T, TOut> function)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.ExecuteFunction<TOut>(function));

    public static async Task<Result<T>> ExecuteFunctionAsync<T>(this Task<Result<T>> task, Func<T, Result<T>> function)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return result.ExecuteFunction<T>(function);
    }

    public static async Task<Result<TOut>> ExecuteFunctionAsync<TOut, T>(this Task<Result<T>> task, Func<T, Result<TOut>> function)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return result.ExecuteFunction<TOut>(function);
    }

    public static async Task<Result<T>> ExecuteFunctionAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result<T>>> truefunction, Func<T, Task<Result<T>>> falsefunction)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ExecuteFunctionAsync<T>(test, truefunction, falsefunction);
    }

    public static async Task<Result<T>> ExecuteFunctionAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result<T>>> truefunction)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ExecuteFunctionAsync(test, truefunction);
    }

    public static async Task<Result> ExecuteFunctionAsync<T>(this Task<Result<T>> task, bool test, Func<T, Task<Result>> truefunction)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ExecuteFunctionAsync(test, truefunction);
    }

    public async static Task<Result> ExecuteFunctionAsync<T>(this Task<Result<T>> task, Func<T, Task<Result>> function)
    {
        var result = await task.ContinueWith(CheckForTaskException);
        return await result.ExecuteFunctionAsync(function);
    }

    public static Task<Result> ExecuteFunctionAsync<T>(this Task<Result<T>> task, Func<T, Result> function)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.ExecuteFunction(function));

    public static Task<Result> ToResultAsync<T>(this Task<Result<T>> task)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.ToResult);

    public static Task<Result<T>> MutateStateAsync<T>(this Task<Result<T>> task, Action<T>? hasValue = null, Action<Exception>? hasException = null)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.MutateState(hasValue, hasException));

    public static Task<Result<T>> MutateStateAsync<T>(this Task<Result<T>> task, bool test, Action<T> trueAction)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.MutateState(test, trueAction));

    public static Task<Result> MutateStateAsync(this Task<Result> task, Action? hasValue = null, Action<Exception>? hasException = null)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.MutateState(hasValue, hasException));

    public static Task<Result<T>> MutateStateAsync<T>(this Task<Result<T>> task, Action<Result> action)
        => task
            .ContinueWith(CheckForTaskException)
            .ContinueWith((t) => t.Result.MutateState(action));

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
