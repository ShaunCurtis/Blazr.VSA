/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Presentation;

public class ReadPresenterFactory : IReadPresenterFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ReadPresenterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IReadPresenter<TRecord, TKey>> GetPresenterAsync<TRecord, TKey>(TKey id)
        where TKey : notnull, IEntityId
        where TRecord : class, new()
    {
        var presenter = _serviceProvider.GetRequiredService<IReadPresenter<TRecord, TKey>>();
        await presenter.LoadAsync(id);

        return presenter;
    }
}
