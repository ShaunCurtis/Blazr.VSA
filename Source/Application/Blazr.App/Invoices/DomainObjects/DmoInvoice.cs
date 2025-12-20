namespace Blazr.App.Core;

public sealed record DmoInvoice
{
    public InvoiceId Id { get; init; } = InvoiceId.Default;
    public FkoCustomer Customer { get; init; } =  FkoCustomer.Default;
    public Money TotalAmount { get; init; } = Money.Default;
    public Date Date { get; init; }

    public static DmoInvoice CreateNew()
        => new() { Id = InvoiceId.NewId, Date = new(DateTime.Now) };
}
