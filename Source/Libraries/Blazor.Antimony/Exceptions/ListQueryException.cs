/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public class ListQueryException : Exception
{
    public ListQueryException() : base("The requested list cannot be retrieved.") { }
    public ListQueryException(string message) : base(message) { }
}
