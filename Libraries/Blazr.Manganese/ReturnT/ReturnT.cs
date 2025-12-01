/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Manganese;

public sealed record Return<T>
{
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Exception))]
    public bool HasValue { get; private init; }

    public Exception? Exception { get; private init; }
    public T? Value { get; private init; }

    public bool HasException => Exception is not null;

    private Return(T value)
    {
        Value = value;
        HasValue = true;
    }
    
    private Return(Exception? exception) => Exception = exception;

    private Return() => this.HasValue = false;

    public static Return<T> Read(T? value)
        => value is null
        ? new(new ReturnException("T was null."))
        : new(value);

    public static Return<T> Read(T? value, string errorMessage) =>
        value is null
            ? new(new ReturnException(errorMessage))
            : new(value);

    public static Return<T> Read(object? value = null  ) => new();

    public static Return<T> Read(Func<T?> input) => Read(input.Invoke());

    public static Return<T> Success(T value) => new(value);
    public static Return<T> Failure() => new();
    public static Return<T> Failure(Exception exception) => new(exception);
    public static Return<T> Failure(string message) => new(new ReturnException(message));
}

public static class ReturnT
{
    public static Return<T> Success<T>(T value) => Return<T>.Success(value);
    public static Return<T> Read<T>(T? value) => Return<T>.Read(value);
    public static Return<T> Read<T>(T? value, string errorMessage) => Return<T>.Read(value);
    public static Return<T> Read<T>(Func<T?> input) => Read(input.Invoke());
}
