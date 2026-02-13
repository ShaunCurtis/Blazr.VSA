/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ResultFunctionalExtensions
{
    extension<T>(Result<T> @this)
    {
        public Result<TResult> Then<TResult>(Func<T, TResult> func)
        {
            try
            {
                return @this switch
                {
                    SuccessResult<T> hv => ResultT.Read(func.Invoke(hv.Value)),
                    _ => new FailureResult<TResult>(ResultException.Null)
                };
            }
            catch (Exception e)
            {
                return new FailureResult<TResult>(e);
            }
        }

        public void Write(Action<T> success, Action<Exception> failure)
        {
            switch (@this)
            {
                case SuccessResult<T> s:
                    success.Invoke(s.Value);
                    break;
                case FailureResult<T> f:
                    failure.Invoke(f.Exception);
                    break;
            }
        }

        public T Write(T failureValue)
            => @this switch
            {
                SuccessResult<T> s => s.Value,
                FailureResult<T> f => failureValue,
                _ => throw new NotImplementedException("Result Object type is undefined.")
            };

        public Result ToResult() =>
            @this is FailureResult<T> f ?  Result.Failed(f.Exception.Message) : Result.Succeeded;
    }

    /// <summary>
    /// Adds a Conversion Extension method to any type that will convert it to a Result of that type.  
    /// If the value is null, it will return a FailureResult with a Null Exception
    /// , otherwise it will return a SuccessResult with the value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="this"></param>
    extension<T>(T @this)
    {
        public Result<T> ToResult =>
            @this is null ? new SuccessResult<T>(@this) : new FailureResult<T>(ResultException.Null);
    }
}

