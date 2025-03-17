/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Gallium;

namespace Blazr.App.Presentation.Bootstrap;

public class CustomerGridBroker : GridUIBroker<DmoCustomer>
{
    public CustomerGridBroker( IMediator mediator, IMessageBus messageBus, ScopedStateProvider keyedFluxGateStore)
        : base(mediator, messageBus, keyedFluxGateStore)
    { }

    protected override async Task<Result<ListItemsProvider<DmoCustomer>>> GetItemsAsync(GridState<DmoCustomer> state)
    {
        // Creates a Mediator CustomerListRequest Request
        var listRequest = new CustomerListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        };

        // Sends the request to the Mediator and returns the result
        var result = await _dataBroker.Send(listRequest);
        return result;
    }
}
