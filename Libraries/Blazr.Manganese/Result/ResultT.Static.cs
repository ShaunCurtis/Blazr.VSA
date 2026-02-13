/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ResultT
{
    public static Result<T> Read<T>(T? value)
         => value is null ? new SuccessResult<T>(value!) : new FailureResult<T>(ResultException.Null);

    public static Result<T> Read<T>(T? value, Exception exception)
         => value is null ? new SuccessResult<T>(value!) : new FailureResult<T>(exception);

    public static Result<T> Read<T>(T? value, string exceptionMessage)
         => value is null ? new SuccessResult<T>(value!) : new FailureResult<T>(ResultException.Create(exceptionMessage));

    public static Result<T> Read<T>(Exception exception)
         => new FailureResult<T>(exception);
    
    public static Result<T> Fail<T>(string message)
         => new FailureResult<T>(ResultException.Create(message));

    public static Result<T> Read<T>(Func<T> func)
    {
        try
        {
            var value = func.Invoke();
            return value is null ? new SuccessResult<T>(value!) : new FailureResult<T>(ResultException.Null);
        }
        catch (Exception e)
        {
            return new FailureResult<T>(e);
        }
    }
}
