/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Presentation;

public sealed class InvoiceItemEditPresenterFactory
{
    private IServiceProvider _serviceProvider;
    public InvoiceItemEditPresenterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public InvoiceItemEditPresenter GetPresenter(InvoiceItemId invoiceItemId)
    {
        var presenter = ActivatorUtilities.CreateInstance<InvoiceItemEditPresenter>(_serviceProvider, new object[] {invoiceItemId });
        ArgumentNullException.ThrowIfNull(presenter, nameof(presenter));
        return presenter;
    }
}


