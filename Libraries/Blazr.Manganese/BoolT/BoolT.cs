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
    public readonly Exception? Exception;
    public readonly T? Value;

    private BoolException _defaultException => new BoolException("An error occurred. No specific exception provided.");

    public bool HasException => Exception is not null;

    private Bool(T value)
        => Value = value;

    private Bool(Exception? exception)
        => Exception = exception;

    private Bool()
        => this.HasValue = false;

    public static Bool<T> Create(T? value) 
        => value is null
        ? new(new BoolException("T was null."))
        : new(value);

    public static Bool<T> Create(T? value, string errorMessage) =>
        value is null
            ? new(new BoolException(errorMessage))
            : new(value);

    public static Bool<T> Success(T value) => new(value);

    public static Bool<T> Failure() => new();

    public static Bool<T> Failure(Exception exception) => new(exception);

    public static Bool<T> Failure(string message) => new(new BoolException(message));
}
