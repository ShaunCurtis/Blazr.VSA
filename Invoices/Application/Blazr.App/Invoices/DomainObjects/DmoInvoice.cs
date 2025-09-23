namespace Blazr.App.Core;

public sealed record DmoInvoice
{
    public InvoiceId Id { get; init; } = InvoiceId.Default;
    public CustomerId CustomerId { get; init; } = CustomerId.Default;
    public Title CustomerName { get; init; } = Title.Default;
    public Money TotalAmount { get; init; } = Money.Default;
    public Date Date { get; init; }
}
