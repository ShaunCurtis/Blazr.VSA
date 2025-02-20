/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Gallium;

namespace Blazr.App.Presentation.Bootstrap;

public class CustomerGridPresenter : GridPresenter<DmoCustomer>
{
    public CustomerGridPresenter(
        IMediator mediator,
        IMessageBus messageBus, 
        ScopedStateProvider keyedFluxGateStore)
        : base(mediator, messageBus, keyedFluxGateStore)
    { }

    protected override async Task<Result<ListResult<DmoCustomer>>> GetItemsAsync(GridState<DmoCustomer> state)
    {
        // Get the list request from the Flux Context and get the result
        var listRequest = new CustomerListRequest()
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
