/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Manganese;

public record Result
{
    [MemberNotNullWhen(false, nameof(Message))]
    public bool Success { get; private init; }

    public string? Message { get; private init; }

    [MemberNotNullWhen(true, nameof(Message))]
    public bool Failure => !Success;

    public void Match(Action success, Action<string> fail)
    {
        if (Success)
            success.Invoke();
        else
            fail.Invoke(this.Message);
    }

    private Result(string message)
    {
        Success = false;
        Message = message;
    }

    private Result()
    {
        Success = true;
    }

    public Result Match(Action? success = null, Action<Exception>? fail = null)
    {
        if (Success)
            success?.Invoke();
        else
            fail?.Invoke(new Exception(this.Message));

        return this;
    }

    public static Result Succeeded => new Result();

    public static Result Failed(string message) => new Result(message);
}
