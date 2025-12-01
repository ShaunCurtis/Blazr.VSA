/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public readonly record struct Writer<T>(T Value)
{
    public T Write()
        => this.Value;

    public void Write(Action<T> output)
        => output.Invoke(this.Value);

    public Writer<TOut> Bind<TOut>(Func<T, Writer<TOut>> func)
        => func.Invoke(this.Value);

    public Writer<TOut> Map<TOut>(Func<T, TOut> func)
        => Writer.Read(func.Invoke(this.Value));
}

public static class Writer
{
    public static Writer<T> Read<T>(Func<T> input)
        => new Writer<T>(input.Invoke());

    public static Writer<T> Read<T>(T value)
        => new Writer<T>(value);

    extension<T>(Writer<T> @this)
    {
        public Writer<TOut> Map<TOut>(Func<T, TOut> function)
            => new Writer<TOut>(function.Invoke(@this.Value));

        public Return<T> ToReturnT()
        {
            var x = ReturnT.Read(@this.Value);
            return x;
        }
    }
}

