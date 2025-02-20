/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.ComponentModel.DataAnnotations;

namespace Blazr.App.Core;

public sealed record DboInvoice 
{
    [Key] public Guid InvoiceID { get; init; }
    public Guid CustomerID { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime Date { get; init; }
}
