/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolTMonad
{
    extension<T>(Bool<T> @this)
    {
        public Bool<TOut> Bind<TOut>(Func<T, Bool<TOut>> function)
            => @this.HasValue
                ? function(@this.Value!)
                : Bool<TOut>.Failure(@this.Exception!);

        public Bool<TOut> Map<TOut>(Func<T, TOut> function)
        {
            if (@this.Exception is not null)
                return Bool<TOut>.Failure(@this.Exception!);

                var value = function.Invoke(@this.Value!);
                return (value is null)
                    ? Bool<TOut>.Failure(new BoolException("The function returned a null value."))
                    : BoolT.Read(value);
        }

        public Bool<TOut> TryMap<TOut>(Func<T, TOut> function)
        {
            if (@this.Exception is not null)
                return Bool<TOut>.Failure(@this.Exception!);

            try
            {
                var value = function.Invoke(@this.Value!);
                return (value is null)
                    ? Bool<TOut>.Failure(new BoolException("The function returned a null value."))
                    : BoolT.Read(value);
            }
            catch (Exception ex)
            {
                return Bool<TOut>.Failure(ex);
            }
        }

        public Bool<T> Map(Action<T>? hasValue = null, Action<Exception>? hasException = null)
        {
            if (@this.HasValue)
                hasValue?.Invoke(@this.Value!);
            else
                hasException?.Invoke(@this.Exception!);

            return @this;
        }

        public IOMonad<TOut> ToIOMonad<TOut>(Func<TOut> hasNoValue, Func<T, TOut>? hasValue = null, Func<Exception, TOut>? hasException = null)
            => (Tuple.Create(@this.HasValue, @this.HasException)) switch
            {
                (true, false) => IOMonad.Input(hasValue is not null ? hasValue.Invoke(@this.Value!) : default!),
                (false, true) => IOMonad.Input(hasException is not null ? hasException.Invoke(@this.Exception!) : default!),
                _ => IOMonad.Input(hasNoValue.Invoke()),
            };
    }
}
