/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public record CustomerListRequest : BaseListRequest, IRequest<Return<ListItemsProvider<DmoCustomer>>>
{
    public static Return<CustomerListRequest> FromGridState(GridState<DmoCustomer> state)
        => Return<CustomerListRequest>.Read(new CustomerListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });
}
