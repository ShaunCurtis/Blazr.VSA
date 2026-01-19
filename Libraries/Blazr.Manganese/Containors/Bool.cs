/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;



public readonly record struct Bool(bool Value)
{
    public bool Write()
        => this.Value;

    public void Write(Action<bool> output)
        => output.Invoke(this.Value);

    public Bool Bind(Func<bool, Bool> func)
        => func.Invoke(this.Value);

    public Bool BindIfTrue(Func<bool, Bool> func)
    {
        if (this.Value)
            return func.Invoke(this.Value);
        else
            return this;
    }

    public Bool BindIfFalse(Func<bool, Bool> func)
    {
        if (!this.Value)
            return func.Invoke(this.Value);
        else
            return this;
    }

    public Bool Map(Func<bool, bool> func)
        => new Bool(func.Invoke(this.Value));

    public Bool MapIfTrue(Func<bool, bool> func)
    {
        if (this.Value)
            return new Bool(func.Invoke(this.Value));
        else
            return this;
    }

    public Bool MapIfFalse(Func<bool, bool> func)
    {
        if (!this.Value)
            return new Bool(func.Invoke(this.Value));
        else
            return this;
    }

    public static Bool Read(bool value)
        => new Bool(value);

    public static Bool Read(Func<bool> func)
        => new Bool(func.Invoke());

    public Bool Then(Action? trueFunc = null, Action? falseFunc = null)
    {
        if (this.Value)
            trueFunc?.Invoke();
        else
            falseFunc?.Invoke();

        return this;
    }

    public Bool Then(Action<bool> func)
    {
        func.Invoke(this.Value);

        return this;
    }

    public Bool ThenOnTrue(Action func)
    {
        if (this.Value)
            func.Invoke();

        return this;
    }

    public Bool ThenOnFalse(Action func)
    {
        if (!this.Value)
            func.Invoke();

        return this;
    }
}

