/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public class BoolException : Exception
{
    public BoolException() : base("The Bool operation failed.") { }
    public BoolException(string message) : base(message) { }

    public static BoolException Create(string message)
        => new (message);
}
