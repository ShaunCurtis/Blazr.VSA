/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class ResultExtensions
{
    public static Result<DmoCustomer> ToResult(this DmoCustomer item)
        => Result<DmoCustomer>.Success(item);

    public static Result<DmoInvoice> ToResult(this DmoInvoice item)
        => Result<DmoInvoice>.Success(item);

    public static Result<DmoInvoiceItem> ToResult(this DmoInvoiceItem item)
        => Result<DmoInvoiceItem>.Success(item);
}
