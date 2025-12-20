/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Infrastructure;
using Blazr.App.Invoice.Core;

namespace Blazr.App.Invoice.Infrastructure;

public sealed class InvoiceItemMap
{
     public static DmoInvoiceItem Map(DboInvoiceItem item)
        => new()
        {
            Id = new(item.InvoiceItemID),
            InvoiceId = new(item.InvoiceID),
            Amount = item.Amount,
            Description = item.Description,
        };

    public static DboInvoiceItem Map(DmoInvoiceItem item)
        => new()
        {
            InvoiceItemID = item.Id.Value,
            InvoiceID = item.InvoiceId.Value,
            Amount = item.Amount,
            Description = item.Description
        };
}
