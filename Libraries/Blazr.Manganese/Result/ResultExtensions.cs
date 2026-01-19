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
            => @this is Result<T>.Success;

        public bool HasFailed
        => @this is Result<T>.Failed;

        public bool HasException
        => @this is Result<T>.Error;

        public bool HasSucceeded
        => @this is Result<T>.Success;

        public bool HasNotSucceeded
        => @this is Result<T>.Error || @this is Result<T>.Failed;

        public Result<T>.Success AsSuccess
            => @this as Result<T>.Success ?? throw new InvalidCastException();

        public Result<T>.Failed AsFailure
            => @this as Result<T>.Failed ?? throw new InvalidCastException();

        public Result<T>.Error AsException
            => @this as Result<T>.Error ?? throw new InvalidCastException();

        public Result<TOut> Convert<TOut>(TOut? value)
            => @this switch
            {
                Result<T>.Success => ResultT.Successful(value ?? default!),
                Result<T>.Failed @false => new Result<TOut>.Failed(@false.message),
                Result<T>.Error @error => new Result<TOut>.Error(error.exception),
                _ => throw new PatternMatchException()
            };
    }
    extension(Result @this)
    {
        public bool IsSuccess
            => @this is Result.Success;

        public bool IsFailure
            => @this is Result.Failed;

        public bool HasSucceeded
            => @this is Result.Success;

        public bool HasNotSucceeded
            => @this is Result.Failed;

        public Result.Success AsSuccess
            => @this as Result.Success ?? throw new InvalidCastException();

        public Result.Failed AsFailure
            => @this as Result.Failed ?? throw new InvalidCastException();
    }
}