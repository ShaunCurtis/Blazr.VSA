/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public class InvalidGuidIdException : Exception
{
    public InvalidGuidIdException() : base("The Id provided was invalid.  It was almost certainly an empty GUID.") { }
    public InvalidGuidIdException(string message) : base(message) { }
}
