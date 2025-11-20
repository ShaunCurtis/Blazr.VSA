/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public record CustomerListRequest
    : BaseListRequest, IRequest<Bool<ListItemsProvider<DmoCustomer>>>
{
    public static Bool<CustomerListRequest> Create(GridState<DmoCustomer> state)
        => Bool<CustomerListRequest>.Input(new CustomerListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });
}
