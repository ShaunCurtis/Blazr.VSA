/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode;

namespace Blazr.Cadmium.Presentation;

public interface IEditUIBrokerFactory<TRecord, TMutor, TKey>
        where TKey : notnull, IEntityId
        where TMutor : IRecordMutor<TRecord>
        where TRecord : class, new()
{
    public ValueTask<IEditUIBroker<TRecord,TMutor, TKey>> GetAsync(TKey id);
}

public interface IEditUIBrokerFactory
{
    public ValueTask<IEditUIBroker<TRecord, TMutor, TKey>> GetAsync<TRecord,TMutor, TKey>(TKey id)
        where TRecord : class, new()
        where TMutor : IRecordMutor<TRecord>
        where TKey : notnull, IEntityId;
}
