/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese.FunctionalExtensions;

public static class ResultTAsyncExtensions
{
    public async static Task<Result<TOut>> ApplyTransformAsync<TOut, T>(this Result<T> result, Func<T, Task<Result<TOut>>> transform)
        => result.HasException
            ? Result<TOut>.Failure(result._exception!)
            : await transform(result._value!);

    public async static Task<Result<T>> ApplyTransformAsync<T>(this Result<T> result, Func<T, Task<Result<T>>> transform)
        => result.HasException
            ? Result<T>.Failure(result._exception!)
            : await transform(result._value!);

    public async static Task<Result> ApplyTransformAsync<T>(this Result<T> result, Func<T, Task<Result>> transform)
        => result.HasException
            ? Result.Failure(result._exception!)
            : await transform(result._value!);

    public async static Task<Result<TOut>> ApplyTransformAsync<TOut, T>(this Result<T> result, bool test, Func<T, Task<Result<TOut>>> trueTransform, Func<T, Task<Result<TOut>>> falseTransform)
        => test
            ? await result.ApplyTransformAsync<TOut, T>(trueTransform)
            : await result.ApplyTransformAsync<TOut, T>(falseTransform);

    public async static Task<Result<T>> ApplyTransformAsync<T>(this Result<T> result, bool test, Func<T, Task<Result<T>>> trueTransform)
        => test
            ? await result.ApplyTransformAsync<T>(trueTransform)
            : result;

    public async static Task<Result> ApplyTransformAsync<T>(this Result<T> result, bool test, Func<T, Task<Result>> trueTransform, Func<T, Task<Result>> falseTransform)
        => test
            ? await result.ApplyTransformAsync(trueTransform)
            : await result.ApplyTransformAsync(falseTransform);

    public async static Task<Result> ApplyTransformAsync<T>(this Result<T> result, bool test, Func<T, Task<Result>> trueTransform)
        => test
            ? await result.ApplyTransformAsync(trueTransform)
            : result.AsResult;
}
