/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Presentation;

public class LookupUIBrokerFactory : ILookupUIBrokerFactory
{
    private IServiceProvider _serviceProvider;
    public LookupUIBrokerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<ILookUpUIBroker<TLookupRecord>> GetAsync<TLookupRecord, TPresenter>()
    where TLookupRecord : class, ILookupItem, new()
    where TPresenter : class, ILookUpUIBroker<TLookupRecord>
    {
        var presenter = ActivatorUtilities.CreateInstance<TPresenter>(_serviceProvider);
        ArgumentNullException.ThrowIfNull(presenter, nameof(presenter));
        await presenter.LoadAsync();

        return presenter;
    }
}
