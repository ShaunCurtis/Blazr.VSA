/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ResultMonadExtensions
{
    extension<T>(Result<T> @this)
    {
        public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> bind)
        => @this switch
        {
            Result<T>.SuccessWithValue @true => bind.Invoke(@true.Value),
            Result<T>.SuccessWithoutValue => new Result<TOut>.SuccessWithoutValue(),
            Result<T>.Failed @false => new Result<TOut>.Failed(@false.message),
            Result<T>.Error @error => new Result<TOut>.Error(error.exception),
            _ => throw new PatternMatchException()
        };

        public async Task<Result<TOut>> BindAsync<TOut>(Func<T, Task<Result<TOut>>> bind)
            => @this switch
            {
                Result<T>.SuccessWithValue @true => await bind.Invoke(@true.Value).ContinueWith(AsyncHelpers.CheckForTaskException),
                Result<T>.SuccessWithoutValue => new Result<TOut>.SuccessWithoutValue(),
                Result<T>.Failed @false => new Result<TOut>.Failed(@false.message),
                Result<T>.Error @error => new Result<TOut>.Error(error.exception),
                _ => throw new PatternMatchException()
            };

        public Result<TOut> Map<TOut>(Func<T, TOut> map)
            => @this switch
            {
                Result<T>.SuccessWithValue @true => TryMap<T, TOut>(map, @true),
                Result<T>.SuccessWithoutValue => new Result<TOut>.SuccessWithoutValue(),
                Result<T>.Failed @false => new Result<TOut>.Failed(@false.message),
                Result<T>.Error @error => new Result<TOut>.Error(error.exception),
                _ => throw new PatternMatchException()
            };

        public async Task<Result<TOut>> MapAsync<TOut>(Func<T, Task<TOut>> map)
            => @this switch
            {
                Result<T>.SuccessWithValue @true => await map.Invoke(@true.Value).ContinueWith(AsyncHelpers.CheckForTaskException),
                Result<T>.SuccessWithoutValue => new Result<TOut>.SuccessWithoutValue(),
                Result<T>.Failed @false => new Result<TOut>.Failed(@false.message),
                Result<T>.Error @error => new Result<TOut>.Error(error.exception),
                _ => throw new PatternMatchException()
            };

        public Result<T> Transform(Func<T> transform)
            => @this switch
            {
                Result<T>.SuccessWithValue @true => TryMap<T>(transform, @this),
                Result<T>.SuccessWithoutValue => new Result<T>.SuccessWithoutValue(),
                Result<T>.Failed @false => new Result<T>.Failed(@false.message),
                Result<T>.Error @error => new Result<T>.Error(error.exception),
                _ => throw new PatternMatchException()
            };

        public Result<T> Write(Action<T> writer, T defaultValue)
        {
            if (@this is Result<T>.SuccessWithValue @result)
                writer.Invoke(@result.Value);
            else
                writer.Invoke(defaultValue);

            return @this;
        }

        public T Write(T defaultValue)
            => @this switch
            {
                Result<T>.SuccessWithValue @true => @true.Value,
                Result<T>.SuccessWithoutValue @false => defaultValue,
                Result<T>.Failed @false => defaultValue,
                Result<T>.Error @error => defaultValue,
                _ => throw new PatternMatchException()
            };

    }

    private static Result<TOut> TryMap<T, TOut>(Func<T, TOut> map, Result<T>.SuccessWithValue result)
        {
            try
            {
                var value = map.Invoke(result.Value);
                return result is null
                    ? Result<TOut>.Failure("Result was Null.")
                    : Result<TOut>.Successful(value);
            }
            catch (Exception ex)
            {
                return Result<TOut>.Exception(ex);
            }
        }

    private static Result<T> TryMap<T>(Func<T> map, Result<T> result)
    {
        if (result is Result<T>.SuccessWithValue || result is Result<T>.SuccessWithoutValue)
        {
            try
            {
                var value = map.Invoke();
                return result is null
                    ? Result<T>.Failure("Result was Null.")
                    : Result<T>.Successful(value);
            }
            catch (Exception ex)
            {
                return Result<T>.Exception(ex);
            }
        }
        return result;
    }
}