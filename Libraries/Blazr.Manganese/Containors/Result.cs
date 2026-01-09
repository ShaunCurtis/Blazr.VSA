/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public abstract record Result<T>
{
    private Result() { }

    public sealed record True(T Value) : Result<T>;
    public sealed record False(string message) : Result<T>;
    public sealed record Error(Exception exception) : Result<T>;

    public static Result<T> Success(T Value) => new Result<T>.True(Value);
    public static Result<T> Failure(string message) => new Result<T>.False(message);
    public static Result<T> Exception(Exception exception) => new Result<T>.Error(exception);

    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> bind)
        => this switch
        {
            True @true => bind.Invoke(@true.Value),
            False @false => new Result<TOut>.False(@false.message),
            Error @error => new Result<TOut>.Error(error.exception),
            _ => throw new PatternMatchException()
        };

    public Result<TOut> Map<TOut>(Func<T, TOut> bind)
        => this switch
        {
            True @true => TryMap(bind, @true),
            False @false => new Result<TOut>.False(@false.message),
            Error @error => new Result<TOut>.Error(error.exception),
            _ => throw new PatternMatchException()
        };

    public void Write(Action<T> writer, T defaultValue)
    {
        if (this is True @this)
            writer.Invoke(@this.Value);
        else
            writer.Invoke(defaultValue);
    }

    public T Write(T defaultValue)
        => this switch
        {
            True @true => @true.Value,
            False @false => defaultValue,
            Error @error => defaultValue,
            _ => throw new PatternMatchException()
        };

    private static Result<TOut> TryMap<TOut>(Func<T, TOut> map, Result<T>.True result)
    {
        try
        {
            var value = map.Invoke(result.Value);
            return result is null
                ? Result<TOut>.Failure("Result was Null.")
                : Result<TOut>.Success(value);
        }
        catch (Exception ex)
        {
            return Result<TOut>.Exception(ex);
        }
    }
}
