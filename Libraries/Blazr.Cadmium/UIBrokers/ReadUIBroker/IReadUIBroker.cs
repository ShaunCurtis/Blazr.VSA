/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode;

namespace Blazr.Cadmium.Presentation;

public interface IReadUIBroker<TRecord, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    public TRecord Item { get; }

    public event EventHandler? RecordChanged;

    public Result LastResult { get;}

    public ValueTask LoadAsync(TKey id);

    public void Dispose();
}