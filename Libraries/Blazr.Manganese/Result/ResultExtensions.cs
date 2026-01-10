/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ResultExtensions
{
    extension<T>(Result<T> @this)
    {
        public bool HasValue
            => @this is Result<T>.SuccessWithValue;

        public bool HasNoValue
            => @this is Result<T>.SuccessWithoutValue;

        public bool HasFailed
        => @this is Result<T>.Failed;

        public bool HasException
        => @this is Result<T>.Error;

        public bool HasSucceeded
        => @this is Result<T>.SuccessWithValue || @this is Result<T>.SuccessWithoutValue;

        public bool HasNotSucceeded
        => @this is Result<T>.Error || @this is Result<T>.Failed;

        public Result<TOut> Convert<TOut>(TOut? value)
        => @this switch
        {
            Result<T>.SuccessWithValue => ResultT.Successful(value ?? default!),
            Result<T>.SuccessWithoutValue => new Result<TOut>.SuccessWithoutValue(),
            Result<T>.Failed @false => new Result<TOut>.Failed(@false.message),
            Result<T>.Error @error => new Result<TOut>.Error(error.exception),
            _ => throw new PatternMatchException()
        };
    }
}