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
    public  Result ApplyTransform( Func<Result> transform)
    => this.Exception is null
        ? transform()
        : this;

    public  Result<T> ApplyTransform<T>( Func<Result<T>> transform)
        => this.Exception is null
            ? transform()
            : Result<T>.Failure(this.Exception);

    public  Result ApplyTransform( bool test, Func<Result> trueTransform, Func<Result> falseTransform)
        => test
            ? this.ApplyTransform(trueTransform)
            : this.ApplyTransform(falseTransform);

    public Result<T> ApplyTransform<T>(Func<Result<T>> HasNoException, Func<Exception,Result<T>> HasException)
    => this.HasException
        ? HasException(this.Exception!)
        : HasNoException();

    public Result ApplyTransformOnException( bool test, string message)
        => this.HasException && test
            ? Result.Failure(message)
            : this;

    public  Result ApplyTransformOnException( bool test, Exception exception)
        => this.HasException && test
                ? Result.Failure(exception)
                : this;

    public  Result ApplySideEffect( Action? hasNoException = null, Action<Exception>? hasException = null)
    {
        this.Output(hasNoException, hasException);
        return this;
    }

    public  Result ApplySideEffect( Action hasNoException)
    {
        this.Output(hasNoException, null);
        return this;
    }

    public  Result ApplySideEffect( bool test, Action? trueAction = null, Action? falseAction = null)
    {

        if (!this.HasException)
            if (test)
                trueAction?.Invoke();
            else
                falseAction?.Invoke();

        return this;
    }
}
