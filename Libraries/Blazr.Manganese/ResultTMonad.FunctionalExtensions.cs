/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result<T>
{
    //public Result<T> ExecuteFunction(Func<T, Result<T>> function)
    //    => this.HasValue
    //        ? function(this.Value!)
    //        : Result<T>.Failure(this.Exception!);

    public Result<TOut> ExecuteFunction<TOut>(Func<T, Result<TOut>> function)
        => this.HasValue
            ? function(this.Value!)
            : Result<TOut>.Failure(this.Exception!);

    public Result<T> ExecuteFunctionOnException(Func<Exception, Result<T>> function)
        => this.HasException
            ? function(this.Exception!)
            : this;

    public Result<TOut> ExecuteFunction<TOut>(Func<T, TOut> function)
    {
        if (this.Exception is not null)
            return Result<TOut>.Failure(this.Exception!);

        try
        {
            var value = function.Invoke(this.Value!);
            return (value is null)
                ? Result<TOut>.Failure(new ResultException("The function returned a null value."))
                : Result<TOut>.Create(value);
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure(ex);
        }
    }

    public Result ExecuteFunction(Func<T, Result> function)
        => this.HasValue
            ? function(this.Value!)
            : Result.Failure(this.Exception!);

    public Result<T> ExecuteFunction(bool test, Func<T, Result<T>> truefunction, Func<T, Result<T>> falsefunction)
        => test
            ? this.ExecuteFunction<T>(truefunction)
            : this.ExecuteFunction<T>(falsefunction);

    public Result<TOut> ExecuteFunction<TOut>(bool test, Func<T, Result<TOut>> truefunction, Func<T, Result<TOut>> falsefunction)
        => test
            ? this.ExecuteFunction<TOut>(truefunction)
            : this.ExecuteFunction<TOut>(falsefunction);

    public Result<T> Dispatch(Func<T, Result<T>> function)
        => this.HasValue
            ? function(this.Value!)
            : Result<T>.Failure(this.Exception!);

    public Result<TOut> Dispatch<TOut>(Func<T, Result<TOut>> function)
        => this.HasValue
            ? function(this.Value!)
            : Result<TOut>.Failure(this.Exception!);

    public Result<T> Dispatch(Func<T, Result> function)
    {
        var outerResult = this;

        return (this.HasException)
            ? Result<T>.Failure(this.Exception!)
            : function(this.Value!)
                .ExecuteFunction(
                    HasNoException: () => outerResult,
                    HasException: (ex) => Result<T>.Failure(ex)
                    );
    }

    public Result<T> MutateState(Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (this.HasValue)
            hasValue?.Invoke(this.Value!);
        else
            hasException?.Invoke(this.Exception!);

        return this;
    }

    public Result<T> MutateState(bool test, Action<T> trueAction, Action<T> falseAction)
        => test
            ? this.MutateState(trueAction, null)
            : this.MutateState(falseAction, null);

    public Result<T> MutateState(bool test, Action<T> trueAction)
        => test
            ? this.MutateState(trueAction, null)
            : this;

    public Result<T> MutateState(Action<Result> Action)
    {
        Action.Invoke(this.ToResult);
        return this;
    }

    public Result<T> ApplyExceptionOnTrue(bool test, string message)
        => this.HasValue && test
            ? Result<T>.Failure(message)
            : this;

    public Result<T> ApplyExceptionOnTrue(bool test, Exception exception)
        => this.HasValue && test
            ? Result<T>.Failure(exception)
            : this;
}
