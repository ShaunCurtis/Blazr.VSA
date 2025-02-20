/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class InvoiceRequests
{
    public readonly record struct InvoiceSaveRequest(InvoiceWrapper Invoice) : IRequest<Result>;

    public readonly record struct InvoiceNewRequest() : IRequest<Result<InvoiceWrapper>>;

    public readonly record struct InvoiceRequest(InvoiceId Id) : IRequest<Result<InvoiceWrapper>>;

    public readonly record struct InvoiceRecordRequest(InvoiceId Id) : IRequest<Result<DmoInvoice>>;

    public record InvoiceListRequest : BaseListRequest, IRequest<Result<ListResult<DmoInvoice>>> { }
}