/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ReturnTMonadExtensions
{
    extension<T>(Return<T> boolMonad)
    {
        public Return<T> Write(Action<T>? hasValue = null, Action<Exception>? hasException = null)
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

        public Return<T> Write(Action<Bool> Action)
        {
            Action.Invoke(boolMonad.ToBool());
            return boolMonad;
        }

        public Return<TOut> ToBoolT<TOut>(TOut? value)
            => boolMonad.HasException
                ? Return<TOut>.Failure(boolMonad.Exception!)
                : Return<TOut>.Read(value);

        public Bool ToBool()
            => boolMonad.Exception is null
                ? Bool.Success()
                : Bool.Failure(boolMonad.Exception);
    }
}
