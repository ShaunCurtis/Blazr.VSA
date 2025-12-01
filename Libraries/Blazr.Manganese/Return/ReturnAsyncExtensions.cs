/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static partial class ReturnAsyncExtensions
{
    extension(Return @this)
    {
        public async Task<Return> BindAsync(Func<Task<Return>> function)
        => @this.Failed
            ? @this
            : await function();

        public async Task<Return<T>> BindAsync<T>(Func<Task<Return<T>>> function)
            => @this.Failed
                ? Return<T>.Failure(@this.Exception!)
                : await function();
    }

    extension<T, TOut>(Task<Return> @this)
    {
        public async Task WriteAsync()
            => (await @this.ContinueWith(CheckForTaskException))
                .Write();

        public async Task WriteAsync(Action? success = null, Action? failure = null, Action<Exception>? exception = null)
            => (await @this.ContinueWith(CheckForTaskException))
                .Write(success, failure, exception);

        public async Task<TOut> WriteAsync(Func<TOut> hasValue, Func<TOut> hasNoValue, Func<Exception, TOut> hasException)
            => (await @this.ContinueWith(CheckForTaskException))
                .Write(hasValue, hasNoValue, hasException);
    }

    private static Return CheckForTaskException(Task<Return> @this)
        => @this.IsCompletedSuccessfully
            ? @this.Result
            : Return.Failure(@this.Exception
                ?? new Exception("The Task failed to complete successfully"));

    private static Return CheckForTaskException(Task @this)
        => @this.IsCompletedSuccessfully
            ? Return.Success()
            : Return.Failure(@this.Exception
                ?? new Exception("The Task failed to complete successfully"));

}

