/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode;

namespace Blazr.Cadmium.Presentation;

public interface ILookUpUIBroker<TItem>
    where TItem : class, ILookupItem, new()
{
    public IEnumerable<TItem> Items { get; }

    public ValueTask<Result> LoadAsync();
}
