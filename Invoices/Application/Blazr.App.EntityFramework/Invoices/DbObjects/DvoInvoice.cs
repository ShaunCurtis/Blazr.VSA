/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.ComponentModel.DataAnnotations;

namespace Blazr.App.EntityFramework;

public sealed record DvoInvoice
{
    [Key] public Guid InvoiceID { get; init; }
    public Guid CustomerID { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime Date { get; init; }

    public static DmoInvoice Map(DvoInvoice item)
    => new()
    {
        Id = new(item.InvoiceID),
        Customer = new(new(item.CustomerID), new(item.CustomerName)),
        TotalAmount = new(item.TotalAmount),
        Date = new(item.Date)
    };

    public static Result<DmoInvoice> MapToResult(DvoInvoice item)
        => Result<DmoInvoice>.Create(Map(item));
}
