/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public class CommandException : Exception
{
    public CommandException() : base("The requested command failed to execute correctly.") { }
    public CommandException(string message) : base(message) { }
}
