/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Infrastructure;
using Blazr.App.Invoice.Core;

namespace Blazr.App.Invoice.Infrastructure;

public sealed class InvoiceMap
{
    public static DmoInvoice Map(DvoInvoice item)
        => new()
        {
            Id = new(item.InvoiceID),
            CustomerId = new(item.CustomerID),
            CustomerName = item.CustomerName,
            TotalAmount = item.TotalAmount,
            Date = DateOnly.FromDateTime(item.Date)
        };

    public static DboInvoice Map(DmoInvoice item)
        => new()
        {
            InvoiceID = item.Id.Value,
            CustomerID = item.CustomerId.Value,
            TotalAmount = item.TotalAmount,
            Date = item.Date.ToDateTime( TimeOnly.MinValue)
        };
}
