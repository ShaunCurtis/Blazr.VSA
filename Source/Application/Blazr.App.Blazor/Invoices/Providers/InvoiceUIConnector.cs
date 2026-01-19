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

    public Func<InvoiceId, Task<Result<DmoInvoice>>> RecordRequestAsync
        => throw new NotImplementedException();

    public Func<DmoInvoice, RecordState, Task<Result<InvoiceId>>> RecordCommandAsync
        => throw new NotImplementedException();

    public Task<Result<GridItemsProviderResult<DmoInvoice>>> GetItemsAsync(GridState<DmoInvoice> state)
        => ResultT.Successful(state)
            .Bind(InvoiceListRequest.FromGridState)
            .BindAsync((request) => _mediator.DispatchAsync(request))
            .MapAsync(itemsProvider => itemsProvider.ToGridItemsProviderResult());

    public IRecordMutor<DmoInvoice> GetRecordMutor(DmoInvoice record)
        => throw new NotImplementedException();
}
