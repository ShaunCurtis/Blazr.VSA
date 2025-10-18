/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public record InvoiceListRequest
    : BaseListRequest, IRequest<Result<ListItemsProvider<DmoInvoice>>>
{
    public static Result<InvoiceListRequest> Create(GridState<DmoInvoice> state)
        => Result<InvoiceListRequest>.Create(new InvoiceListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });
}
