/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Mediator;
using Blazr.Antimony;
using Blazr.App.Core;

namespace Blazr.App.Invoice.Core;

public static class InvoiceRequests
{
    public readonly record struct InvoiceSaveRequest(InvoiceEntity Invoice) : IRequest<Result>;

    public readonly record struct InvoiceNewRequest() : IRequest<Result<InvoiceEntity>>;

    public readonly record struct InvoiceRequest(InvoiceId Id) : IRequest<Result<InvoiceEntity>>;

    public readonly record struct InvoiceRecordRequest(InvoiceId Id) : IRequest<Result<DmoInvoice>>;

    public record InvoiceListRequest : BaseListRequest, IRequest<Result<ListItemsProvider<DmoInvoice>>> { }
}