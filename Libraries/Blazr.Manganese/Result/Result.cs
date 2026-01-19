/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public abstract record Result
{
    private Result() { }

    public sealed record Success() : Result;
    public sealed record Failed(string Message) : Result;

    public static Result Successful() => new Result.Success();
    public static Result Failure(string message) => new Result.Failed(message);

    public Result Match(Action? success = null, Action<string>? failure = null)
    {
        if (this is Result.Failed failed)
            failure?.Invoke(failed.Message);
        else
            success?.Invoke();

        return this;
    }
}
