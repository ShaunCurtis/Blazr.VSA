/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Presentation;

public class EditUIBrokerFactory : IEditUIBrokerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public EditUIBrokerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IEditUIBroker<TEditContext, TKey>> GetAsync<TEditContext, TKey>(TKey id)
        where TKey : notnull, IEntityId
    {
        var presenter = _serviceProvider.GetRequiredService<IEditUIBroker<TEditContext, TKey>>();
        await presenter.LoadAsync(id);

        return presenter;
    }
}
