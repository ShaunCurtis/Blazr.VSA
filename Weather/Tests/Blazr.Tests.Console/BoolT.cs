/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Manganese;

public readonly record struct Bool<T>
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; private init; } = false;

    public T Value { get; private init; } = default!;

    public Bool() { }

    public Bool(T? value)
    {
        if (value is not null)
        {
            HasValue = true;
            Value = value;
        }
    }

    public Bool<TOut> Map<TOut>(Func<T, TOut> map)
        => this.HasValue
            ? Bool<TOut>.True(map.Invoke(this.Value))
            : Bool<TOut>.False();

    public Bool<TOut> TryMap<TOut>(Func<T, TOut> map)
    {
        try
        {
            return Map<TOut>(map);
        }
        catch
        {
            return Bool<TOut>.False();
        }
    }

    public T OutputValue(T defaultValue)
        => this.HasValue ? this.Value : defaultValue;

    public T OutputValue(Func<T> defaultValue)
        => this.HasValue ? this.Value : defaultValue.Invoke();

    public void Match(Action<T> hasValue, Action hasNoValue)
    {
        if (this.HasValue)
            hasValue.Invoke(this.Value);
        else
            hasNoValue.Invoke();
    }

    public static Bool<T> True(T value)
        => new Bool<T>(value);

    public static Bool<T> False()
        => new Bool<T>();

    //public static Bool<T> Create(T? value)
    //    => value is null ? new() : new(value);

    //public Bool<TOut> Bind<TOut>(Func<T, Bool<TOut>> bind)
    //    => this.HasValue ? bind.Invoke(this.Value) : new Bool<TOut>();
}
