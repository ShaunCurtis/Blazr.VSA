/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Blazr.Cadmium.Presentation;
using Blazr.Cadmium.QuickGrid;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.UI;

public sealed record InvoiceUIEntityProvider : IUIEntityProvider<DmoInvoice, InvoiceId>
{
    private readonly IServiceProvider _serviceProvider;

    public string SingleDisplayName { get; } = "Invoice";
    public string PluralDisplayName { get; } = "Invoices";
    public Type? EditForm { get; } = null;
    public Type? ViewForm { get; } = null;
    public string Url { get; } = "/Invoice";

    public InvoiceUIEntityProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IReadUIBroker<DmoInvoice, InvoiceId>> GetReadUIBrokerAsync(InvoiceId id)
    {
        var presenter = ActivatorUtilities.CreateInstance<ReadUIBroker<DmoInvoice, InvoiceId>>(_serviceProvider);
        await presenter.LoadAsync(id);
        return presenter;
    }

    public ValueTask<IGridUIBroker<DmoInvoice>> GetGridUIBrokerAsync()
    {
        var presenter = ActivatorUtilities.CreateInstance<GridUIBroker<DmoInvoice, InvoiceId>>(_serviceProvider);
        return ValueTask.FromResult<IGridUIBroker<DmoInvoice>>(presenter);
    }

    public async ValueTask<IEditUIBroker<TEditContext, InvoiceId>> GetEditUIBrokerAsync<TEditContext>(InvoiceId id)
        where TEditContext : IRecordEditContext<DmoInvoice>, new()
    {
        var presenter = ActivatorUtilities.CreateInstance<EditUIBroker<DmoInvoice, TEditContext, InvoiceId>>(_serviceProvider);
        await presenter.LoadAsync(id);
        return presenter;
    }

    public ValueTask<IGridUIBroker<DmoInvoice>> GetGridUIBrokerAsync(Guid contextId)
        => throw new NotImplementedException();

    public ValueTask<IGridUIBroker<DmoInvoice>> GetGridUIBrokerAsync(Guid contextId, UpdateGridRequest<DmoInvoice> resetGridRequest)
        => throw new NotImplementedException();
}
