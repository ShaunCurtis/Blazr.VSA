/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ReturnTExtensions
{
    extension<T>(Return<T> @this)
    {
        public Return<TOut> Bind<TOut>(Func<T, Return<TOut>> function)
            => @this.HasValue
                ? function(@this.Value)
                : Return<TOut>.Failure(@this.Exception);

        public Return<TOut> Map<TOut>(Func<T, TOut> function)
             => @this.HasValue
                ? ReturnT.Read(function.Invoke(@this.Value!)) 
                    ?? Return<TOut>.Failure(new ReturnException("The function returned a null value."))
                : Return<TOut>.Failure(@this.Exception!);


        public Return<TOut> TryMap<TOut>(Func<T, TOut> function)
        {
            try
            {
                return @this.HasValue
                    ? ReturnT.Read(function.Invoke(@this.Value)) 
                            ?? Return<TOut>.Failure(new ReturnException("The function returned a null value."))
                    : Return<TOut>.Failure(@this.Exception);
            }
            catch (Exception ex)
            {
                return Return<TOut>.Failure(ex);
            }
        }

        public void Write(Action<T>? hasValue = null, Action? hasNoValue = null, Action<Exception>? hasException = null)
        {
            switch (@this.HasValue, @this.HasException)
            {
                case (true, _) when hasValue is not null:
                    hasValue.Invoke(@this.Value!);
                    break;
                case (false, false) when hasNoValue is not null:
                    hasNoValue.Invoke();
                    break;
                case (false, true) when hasException is not null:
                    hasException.Invoke(@this.Exception!);
                    break;
            }
        }

        public Return<T> Notify(Action<T>? hasValue = null, Action? hasNoValue = null, Action<Exception>? hasException = null)
        {
            switch (@this.HasValue, @this.HasException)
            {
                case (true, _) when hasValue is not null:
                    hasValue.Invoke(@this.Value!);
                    break;
                case (false, false) when hasNoValue is not null:
                    hasNoValue.Invoke();
                    break;
                case (false, true) when hasException is not null:
                    hasException.Invoke(@this.Exception!);
                    break;
            }
            return @this;
        }

        public TOut Write<TOut>(Func<T, TOut> hasValue, Func<TOut> hasNoValue, Func<Exception, TOut> hasException)
            => (@this.HasValue, @this.HasException) switch
            {
                (true, _) => hasValue.Invoke(@this.Value!),
                (false, false) => hasNoValue.Invoke(),
                (false, true) => hasException.Invoke(@this.Exception!)
            };

        public TOut Write<TOut>(Func<T, TOut> hasValue, Func<TOut> hasNoValue)
            => @this.HasValue switch
            {
                true => hasValue.Invoke(@this.Value!),
                false => hasNoValue.Invoke(),
            };

        public T Write(T defaultValue)
            => @this.HasValue
                ? @this.Value
                : defaultValue;

        public Return ToReturn()
            => @this.Exception is null
                ? Return.Success()
                : Return.Failure(@this.Exception);

        public Return<T> SetReturn(Action<Return> returnOut)
        {
            if (@this.HasValue)
            {
                returnOut.Invoke(Return.Success());
                return @this;
            }

            returnOut.Invoke(Return.Failure(@this.Exception));
            return @this;
        }
    }
}
