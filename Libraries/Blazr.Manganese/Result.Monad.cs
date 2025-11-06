/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result
{
    public static Result Return(Exception? exception = null) 
        => exception is null
            ? new()
            : new(new ResultException("T was null."));
}

public static class ResultMonad
{
    public static Result Bind(this Result result, Func<Result> function)
        => result.Succeeded
            ? function()
            : Result.Failure(result.Exception!);

    public static Result Map(this Result result, Action function)
    {
        if (result.Failed)
            return Result.Failure(result.Exception!);

        try
        {
            function.Invoke();
            return Result.Return();
        }
        catch (Exception ex)
        {
            return Result.Return(ex);
        }
    }

    public static T Match<T>(this Result result, Func<T> Succeeded, Func<Exception, T> Failed)
        => result.Succeeded
        ? Succeeded.Invoke()
        : Failed.Invoke(result.Exception!);
}

