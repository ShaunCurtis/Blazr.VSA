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
        => !result.HasException
            ? function()
            : Result.Failure(result.Exception!);

    public static Result Map(this Result result, Action function)
    {
        if (result.Exception is not null)
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

    public static Result Match<T>(this Result result, Action? hasValue = null, Action<Exception>? hasException = null)
    {
        if (!result.HasException)
            hasValue?.Invoke();
        else
            hasException?.Invoke(result.Exception!);

        return result;
    }
}

