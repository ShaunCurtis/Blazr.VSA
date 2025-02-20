/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Infrastructure;

namespace Blazr.App.Infrastructure;

public sealed class DboInvoiceItemMap : IDboEntityMap<DboInvoiceItem, DmoInvoiceItem>
{
    public DmoInvoiceItem MapTo(DboInvoiceItem item)
        => Map(item);

    public DboInvoiceItem MapTo(DmoInvoiceItem item)
        => Map(item);

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
