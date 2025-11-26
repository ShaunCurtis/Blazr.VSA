/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolTMonadExtensions
{
    extension<T>(Bool<T> boolMonad)
    {
        public Bool<T> Write(Action<T>? hasValue = null, Action<Exception>? hasException = null)
        {
            if (boolMonad.HasValue)
                hasValue?.Invoke(boolMonad.Value!);
            else
                hasException?.Invoke(boolMonad.Exception!);

            return boolMonad;
        }

        public T Write(Func<Exception, T> hasException)
            => boolMonad.HasException
                ? hasException.Invoke(boolMonad.Exception!)
                : boolMonad.Value!;

        public T Write(Func<T> exceptionValue)
            => boolMonad.HasException
                ? exceptionValue.Invoke()
                : boolMonad.Value!;

        public TOut Write<TOut>(Func<T, TOut> hasValue, Func<Exception, TOut> hasException)
            => boolMonad.HasValue
                ? hasValue.Invoke(boolMonad.Value!)
                : hasException.Invoke(boolMonad.Exception!);

        public T Write(T defaultValue)
            => boolMonad.HasValue
                ? boolMonad.Value!
                : defaultValue;

        public Bool<T> Write(Action<Bool> Action)
        {
            Action.Invoke(boolMonad.ToBool());
            return boolMonad;
        }

        public Bool<TOut> ToBoolT<TOut>(TOut? value)
            => boolMonad.HasException
                ? Bool<TOut>.Failure(boolMonad.Exception!)
                : Bool<TOut>.Read(value);

        public Bool ToBool()
            => boolMonad.Exception is null
                ? Bool.Success()
                : Bool.Failure(boolMonad.Exception);
    }
}
