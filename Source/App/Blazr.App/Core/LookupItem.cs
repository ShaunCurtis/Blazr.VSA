/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public abstract record LookupItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "[NOT SET]";
}
