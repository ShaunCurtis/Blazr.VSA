/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ResultTAsyncExtensions
{
    extension<T, TOut>(Result<T> @this)
    {
        public async Task<Result<TOut>> BindAsync(Func<T, Task<Result<TOut>>> function)
            => @this switch
            {
                SuccessResult<T> s => await function(s.Value).ContinueWith(CheckForTaskException),
                FailureResult<T> f => new FailureResult<TOut>(f.Exception!),
                _ => throw new InvalidOperationException("Unknown Result type")
            };

        public async Task<Result<TOut>> MapAsync(Func<T, Task<TOut>> function)
            => @this switch
            {
                SuccessResult<T> s => await function(s.Value).ContinueWith(CheckForTaskException),
                FailureResult<T> f => new FailureResult<TOut>(f.Exception!),
                _ => throw new InvalidOperationException("Unknown Result type")
            };
    }

    extension<T, TOut>(Task<Result<T>> @this)
    {
        public async Task<Result<TOut>> BindAsync(Func<T, Task<Result<TOut>>> function)
            => await (await @this.ContinueWith(CheckForTaskException))
                .BindAsync(function);

        public async Task<Result<TOut>> MapAsync(Func<T, Task<TOut>> function)
            => await (await @this.ContinueWith(CheckForTaskException))
                .MapAsync(function);

        public async Task<Result<TOut>> BindAsync(Func<T, Result<TOut>> function)
            => (await @this.ContinueWith(CheckForTaskException))
                .Bind(function);

        public async Task<Result<TOut>> MapAsync(Func<T, TOut> function)
            => (await @this.ContinueWith(CheckForTaskException))
                .Map<T, TOut>(function);

        public async Task<T> WriteAsync(T failureValue)
            => (await @this.ContinueWith(CheckForTaskException))
                .Write(failureValue);

        public async Task WriteAsync(Action<T> success, Action<Exception> failure)
            => (await @this.ContinueWith(CheckForTaskException))
                .Write(success, failure);
    }

    private static Result<T> CheckForTaskException<T>(Task<T> @this)
        => @this.IsCompletedSuccessfully
            ? ResultT.Read(@this.Result)
            : ResultT.Read<T>(@this.Exception
                ?? new Exception("The Task failed to complete successfully"));

    private static Result<T> CheckForTaskException<T>(Task<Result<T>> @this)
    => @this.IsCompletedSuccessfully
        ? @this.Result
        : ResultT.Read<T>(@this.Exception
            ?? new Exception("The Task failed to complete successfully"));
}