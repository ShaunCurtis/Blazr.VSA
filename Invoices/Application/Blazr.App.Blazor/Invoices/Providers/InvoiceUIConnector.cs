/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Presentation;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.App.Presentation;

public class InvoiceUIConnector
   : IUIConnector<DmoInvoice, InvoiceId>
{
    private readonly IMediatorBroker _mediator;

    public string SingleDisplayName { get; } = "Invoice";
    public string PluralDisplayName { get; } = "Invoices";
    public Type? EditForm { get; } = null;
    public Type? ViewForm { get; } = null;
    public string Url { get; } = "/Invoice";

    public InvoiceUIConnector(IMediatorBroker mediator)
    {
        _mediator = mediator;
    }

    public Func<InvoiceId, Task<Return<DmoInvoice>>> RecordRequestAsync
        => throw new NotImplementedException();

    public Func<StateRecord<DmoInvoice>, Task<Return<InvoiceId>>> RecordCommandAsync
        => throw new NotImplementedException();

    public Task<Return<GridItemsProviderResult<DmoInvoice>>> GetItemsAsync(GridState<DmoInvoice> state)
        => InvoiceListRequest.Create(state)
            .BindAsync((request) => _mediator.DispatchAsync(request))
            .MapAsync((itemsProvider) => GridItemsProviderResult
                    .From<DmoInvoice>(itemsProvider.Items
                    .ToList(),
                itemsProvider.TotalCount));

    public IRecordMutor<DmoInvoice> GetRecordMutor(DmoInvoice record)
        => throw new NotImplementedException();
}
