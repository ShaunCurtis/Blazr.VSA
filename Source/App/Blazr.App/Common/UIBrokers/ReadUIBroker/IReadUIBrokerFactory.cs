/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface IReadUIBrokerFactory
{
    public ValueTask<IReadUIBroker<TRecord, TKey>> GetPresenterAsync<TRecord, TKey>(TKey id)
        where TRecord : class, new()
        where TKey : notnull, IEntityId;
}
