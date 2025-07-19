/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public record Maybe<T>
{
    private readonly T? _value = default;
    private bool _hasValue;
    
    private Maybe(T value)
    {
        _value = value;
        _hasValue = true;
    }

    private Maybe()
    {
        _hasValue = false;
    }

    public Maybe<TOut> Map<TOut>(Func<T, Maybe<TOut>> func)
        => _hasValue ? func(_value!) : Maybe<TOut>.None();

    public Maybe<TOut> Map<TOut>(Func<T, TOut> func)
    {
        if (_value is null)
            return Maybe<TOut>.None();

        var result = func(_value);
        
        return result is null 
            ? Maybe<TOut>.None() 
            : new Maybe<TOut>(result);
    }

    public Maybe<T> Output(Action<T>? some = null, Action? none = null)
    {
        if (_value is not null && some != null)
            some(_value);

        if (_value is null && none != null)
            none();

        return this;
    }

    public static Maybe<T> Create(T? value)
        => value is null ? Maybe<T>.None() : Maybe<T>.Some(value);

    public static Maybe<T> None() => new Maybe<T>();

    public static Maybe<T> Some(T value)
        => new Maybe<T>(value);
}
