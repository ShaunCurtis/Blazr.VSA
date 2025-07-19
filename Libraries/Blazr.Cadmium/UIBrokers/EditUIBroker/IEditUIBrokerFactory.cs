/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode;

namespace Blazr.Cadmium.Presentation;

public interface IEditUIBrokerFactory<TRecordEditContext, TKey>
        where TKey : notnull, IEntityId
{
    public ValueTask<IEditUIBroker<TRecordEditContext, TKey>> GetAsync(TKey id);
}

public interface IEditUIBrokerFactory
{
    public ValueTask<IEditUIBroker<TRecordEditContext, TKey>> GetAsync<TRecordEditContext, TKey>(TKey id)
                where TKey : notnull, IEntityId;
}
