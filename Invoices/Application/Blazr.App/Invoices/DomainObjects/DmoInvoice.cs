namespace Blazr.App.Invoice.Core;

public sealed record DmoInvoice
{
    public InvoiceId Id { get; init; } = InvoiceId.Default;
    public CustomerId CustomerId { get; init; } = CustomerId.Default;
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public Date Date { get; init; }
}
