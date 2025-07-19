/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

/// <summary>
/// Defines Entity Id's so we can deal with them in generic componenta
/// </summary>
public interface IEntity
{
    public bool IdEquality(object entity);
}

