/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Blazr.Manganese;

public readonly record struct Containor<T>
{
    internal T Value { get; private init; }

    public Containor(T value)
        => Value = value;

    public static Containor<T> Read(T Value)
        => new Containor<T>(Value);

    public T Write()
        => this.Value;

    public void Write(Action<T> write)
        => write.Invoke(Value);

    public Containor<TOut> Bind<TOut>(Func<T, Containor<TOut>> func)
        => func.Invoke(Value);

    public Containor<TOut> Map<TOut>(Func<T, TOut> func)
        => Containor<TOut>.Read(func.Invoke(Value));
}

public static class ContainorT
{
    public static Containor<T> Read<T>(Func<T> input)
        => new Containor<T>(input.Invoke());

    public static Containor<T> Read<T>(T value)
        => new Containor<T>(value);

    extension<T>(Containor<T> @this)
    {
        public Return<T> ToReturnT()
            => Return<T>.Success(@this.Value);
    }

    extension<T, TOut>(Containor<T> @this)
    {
        public async Task<Containor<TOut>> BindAsync(Func<T, Task<Containor<TOut>>> function)
            => await function(@this.Value);

        public async Task<Containor<TOut>> MapAsync(Func<T, Task<TOut>> function)
            => ContainorT.Read( await function(@this.Value));
    }


    extension<T, TOut>(Task<Containor<T>> @this)
    {
        public async Task<Containor<TOut>> BindAsync(Func<T, Task<Containor<TOut>>> function)
            => await (await @this)
                .BindAsync(function);

        public async Task<Containor<TOut>> MapAsync(Func<T, Task<TOut>> function)
            => await (await @this)
                .MapAsync(function);
    }

    //extension<T>(Containor<T> @this)
    //{
    //    public Return<T> ToReturnT()
    //        => ReturnT.Read(@this.Value);
    //}
}

