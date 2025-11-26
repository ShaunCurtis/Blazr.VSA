/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Manganese;

public sealed record Bool<T>
{
    public Exception? Exception { get; private init; }
    public T? Value { get; private init; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Exception))]
    public bool HasValue { get; private init; }

    public bool HasException => Exception is not null;

    private Bool(T value)
    {
        Value = value;
        HasValue = true;
    }
    
    private Bool(Exception? exception)
        => Exception = exception;

    private Bool()
        => this.HasValue = false;

    public static Bool<T> Read(T? value)
        => value is null
        ? new(new BoolException("T was null."))
        : new(value);

    public static Bool<T> Read(T? value, string errorMessage) =>
        value is null
            ? new(new BoolException(errorMessage))
            : new(value);

    public static Bool<T> Read(object? value = null  )
        => new();

    public static Bool<T> Read(Func<T?> input)
        => Read(input.Invoke());

    public static Bool<T> Success(T value) => new(value);

    public static Bool<T> Failure() => new();

    public static Bool<T> Failure(Exception exception) => new(exception);

    public static Bool<T> Failure(string message) => new(new BoolException(message));
}

public static class BoolT
{
    public static Bool<T> Success<T>(T value) => Bool<T>.Success(value);

    public static Bool<T> Read<T>(T? value)
    => Bool<T>.Read(value);

    public static Bool<T> Read<T>(T? value, string errorMessage)
        => Bool<T>.Read(value);

    public static Bool<T> Read<T>(Func<T?> input)
        => Read(input.Invoke());
}
