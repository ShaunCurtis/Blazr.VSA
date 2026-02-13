/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public class ResultException : Exception
{
    public ResultException(string message)
        : base(message) { }

    public static ResultException Create(string message) => new ResultException(message);
    public static ResultException Null => new ResultException("The input value was null");
}
