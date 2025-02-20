/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Presentation;

public class LookupPresenterFactory : ILookupPresenterFactory
{
    private IServiceProvider _serviceProvider;
    public LookupPresenterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<ILookUpPresenter<TLookupRecord>> GetPresenterAsync<TLookupRecord, TPresenter>()
    where TLookupRecord : class, ILookupItem, new()
    where TPresenter : class, ILookUpPresenter<TLookupRecord>
    {
        var presenter = ActivatorUtilities.CreateInstance<TPresenter>(_serviceProvider);
        ArgumentNullException.ThrowIfNull(presenter, nameof(presenter));
        await presenter.LoadAsync();

        return presenter;
    }
}
