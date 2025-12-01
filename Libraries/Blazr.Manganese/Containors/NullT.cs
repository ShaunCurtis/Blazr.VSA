/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Manganese;

public readonly record struct Null<T>
{
    [MemberNotNullWhen(true, nameof(Value))]
    internal bool HasValue { get; init; } = false;
    internal T? Value { get; init; } = default!;

    public Null(T? value)
    {
        if (value is not null)
        {
            Value = value;
            HasValue |= true;
        }
    }

    public static Null<T> Read(T Value)
        => new Null<T>(Value);

    public static Null<T> NoValue(T Value)
    => new Null<T>();

    public T Write(T defaultValue)
        => HasValue ? Value : defaultValue;

    public void Write(Action<T> hasValue, Action? hasNoValue = null)
    {
        if (HasValue)
        {
            hasValue.Invoke(Value);
            return;
        }

        hasNoValue?.Invoke();
    }

    public TOut Write<TOut>(Func<T, TOut> hasValue, Func<TOut> hasNoValue )
        => this.HasValue
            ? hasValue.Invoke(Value)
            : hasNoValue.Invoke();

    public Null<TOut> Bind<TOut>(Func<T, Null<TOut>> func)
        => HasValue ? func.Invoke(Value) : new Null<TOut> { HasValue = false };

    public Null<TOut> Map<TOut>(Func<T, TOut> func)
        => HasValue ? Null<TOut>.Read(func.Invoke(Value)) : new Null<TOut>();
}

public static class NullT
{
    public static Null<T> Read<T>(Func<T> input)
        => new Null<T>(input.Invoke());

    public static Null<T> Read<T>(T value)
        => new Null<T>(value);

    public static Null<T> NoValue<T>()
        => new Null<T>();

    extension<T>(Null<T> @this)
    {
        public Return<T> ToBoolT()
            => @this.HasValue
                ? Return<T>.Success(@this.Value)
                : Return<T>.Failure();
    }
}

