/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ResultTMonadExtensions
{
    extension<T>(Result<T> @this)
    {
        public Result<TResult> Map<TResult>(Func<T, TResult> func)
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

        public void Match(Action<T>? success = null, Action<Exception>? failure = null)
        {
            switch (@this)
            {
                case SuccessResult<T> s:
                    success?.Invoke(s.Value);
                    break;
                case FailureResult<T> f:
                    failure?.Invoke(f.Exception);
                    break;
            }
        }

        public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> func)
            => @this switch
            {
                SuccessResult<T> s => func.Invoke(s.Value),
                FailureResult<T> f => new FailureResult<TResult>(f.Exception),
                _ => new FailureResult<TResult>(ResultException.Null)
            };

        public Result<TResult> Bind<TResult>(Func<T, TResult> func)
        {
            try
            {
                if (@this is FailureResult<T> f)
                    return new FailureResult<TResult>(f.Exception);

                var previousSuccessValue = ((SuccessResult<T>)@this).Value;
                return func(previousSuccessValue).ToResult;
            }
            catch (Exception e)
            {
                return new FailureResult<TResult>(e);
            }
        }
    }
}

