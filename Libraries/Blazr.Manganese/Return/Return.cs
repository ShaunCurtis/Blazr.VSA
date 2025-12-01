/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Manganese;

public record Return
{
    [MemberNotNullWhen(false, nameof(Exception))]
    public bool Failed { get; private init; }

    public Exception? Exception { get; private init; }

    public bool Succeeded => !this.Failed;
    public bool HasException => Exception is not null;
    public string Message => this.Exception?.Message ?? "There is no message to display!";

    private Return(Exception? exception)
        => Exception = exception
            ?? new ReturnException("An error occurred. No specific exception provided.");

    private Return() { }

    public static Return Read(object? value) => value is null ? Failure() : Success();

    public static Return Read<T>(Func<T?> input) => Read(input.Invoke());

    public static Return Success() => new();

    public static Return Failure() => new() { Failed = true };
    
    public static Return Failure(Exception? exception) => exception is null 
        ? new() { Failed = true }
        : new(exception);
    
    public static Return Failure(string message) => new(new ReturnException(message));
}
