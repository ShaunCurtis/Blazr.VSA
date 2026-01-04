/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static partial class ReturnAsyncExtensions
{
    extension<T>(Return<T> @this)
    {
        public async Task<Return> BindAsync(Func<T, Task<Return>> function)
            => @this.HasValue
                ? await function(@this.Value!).ContinueWith(CheckForTaskException)
                : Return.Failure(@this.Exception!);
    }

    extension<T, TOut>(Return<T> @this)
    {
        public async Task<Return<TOut>> BindAsync(Func<T, Task<Return<TOut>>> function)
            => @this.HasValue
                ? await function(@this.Value!).ContinueWith(CheckForTaskException)
                : Return<TOut>.Failure(@this.Exception!);

        public async Task<Return<TOut>> MapAsync(Func<T, Task<TOut>> function)
            => @this.HasValue
                ? await function(@this.Value!).ContinueWith(CheckForTaskException)
                : Return<TOut>.Failure(@this.Exception!);
    }

    extension<T, TOut>(Task<Return<T>> @this)
    {
        public async Task<Return<TOut>> BindAsync(Func<T, Task<Return<TOut>>> function)
            => await (await @this.ContinueWith(CheckForTaskException))
                .BindAsync(function);

        public async Task<Return<TOut>> MapAsync(Func<T, Task<TOut>> function)
            => await (await @this.ContinueWith(CheckForTaskException))
                .MapAsync(function);
    }

    extension<T, TOut>(Task<Return<T>> @this)
    {
        public async Task<Return<TOut>> BindAsync(Func<T, Return<TOut>> function)
            => (await @this.ContinueWith(CheckForTaskException))
                .Bind(function);

        public async Task<Return<TOut>> MapAsync(Func<T, TOut> function)
            => (await @this.ContinueWith(CheckForTaskException))
                .Map<T, TOut>(function);

        public async Task<Return<TOut>> TryMapAsync(Func<T, TOut> function)
            => (await @this.ContinueWith(CheckForTaskException))
                .TryMap<T, TOut>(function);

        public async Task<T> WriteAsync(T defaultValue)
            => (await @this.ContinueWith(CheckForTaskException))
                .Write(defaultValue);

        public async Task WriteAsync(Action<T>? hasValue = null, Action? hasNoValue = null, Action<Exception>? hasException = null)
            => (await @this.ContinueWith(CheckForTaskException))
                .Write(hasValue, hasNoValue, hasException);

        public async Task<TOut> WriteAsync(Func<T, TOut> hasValue, Func<TOut> hasNoValue, Func<Exception, TOut> hasException)
            => (await @this.ContinueWith(CheckForTaskException))
                .Write(hasValue, hasNoValue, hasException);

        public async Task<Return> ToReturnAsync()
            => (await @this.ContinueWith(CheckForTaskException))
                .ToReturn();
    }

    extension<T>(Task<Return<T>> @this)
    {
        public async Task<Return<T>> SetReturnAsync(Action<Return> returnOut)
            => (await @this.ContinueWith(ReturnAsyncHelpers.CheckForTaskException))
                .SetReturn(returnOut);

        public async Task<T> WriteAsync(T defaultValue)
            => (await @this.ContinueWith(ReturnAsyncHelpers.CheckForTaskException))
                .Write(defaultValue);

        public async Task WriteAsync(Action<T> writer)
            => (await @this.ContinueWith(ReturnAsyncHelpers.CheckForTaskException))
                .Write(writer);

        public async Task WriteAsync(Action<T>? hasValue = null, Action? hasNoValue = null, Action<Exception>? hasException = null)
            => (await @this.ContinueWith(ReturnAsyncHelpers.CheckForTaskException))
                .Write(hasValue, hasNoValue, hasException);

        public async Task WriteAsync(Action<T> success, Action? failure = null)
            => (await @this.ContinueWith(ReturnAsyncHelpers.CheckForTaskException))
                .Write(success, failure, null);

        public async Task<Return> ToReturnAsync()
            => (await @this.ContinueWith(ReturnAsyncHelpers.CheckForTaskException))
                .ToReturn();

        public async Task<Return<T>> NotifyAsync(Action<T>? hasValue = null, Action? hasNoValue = null, Action<Exception>? hasException = null)
            => (await @this.ContinueWith(ReturnAsyncHelpers.CheckForTaskException))
                .Notify(hasValue, hasNoValue, hasException);
    }

    private static Return<T> CheckForTaskException<T>(Task<T> @this)
        => @this.IsCompletedSuccessfully
            ? ReturnT.Read(@this.Result)
            : Return<T>.Failure(@this.Exception
                ?? new Exception("The Task failed to complete successfully"));

    private static Return<T> CheckForTaskException<T>(Task<Return<T>> @this)
    => @this.IsCompletedSuccessfully
        ? @this.Result
        : Return<T>.Failure(@this.Exception
            ?? new Exception("The Task failed to complete successfully"));
}