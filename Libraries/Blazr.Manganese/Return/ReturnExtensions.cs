/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static partial class ReturnExtensions
{
    extension(Return @this)
    {
        public Return Bind(Func<Return> function)
            => @this.Succeeded
                ? function()
                : Return.Failure(@this.Exception!);

        public T Match<T>(Func<T> Succeeded, Func<Exception, T> Failed)
            => @this.Succeeded
                ? Succeeded.Invoke()
                : Failed.Invoke(@this.Exception!);

        public void Write(Action? success = null, Action? failure = null, Action<Exception>? exception = null)
        {
            switch (@this.Failed, @this.HasException)
            {
                case (false, _) when success is not null:
                    success.Invoke();
                    break;
                case (true, true) when failure is not null:
                    failure.Invoke();
                    break;
                case (true, true) when exception is not null:
                    exception.Invoke(@this.Exception!);
                    break;
            }
        }

        public TOut Write<TOut>(Func<TOut> hasValue, Func<TOut> hasNoValue, Func<Exception, TOut> hasException)
            => (@this.Failed, @this.HasException) switch
            {
                (false, _) => hasValue.Invoke(),
                (true, false) => hasNoValue.Invoke(),
                (true, true) => hasException.Invoke(@this.Exception!)
            };


    public bool Write()
            => @this.Succeeded;
    }
}

