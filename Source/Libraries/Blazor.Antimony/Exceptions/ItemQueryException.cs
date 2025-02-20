/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public class ItemQueryException : Exception
{
    public ItemQueryException() : base("The requested item cannot be retrieved.") { }
    public ItemQueryException(string message) : base(message) { }
}
