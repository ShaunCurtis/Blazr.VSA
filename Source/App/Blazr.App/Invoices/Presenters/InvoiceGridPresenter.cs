/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Gallium;

namespace Blazr.App.Presentation.Bootstrap;

public class InvoiceGridPresenter : GridPresenter<DmoInvoice>
{
    public InvoiceGridPresenter(
        IMediator mediator, 
        IMessageBus messageBus, 
        ScopedStateProvider keyedFluxGateStore)
        : base(mediator, messageBus, keyedFluxGateStore)
    { }

    protected override async Task<Result<ListResult<DmoInvoice>>> GetItemsAsync(GridState<DmoInvoice> state)
    {
        // Get the list request from the Flux Context and get the result
        var listRequest = new InvoiceRequests.InvoiceListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        };

        var result = await _dataBroker.Send(listRequest);

        return result;
    }
}
