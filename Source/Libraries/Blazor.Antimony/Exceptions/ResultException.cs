/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public class ResultException : Exception
{
    public ResultException() : base("The Result is Failure.") { }
    public ResultException(string message) : base(message) { }
}
