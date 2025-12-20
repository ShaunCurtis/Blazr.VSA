/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class ActionException : Exception
{
    public ActionException() : base("The requested action failed to execute correctly.") { }
    public ActionException(string message) : base(message) { }
}
