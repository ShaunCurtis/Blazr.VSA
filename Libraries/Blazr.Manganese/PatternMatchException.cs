/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Manganese;

public class PatternMatchException : Exception
{
    public PatternMatchException() : base("The pattern match fell out the bottom with no match.") { }
    public PatternMatchException(string message) : base(message) { }
}
