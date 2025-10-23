namespace Blazr.App.Core;

public sealed record DmoInvoice
{
    public InvoiceId Id { get; init; } = InvoiceId.Default;
    public InvoiceCustomer Customer { get; init; } =  InvoiceCustomer.Default;
    public Money TotalAmount { get; init; } = Money.Default;
    public Date Date { get; init; }

    public static DmoInvoice Create()
        => new() { Id = InvoiceId.Create, Date = new(DateTime.Now) };
}
