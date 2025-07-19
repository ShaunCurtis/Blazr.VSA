/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public class EntityRuleException : Exception
{
    public EntityRuleException() : base("The requested action failedan Entity rule.") { }
    public EntityRuleException(string message) : base(message) { }
}
