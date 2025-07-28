/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Cadmium.Core;
using Blazr.Cadmium.Presentation;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode;

namespace Blazr.Cadmium;

public interface IUIEntityProvider<TRecord, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    public string SingleDisplayName { get; }
    public string PluralDisplayName { get;}
    public Type? EditForm { get; }
    public Type? ViewForm { get; }
    public string Url { get; }

    public ValueTask<IReadUIBroker<TRecord, TKey>> GetReadUIBrokerAsync(TKey id);

    public ValueTask<IEditUIBroker<TRecordEditContext, TKey>> GetEditUIBrokerAsync<TRecordEditContext>(TKey id)
                where TRecordEditContext : IRecordEditContext<TRecord>, new();

    public ValueTask<IGridUIBroker<TRecord>> GetGridUIBrokerAsync(Guid contextId);

    public ValueTask<IGridUIBroker<TRecord>> GetGridUIBrokerAsync(Guid contextId, UpdateGridRequest<TRecord> resetGridRequest );

}
