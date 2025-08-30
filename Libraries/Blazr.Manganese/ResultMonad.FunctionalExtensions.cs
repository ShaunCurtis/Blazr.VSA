/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

/// <summary>
///  Object implementing a functional approach to this management
/// All constructors are private
/// Create instances through the provided  methods
/// </summary>

public partial record Result
{
    public static Result<TOut> CreateFromFunction<TOut>(Func<TOut> function)
        => Result.Success().ExecuteFunction(function);

    public Result ExecuteFunction(Func<Result> function)
    => this.Exception is null
        ? function()
        : this;

    public Result<T> ExecuteFunction<T>(Func<Result<T>> function)
        => this.Exception is null
            ? function()
            : Result<T>.Failure(this.Exception);

    public Result ExecuteFunction(bool test, Func<Result> truefunction, Func<Result> falsefunction)
        => test
            ? this.ExecuteFunction(truefunction)
            : this.ExecuteFunction(falsefunction);

    public Result<T> ExecuteFunction<T>(Func<Result<T>> HasNoException, Func<Exception, Result<T>> HasException)
    => this.HasException
        ? HasException(this.Exception!)
        : HasNoException();

    public Result<TOut> ExecuteFunction<TOut>(Func<TOut> function)
    {
        if (this.Exception is not null)
            return Result<TOut>.Failure(this.Exception!);

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

    public Result SwitchToException(bool test, string message)
        => this.HasException && test
            ? Result.Failure(message)
            : this;

    public Result SwitchToException(bool test, Exception exception)
        => this.HasException && test
                ? Result.Failure(exception)
                : this;

    public Result ExecuteAction(Action? hasNoException = null, Action<Exception>? hasException = null)
    {
        this.Output(hasNoException, hasException);
        return this;
    }

    public Result ExecuteActionWithResult(Func<Result> function)
        => this.HasException
            ? this
            : function();

    public Result ExecuteAction(Action hasNoException)
    {
        this.Output(hasNoException, null);
        return this;
    }

    public Result ExecuteAction(bool test, Action? trueAction = null, Action? falseAction = null)
    {

        if (!this.HasException)
            if (test)
                trueAction?.Invoke();
            else
                falseAction?.Invoke();

        return this;
    }

    public Result ExecuteOnSuccess(Action hasNoException)
    {
        if (!HasException)
            hasNoException.Invoke();

        return this;
    }

    public Result ExecuteOnFailure(Action hasException)
    {
        if (!HasException)
            hasException.Invoke();

        return this;
    }

    public Result OutputOnSuccess(Action hasNoException)
    {
        if (!HasException)
            hasNoException.Invoke();

        return this;
    }

    public Result OutputOnFailure(Action hasException)
    {
        if (!HasException)
            hasException.Invoke();

        return this;
    }

    public Result<T> ToResult<T>(T? value)
        => this.HasException
            ? Result<T>.Failure(this.Exception!)
                : Result<T>.Create(value);
}
