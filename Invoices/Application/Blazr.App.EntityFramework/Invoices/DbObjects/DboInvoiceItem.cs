/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.ComponentModel.DataAnnotations;

namespace Blazr.App.EntityFramework;

public sealed record DboInvoiceItem
{
    [Key] public Guid InvoiceItemID { get; init; }
    public Guid InvoiceID { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }

    public static DboInvoiceItem Map(DmoInvoiceItem item)
        => new()
        {
            InvoiceItemID = item.Id.Value,
            InvoiceID = item.InvoiceId.Value,
            Amount = item.Amount.Value,
            Description = item.Description.Value
        };
}
