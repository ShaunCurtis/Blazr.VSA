/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result<T>
{
    public Result<T> ApplyTransform(Func<T, Result<T>> transform)
        => this.HasValue
            ? transform(this.Value!)
            : Result<T>.Failure(this.Exception!);

    public  Result<TOut> ApplyTransform<TOut>( Func<T, Result<TOut>> transform)
        => this.HasValue
            ? transform(this.Value!)
            : Result<TOut>.Failure(this.Exception!);

    public  Result<T> ApplyTransformOnException( Func<Exception, Result<T>> transform)
        => this.HasException
            ? transform(this.Exception!)
            : this;

    public  Result<TOut> ApplyTransform<TOut>( Func<T, TOut> transform)
    {
        if (this.Exception is not null)
            return Result<TOut>.Failure(this.Exception!);

        try
        {
            var value = transform.Invoke(this.Value!);
            return (value is null)
                ? Result<TOut>.Failure(new ResultException("The mapping function returned a null value."))
                : Result<TOut>.Create(value);
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure(ex);
        }
    }

    public  Result ApplyTransform( Func<T, Result> transform)
        => this.HasValue
            ? transform(this.Value!)
            : Result.Failure(this.Exception!);

    public  Result<T> ApplySideEffect( Action<T>? hasValue = null, Action<Exception>? hasException = null)
    {
        if (this.HasValue)
            hasValue?.Invoke(this.Value!);
        else
            hasException?.Invoke(this.Exception!);

        return this;
    }

    public  Result<T> ApplySideEffect( bool test, Action<T> trueAction, Action<T> falseAction)
        => test
            ? this.ApplySideEffect(trueAction, null)
            : this.ApplySideEffect(falseAction, null);

    public  Result<T> ApplySideEffect( bool test, Action<T> trueAction)
        => test
            ? this.ApplySideEffect(trueAction, null)
            : this;

    public  Result<T> ApplyTransform( bool test, Func<T, Result<T>> trueTransform, Func<T, Result<T>> falseTransform)
        => test
            ? this.ApplyTransform<T>(trueTransform)
            : this.ApplyTransform<T>(falseTransform);

    public  Result<TOut> ApplyTransform<TOut>( bool test, Func<T, Result<TOut>> trueTransform, Func<T, Result<TOut>> falseTransform)
        => test
            ? this.ApplyTransform<TOut>(trueTransform)
            : this.ApplyTransform<TOut>(falseTransform);

    public  Result<T> ApplyExceptionOnTrue( bool test, string message)
        => this.HasValue && test
            ? Result<T>.Failure(message)
            : this;

    public  Result<T> ApplyExceptionOnTrue( bool test, Exception exception)
        => this.HasValue && test
            ? Result<T>.Failure(exception)
            : this;

}
