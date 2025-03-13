/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Presentation;

public class ReadUIBrokerFactory : IReadUIBrokerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ReadUIBrokerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IReadUIBroker<TRecord, TKey>> GetPresenterAsync<TRecord, TKey>(TKey id)
        where TKey : notnull, IEntityId
        where TRecord : class, new()
    {
        var presenter = _serviceProvider.GetRequiredService<IReadUIBroker<TRecord, TKey>>();
        await presenter.LoadAsync(id);

        return presenter;
    }
}
