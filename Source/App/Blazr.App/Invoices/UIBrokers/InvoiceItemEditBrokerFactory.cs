/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Presentation;

public sealed class InvoiceItemEditBrokerFactory
{
    private IServiceProvider _serviceProvider;
    public InvoiceItemEditBrokerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public InvoiceItemEditBroker GetPresenter(InvoiceItemId invoiceItemId)
    {
        var presenter = ActivatorUtilities.CreateInstance<InvoiceItemEditBroker>(_serviceProvider, new object[] {invoiceItemId });
        ArgumentNullException.ThrowIfNull(presenter, nameof(presenter));
        return presenter;
    }
}


