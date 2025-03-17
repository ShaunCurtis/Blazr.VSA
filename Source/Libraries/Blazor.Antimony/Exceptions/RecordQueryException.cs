/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public class RecordQueryException : Exception
{
    public RecordQueryException() : base("The requested item cannot be retrieved.") { }
    public RecordQueryException(string message) : base(message) { }
}
