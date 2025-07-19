/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode;
using Blazr.Diode.Mediator;

namespace Blazr.Cadmium.Presentation;

public abstract class LookUpUIBroker<TItem>
    : ILookUpUIBroker<TItem>
        where TItem : class, ILookupItem, new()
{
    protected IMediatorBroker Mediator;

    public IEnumerable<TItem> Items { get; protected set; } = Enumerable.Empty<TItem>();

    public LookUpUIBroker(IMediatorBroker dataBroker)
    {
        Mediator = dataBroker;
    }

    public abstract ValueTask<Result> LoadAsync();

    public async void OnUpdate(object? sender, EventArgs e)
        => await this.LoadAsync();
}

