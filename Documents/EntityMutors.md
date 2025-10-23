# Mutors

**Mutor** is a word I *invented* to describe a group of patterns for creating an edit context for immutable objects.

## The Entity Mutor

Consider the `Invoice` entity.

```csharp
public record InvoiceEntity
{
    public DmoInvoice Record { get; private init; }
    public ImmutableList<DmoInvoiceItem> Items { get; private init; }

    public InvoiceEntity(DmoInvoice record, IEnumerable<DmoInvoiceItem> items)
    {
        this.Record = record;
        this.Items = items.ToImmutableList();
    }
    
    public static InvoiceEntity Create()
        => new InvoiceEntity(new DmoInvoice() { Id = InvoiceId.Create}, Enumerable.Empty<DmoInvoiceItem>());

    public static InvoiceEntity Create(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
        => new InvoiceEntity(invoice, invoiceItems);

}
```
