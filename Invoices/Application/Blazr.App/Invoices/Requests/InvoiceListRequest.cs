/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public record InvoiceListRequest
    : BaseListRequest, IRequest<Bool<ListItemsProvider<DmoInvoice>>>
{
    public static Bool<InvoiceListRequest> Create(GridState<DmoInvoice> state)
        => Bool<InvoiceListRequest>.Input(new InvoiceListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });
}
