/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ResultMonadExtensions
{
    public static Result Output(this Result result, Action? hasNoException = null, Action<Exception>? hasException = null)
    {
        if (result.HasException)
            hasException?.Invoke(result.Exception!);
        else
            hasNoException?.Invoke();

        return result;
    }

    public static bool OutputValue(this Result result)
        => !result.HasException;

    public static Result ExecuteTransaction(this Result result, Func<Result> function)
    => result.Exception is null
        ? function()
        : result;

    public static Result<T> ExecuteFunction<T>(this Result result, Func<Result<T>> HasNoException, Func<Exception, Result<T>> HasException)
    => result.HasException
        ? HasException(result.Exception!)
        : HasNoException();

    public static Result<TOut> ExecuteFunction<TOut>(this Result result, Func<TOut> function)
    {
        if (result.Exception is not null)
            return Result<TOut>.Failure(result.Exception!);

        try
        {
            var value = function.Invoke();
            return (value is null)
                ? Result<TOut>.Failure(new ResultException("The transform function returned a null value."))
                : Result<TOut>.Create(value);
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure(ex);
        }
    }

    public static Result ExecuteSideEffect(this Result result, Action? hasNoException = null, Action<Exception>? hasException = null)
    {
        result.Output(hasNoException, hasException);
        return result;
    }

    public static Result ExecuteActionWithResult(this Result result, Func<Result> function)
        => result.HasException
            ? result
            : function();
}
