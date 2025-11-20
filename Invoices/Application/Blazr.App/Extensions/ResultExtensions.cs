/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class ResultExtensions
{
    public static Bool<DmoCustomer> ToResult(this DmoCustomer item)
        => Bool<DmoCustomer>.Success(item);

    public static Bool<DmoInvoice> ToResult(this DmoInvoice item)
        => Bool<DmoInvoice>.Success(item);

    public static Bool<DmoInvoiceItem> ToResult(this DmoInvoiceItem item)
        => Bool<DmoInvoiceItem>.Success(item);
}
