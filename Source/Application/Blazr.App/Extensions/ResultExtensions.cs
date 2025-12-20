/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class ResultExtensions
{
    public static Return<DmoCustomer> ToResult(this DmoCustomer item)
        => Return<DmoCustomer>.Success(item);

    public static Return<DmoInvoice> ToResult(this DmoInvoice item)
        => Return<DmoInvoice>.Success(item);

    public static Return<DmoInvoiceItem> ToResult(this DmoInvoiceItem item)
        => Return<DmoInvoiceItem>.Success(item);
}
