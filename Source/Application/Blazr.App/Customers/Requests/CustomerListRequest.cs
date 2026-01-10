/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public record CustomerListRequest : BaseListRequest, IRequest<Result<ListItemsProvider<DmoCustomer>>>
{
    public static Result<CustomerListRequest> FromGridState(GridState<DmoCustomer> state)
        => Result<CustomerListRequest>.Successful(new CustomerListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });
}
