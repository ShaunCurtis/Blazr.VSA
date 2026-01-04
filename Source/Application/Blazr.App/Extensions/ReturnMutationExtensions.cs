/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public static partial class ReturnMutationExtensions
{
    extension<T>(Task<Return<T>> @this)
    {
        public Task<Return<T>> MutateStateAsync(Action<T>? hasValue = null, Action? hasNoValue = null, Action<Exception>? hasException = null)
            => @this.NotifyAsync(hasValue, hasNoValue, hasException);
    }

    extension<T>(Return<T> @this)
    {
        public Return<T> MutateState(Action<T>? hasValue = null, Action? hasNoValue = null, Action<Exception>? hasException = null)
        => @this.Notify(hasValue, hasNoValue, hasException);
    }

    extension(Return @this)
    {
        public Return MutateState(Action? success = null, Action? failure = null, Action<Exception>? exception = null)
            => @this.Notify(success, failure, exception);
    }

    extension(Task<Return> @this)
    {
        public Task<Return> MutateStateAsync(Action? success = null, Action? failure = null, Action<Exception>? exception = null)
            => @this.NotifyAsync(success, failure, exception);
    }
}