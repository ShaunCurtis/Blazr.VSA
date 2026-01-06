/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public readonly record struct Null
{
    public bool IsNull { get; private init; } = true;
    
    public Null()
        => this.IsNull = true;
    public Null(object? value)
        => this.IsNull = value is null;

    public static Null Read(Func<bool> func)
        => new Null(func.Invoke());

    public static Null Read(object? value)
        => new Null(value);

    public Null Then(Action? notNullFunc = null, Action? nullFunc = null)
    {
        if (IsNull)
            nullFunc?.Invoke();
        else
            notNullFunc?.Invoke();

        return this;
    }

    public Null Then(Action<bool> func)
    {
        func.Invoke(this.IsNull);

        return this;
    }

    public Null ThenIfNull(Action func)
    {
        if (this.IsNull)
            func.Invoke();

        return this;
    }

    public Null ThenIfNotNull(Action func)
    {
        if (!this.IsNull)
            func.Invoke();

        return this;
    }
}

