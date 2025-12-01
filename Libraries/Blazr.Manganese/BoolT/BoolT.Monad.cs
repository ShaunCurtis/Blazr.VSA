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
                ? function(@this.Value!)
                : Return<TOut>.Failure(@this.Exception!);

        public Return<TOut> Map<TOut>(Func<T, TOut> function)
        {
            if (@this.Exception is not null)
                return Return<TOut>.Failure(@this.Exception!);

            var value = function.Invoke(@this.Value!);
            return (value is null)
                ? Return<TOut>.Failure(new ReturnException("The function returned a null value."))
                : BoolT.Read(value);
        }

        public Return<TOut> TryMap<TOut>(Func<T, TOut> function)
        {
            if (@this.Exception is not null)
                return Return<TOut>.Failure(@this.Exception!);

            try
            {
                var value = function.Invoke(@this.Value!);
                return (value is null)
                    ? Return<TOut>.Failure(new ReturnException("The function returned a null value."))
                    : BoolT.Read(value);
            }
            catch (Exception ex)
            {
                return Return<TOut>.Failure(ex);
            }
        }

        public Return<T> Match(Action<T>? hasValue = null, Action<Exception>? hasException = null)
        {
            if (@this.HasValue)
                hasValue?.Invoke(@this.Value!);
            else
                hasException?.Invoke(@this.Exception!);

            return @this;
        }

        public Writer<TOut> ToWriter<TOut>(Func<TOut> hasNoValue, Func<T, TOut>? hasValue = null, Func<Exception, TOut>? hasException = null)
            => (Tuple.Create(@this.HasValue, @this.HasException)) switch
            {
                (true, false) => Writer.Read(hasValue is not null ? hasValue.Invoke(@this.Value!) : default!),
                (false, true) => Writer.Read(hasException is not null ? hasException.Invoke(@this.Exception!) : default!),
                _ => Writer.Read(hasNoValue.Invoke()),
            };
    }
}
