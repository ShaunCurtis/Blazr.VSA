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
    public Result ExecuteTransaction(Func<Result> function)
    => this.Exception is null
        ? function()
        : this;

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

    public Result ExecuteSideEffect(Action? hasNoException = null, Action<Exception>? hasException = null)
    {
        this.Output(hasNoException, hasException);
        return this;
    }

    public Result ExecuteActionWithResult(Func<Result> function)
        => this.HasException
            ? this
            : function();

    public Result<T> ToResult<T>(T? value)
        => this.HasException
            ? Result<T>.Failure(this.Exception!)
                : Result<T>.Create(value);
}
