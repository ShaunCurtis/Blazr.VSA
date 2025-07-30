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
    public  Result ExecuteFunction( Func<Result> function)
    => this.Exception is null
        ? function()
        : this;

    public  Result<T> ExecuteFunction<T>( Func<Result<T>> function)
        => this.Exception is null
            ? function()
            : Result<T>.Failure(this.Exception);

    public  Result ExecuteFunction( bool test, Func<Result> truefunction, Func<Result> falsefunction)
        => test
            ? this.ExecuteFunction(truefunction)
            : this.ExecuteFunction(falsefunction);

    public Result<T> ExecuteFunction<T>(Func<Result<T>> HasNoException, Func<Exception,Result<T>> HasException)
    => this.HasException
        ? HasException(this.Exception!)
        : HasNoException();

    public Result ExecuteFunctionOnException( bool test, string message)
        => this.HasException && test
            ? Result.Failure(message)
            : this;

    public  Result ExecuteFunctionOnException( bool test, Exception exception)
        => this.HasException && test
                ? Result.Failure(exception)
                : this;

    public  Result MutateState( Action? hasNoException = null, Action<Exception>? hasException = null)
    {
        this.Output(hasNoException, hasException);
        return this;
    }

    public  Result MutateState( Action hasNoException)
    {
        this.Output(hasNoException, null);
        return this;
    }

    public  Result MutateState( bool test, Action? trueAction = null, Action? falseAction = null)
    {

        if (!this.HasException)
            if (test)
                trueAction?.Invoke();
            else
                falseAction?.Invoke();

        return this;
    }
}
