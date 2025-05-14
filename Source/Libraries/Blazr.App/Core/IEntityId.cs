/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

/// <summary>
/// Defines Entity Id's so we can deal with them in generic componenta
/// </summary>
public interface IEntityId
{
    public bool IsDefault { get; }
}
