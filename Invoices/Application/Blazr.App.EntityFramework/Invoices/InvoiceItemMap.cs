/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Invoice.Core;

namespace Blazr.App.Invoice.Infrastructure;

public sealed class InvoiceItemMap
{
    public static DmoInvoiceItem Map(DboInvoiceItem item)
       => new()
       {
           Id = new(item.InvoiceItemID),
           InvoiceId = new(item.InvoiceID),
           Amount = new(item.Amount),
           Description = new(item.Description),
       };

    public static DboInvoiceItem Map(DmoInvoiceItem item)
        => new()
        {
            InvoiceItemID = item.Id.Value,
            InvoiceID = item.InvoiceId.Value,
            Amount = item.Amount.Value,
            Description = item.Description.Value
        };
}

public static class InvoiceItemMapExtensions
{
    public static DmoInvoiceItem ToDmo(this DboInvoiceItem item)
        => InvoiceItemMap.Map(item);
    public static DboInvoiceItem ToDbo(this DmoInvoiceItem item)
        => InvoiceItemMap.Map(item);
}