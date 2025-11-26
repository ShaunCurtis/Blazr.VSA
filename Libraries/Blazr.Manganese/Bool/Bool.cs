/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Manganese;

/// <summary>
///  Object implementing a functional approach to result management
/// All constructors are private
/// Create instances through the provided static methods
/// </summary>

public record Bool
{
    [MemberNotNullWhen(false, nameof(Exception))]
    public bool Failed { get; private init; }

    public Exception? Exception { get; private init; }

    public bool Succeeded => !this.Failed;
    public string Message => this.Exception?.Message ?? "There is no message to display!";

    private Bool(Exception? exception)
        => Exception = exception
            ?? new BoolException("An error occurred. No specific exception provided.");

    private Bool() { }

    public static Bool Success() => new();

    public static Bool Failure() => new() { Failed = true };
    
    public static Bool Failure(Exception? exception) => exception is null 
        ? new() { Failed = true }
        : new(exception);
    
    public static Bool Failure(string message) => new(new BoolException(message));
}
