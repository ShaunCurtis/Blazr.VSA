/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public record InvoiceListRequest
    : BaseListRequest, IRequest<Return<ListItemsProvider<DmoInvoice>>>
{
    public static Return<InvoiceListRequest> FromGridState(GridState<DmoInvoice> state)
        => Return<InvoiceListRequest>.Read(new InvoiceListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });
}

