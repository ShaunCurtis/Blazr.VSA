/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public readonly record struct IOMonad<T>(T Value)
{
    public T Output()
        => this.Value;

    public void Output(Action<T> output)
        => output.Invoke(this.Value);

    public IOMonad<TOut> Bind<TOut>(Func<T, IOMonad<TOut>> func)
        => func.Invoke(this.Value);

    public IOMonad<TOut> Map<TOut>(Func<T, TOut> func)
        => IOMonad.Input(func.Invoke(this.Value));
}

public static class IOMonad
{
    public static IOMonad<T> Input<T>(Func<T> input)
        => new IOMonad<T>(input.Invoke());

    public static IOMonad<T> Input<T>(T value)
        => new IOMonad<T>(value);

    extension<T>(IOMonad<T> ioMonad)
    {
        public IOMonad<TOut> Map<TOut>(Func<T, TOut> function)
            => new IOMonad<TOut>(function.Invoke(ioMonad.Value));

        public Bool<T> ToBoolT()
        //=> Bool.Input(ioMonad.Value);
        {
            var x = BoolT.Read(ioMonad.Value);
            return x;
        }
    }
}

