public interface Kind<F, A>;

public readonly record struct Containor<T>
{
    private readonly T Value;

    public Containor(T value)
        => this.Value = value;

    public static Containor<T> Read(T value)
        => new Containor<T>(value);

    public T Write()
        => this.Value;

    public static Containor<T> Read(Func<T> func)
        => new Containor<T>(func.Invoke());

    public void Write<TOut>(Action<T> action)
        => action.Invoke(this.Value);
}

public static class Containor
{
    public static Containor<T> Read<T>(T value)
        => Containor<T>.Read(value);
    public static Containor<T> Read<T>(Func<T> func)
        => Containor<T>.Read(func);
}
