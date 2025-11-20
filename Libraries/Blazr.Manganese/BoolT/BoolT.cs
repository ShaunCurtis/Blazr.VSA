/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Manganese;

public record Bool<T>
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; private init; }
    public Exception? Exception { get; private init; }
    public T? Value { get; private init; }

    private BoolException _defaultException => new BoolException("An error occurred. No specific exception provided.");

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

    public static Bool<T> Input(T? value)
        => value is null
        ? new(new BoolException("T was null."))
        : new(value);

    public static Bool<T> Input(T? value, string errorMessage) =>
        value is null
            ? new(new BoolException(errorMessage))
            : new(value);

    public static Bool<T> Input(Func<T?> input)
        => Input(input.Invoke());

    public static Bool<T> Success(T value) => new(value);

    public static Bool<T> Failure() => new();

    public static Bool<T> Failure(Exception exception) => new(exception);

    public static Bool<T> Failure(string message) => new(new BoolException(message));
}

public static class BoolT
{
    public static Bool<T> Success<T>(T value) => Bool<T>.Success(value);

    public static Bool<T> Input<T>(T? value)
    => Bool<T>.Input(value);

    public static Bool<T> Input<T>(T? value, string errorMessage)
        => Bool<T>.Input(value);

    public static Bool<T> Input<T>(Func<T?> input)
        => Input(input.Invoke());
}
