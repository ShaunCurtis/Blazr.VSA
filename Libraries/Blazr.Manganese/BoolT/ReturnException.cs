/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public class ReturnException : Exception
{
    public ReturnException() : base("The Bool operation failed.") { }
    public ReturnException(string message) : base(message) { }

    public static ReturnException Create(string message)
        => new (message);
}
