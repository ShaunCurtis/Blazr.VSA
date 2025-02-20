# Aggregates

Updating an object where the consequences of the change are limited to the object is simple.  

> An aggregate is a set of objects bound by a set of application rules.  The purpose of the aggregate is to apply those rules to maintain application consisistency.  
 

Aggregates are a fundimental building block of applications that address this problem. 

> An aggregate is a group of objects bound by one or more application rules.  The aggregate ensures application consisistency.  
 
Consider an aggregate a black box.  All changes are applied to the black box, not the individual objects within it.  The black box contains the logic to ensure application consistency.

An aggregate is a black box.  All changes are applied to the black box, not the individual objects within it.  The black box contains the logic to ensure application consistency.  An aggregate only has purpose when you change an object to which those rules apply.  

Delete a line item through the aggregate, and it tracks the deletion of the item, calculates the new total amount and updates the invoice.  Persist the aggregate and it provides the persistance layer update/add/delete information to apply the changes as a *Unit of Work* to the data store.

The aggregate provides the invoice and line items as read only objects.  No modifications allowed.

## The Classic Invoice Example

The rest of this article uses a simple Invoice as a working example. 

The invoice objects are minimal: keep things simple.  The entity objects we use are: [*Dmo* equals *Domain Object*]

```csharp
public class InvoiceAggregate
{
    public int InvoiceID {get; set;}
    //...
    public ReadOnlyList<InvoiceItems> Items {get; private set;}

    //...  methods to change items
}
```

The Invoice is the aggregate root.  I'll discuss why I don't like this design later.

## The Classic Invoice Example

The rest of this article uses the Invoice example. The objects are minimal to keep things simple.

```csharp
public sealed record DmoCustomer : ICommandEntity
{
    public CustomerId Id { get; init; } = CustomerId.Default;
    public string CustomerName { get; init; } = string.Empty;
}
```

```csharp
public sealed record DmoInvoice
{
    public InvoiceId Id { get; init; } = InvoiceId.Default;
    public CustomerId CustomerId { get; init; } = CustomerId.Default;
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateOnly Date { get; init; }
}
```
 
```csharp
public sealed record DmoInvoiceItem
{
    public InvoiceItemId Id { get; init; } = InvoiceItemId.Default;
    public InvoiceId InvoiceId { get; init; } = InvoiceId.Default;
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
```

## The Composite

A composite is a wrapper.  The invoice [the aggreagate root] is an object within the composite. 

```csharp
public sealed class Item : IDisposable
{
    public Invoice Invoice {get;}
    public IEnumerable<InvoiceItems> Items {get;}
}
```

`UpdateCallback` provides a callback into the parent object to notify it of a change, and thus the need to apply the aggregate rules.

### Managing Mutation

Change is managed within the composite using `Blazor.Flux` which is a simple indexed *Flux* pattern library.

The `InvoiceWrapper` is the aggregate class.  

```csharp
public class InvoiceWrapper
{
}
```

The `DmoInvoice` is held within the `Invoice` state context and exposed as an InvoiceRecord. 

```csharp
public class Invoice
{
    private readonly Invoice Invoice;

    public InvoiceRecord InvoiceRecord => this.Invoice.AsRecord;
```

The `DmoInvoiceItem` collection is held within and internal list and exposed as an `IEnumerable`.  The bin contains invoice items that have been marked for deletion [and can be recycled].


```csharp
public class Invoice
{
    private readonly List<InvoiceItem> Items = new List<InvoiceItem>();
    private readonly List<InvoiceItem> ItemsBin = new List<InvoiceItem>();

    public IEnumerable<InvoiceItemRecord> InvoiceItems => this.Items.Select(item => item.AsRecord).AsEnumerable();
    public IEnumerable<InvoiceItemRecord> InvoiceItemsBin => this.ItemsBin.Select(item => item.AsRecord).AsEnumerable();
```

## Managing Mutation

The aggregate provides a *Flux* based implementation to manage mutation.

Each mutation is defined by an action, and applied by calling a dispatcher on the aggregate.

To update the `DmoInvoice` create a `UpdateInvoiceAction`. 

```csharp
    public readonly record struct UpdateInvoiceAction(DmoInvoice Item);
```

And call `Dispatch`

```csharp
    public Result Dispatch(UpdateInvoiceAction action)
    {
        this.Invoice.Update(action.Item);
        return Result.Success();
    }
```

This replaces `InvoiceRecord` and then invokes `UpdateCallback` which triggers the aggregate business logic:

```csharp
    private void Process()
    {
        // prevent calling oneself
        if (_processing)
            return;

        _processing = true;
        decimal total = 0m;
        foreach (var item in Items)
            total += item.Amount;

        if (total != this.InvoiceRecord.Record.TotalAmount)
        {
            this.Invoice.Update(this.InvoiceRecord.Record with { TotalAmount = total });
        }
        this.StateHasChanged?.Invoke(this, this.InvoiceRecord.Record.Id);

        _processing = false;
    }
```


## The Boundary Decision

The most difficult decision to make in designing aggregates is the boundary.  What objects are within and outside the wrapper.  It's very easy to add too much.

In our classic example we have `Invoice`, `InvoiceItem` and `Customer` objects.  They are all related, so do all three belong with the composite?

An `InvoiceItem` is intrinsically linked to an invoice.  It has no context outside the `Invoice`.  Changing the `Amount` on an `InvoiceItem` changes the `TotalAmount` in the `Invoice`.

On the other hand a `Customer` is a stand alone item.  Changing data on the invoice doesn't affect the integrity of the `Customer` object.  It doesn't belong inside the aggregate/composite.

## The Aggregate Root

It's very easy to fall into the trap of making the Invoice the aggregate root.  In the classic implementation that's exactly what's done.  `InvoiceID`, `InvoiceDate`, `InvoiceAmount` all become properties of the aggregate.

I consider this a breach of the *Single Responsibility Principle*.  The aggregate root two responsibilities: maintaining the root data and applying the business rules to the whole aggregate.

The aggregate is the fascade for applying consistency/business rules to the objects within the aggregate boundary.  In my aggregates, the invoice is just another object within the aggregate.