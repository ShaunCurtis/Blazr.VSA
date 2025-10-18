/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result<T>
{
    public Result<T> ExecuteTransaction(Func<T, Result<T>> function)
        => this.HasValue
            ? function(this.Value!)
            : Result<T>.Failure(this.Exception!);

    public Result<TOut> ExecuteTransform<TOut>(Func<T, Result<TOut>> function)
        => this.HasValue
            ? function(this.Value!)
            : Result<TOut>.Failure(this.Exception!);

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

    public Result ExecuteTransform(Func<T, Result> function)
        => this.HasValue
            ? function(this.Value!)
            : Result.Failure(this.Exception!);

    public Result<T> ExecuteConditionalTransaction(bool test, Func<T, Result<T>> truefunction, Func<T, Result<T>> falsefunction)
        => test
            ? this.ExecuteTransaction(truefunction)
            : this.ExecuteTransaction(falsefunction);

    public Result<T> ExecuteConditionalTransaction(T testValue, Func<T, Result<T>> function)
        => this.HasValue && this.Value!.Equals(testValue)
            ? this.ExecuteTransaction(function)
            : this;
    
    public Result<T> ExecuteConditionalTransaction(Func<T, bool> conditionalTest, Func<T, Result<T>> function)
        => this.HasValue && conditionalTest.Invoke(this.Value!)
            ? this.ExecuteTransaction(function)
            : this;

    public Result<TOut> ExecuteConditionalTransform<TOut>(bool test, Func<T, Result<TOut>> truefunction, Func<T, Result<TOut>> falsefunction)
        => test
            ? this.ExecuteTransform<TOut>(truefunction)
            : this.ExecuteTransform<TOut>(falsefunction);

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

    public Result<T> ExecuteAction(Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (this.HasValue)
            hasValue?.Invoke(this.Value!);
        else
            hasException?.Invoke(this.Exception!);

        return this;
    }

    public Result<T> ExecuteActionOnSuccess(Action<T> onSuccess)
    {
        if (this.HasValue)
            onSuccess?.Invoke(this.Value!);

        return this;
    }

    public Result<T> ExecuteAction(bool test, Action<T> trueAction, Action<T> falseAction)
        => test
            ? this.ExecuteAction(trueAction, null)
            : this.ExecuteAction(falseAction, null);

    public Result<T> ExecuteAction(bool test, Action<T> trueAction)
        => test
            ? this.ExecuteAction(trueAction, null)
            : this;

    public Result<T> ExecuteAction(Action<Result> Action)
    {
        Action.Invoke(this.ToResult());
        return this;
    }

    public Result<T> SwitchToExceptionOnTrue(bool test, string message)
        => this.HasValue && test
            ? Result<T>.Failure(message)
            : this;

    public Result<T> SwitchToExceptionOnTrue(bool test, Exception exception)
        => this.HasValue && test
            ? Result<T>.Failure(exception)
            : this;

    public Result<TOut> ToResult<TOut>(TOut? value)
        => this.HasException
            ? Result<TOut>.Failure(this.Exception!)
            : Result<TOut>.Create(value);
}
