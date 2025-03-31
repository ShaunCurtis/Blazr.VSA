# Aggregates

Updating an object where the consequences of the change are limited to the object is simple.  Making changes to an object that affect business rules that apply to another object is complex.  The aggregate pattern addresses that problem.  

> An aggregate is a group of objects bound by one or more application rules.  The purpose of the aggregate is to ensure those rules are applied, and cannot be broken.  
 
An aggregate is a black box.  All changes are submitted to the black box, not the individual objects within it.  The black box applies the changes and runs the logic to ensure consistency of the entities within the box.

Be aware, an aggregate only has purpose in a mutation context: you don't need aggregates to list or display data.  

In an invoice, delete a line item, and the aggregate needs to track the deletion of the item, calculate the new total amount and updates the invoice.  Persist the aggregate to the data store, and the aggregate needs to hold the necessary state information to apply the appropriate update/add/delete actions as a *Unit of Work* to the data store.

## The Classic Invoice Example

The rest of this article uses a simple Invoice as a working example. 

### Domain Entities

The domain entities [Dmo equals *Domain Object*] are:

```csharp
public sealed record DmoCustomer
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

### Data Store Objects

The corresponding data objects are: [*Dbo* equals *Database Table Object*]

```csharp
public sealed record DboCustomer : ICommandEntity
{
    [Key] public Guid CustomerID { get; init; }
    public string CustomerName { get; init; } = string.Empty;
}
```

```csharp
public sealed record DboInvoice 
{
    [Key] public Guid InvoiceID { get; init; }
    public Guid CustomerID { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime Date { get; init; }
}
```

```csharp
public sealed record DboInvoiceItem
{
    [Key] public Guid InvoiceItemID { get; init; }
    public Guid InvoiceID { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
```

### Mapping

Mappers are used to map data between data and domain objects.  Here's the Customer Mapper as an example:

```csharp
public sealed class DboCustomerMap : IDboEntityMap<DboCustomer, DmoCustomer>
{
    public DmoCustomer MapTo(DboCustomer item)
        => Map(item);

    public DboCustomer MapTo(DmoCustomer item)
        => Map(item);

    public static DmoCustomer Map(DboCustomer item)
        => new()
        {
            Id = new(item.CustomerID),
            CustomerName = new(item.CustomerName),
        };

    public static DboCustomer Map(DmoCustomer item)
        => new()
        {
            CustomerID = item.Id.Value,
            CustomerName = item.CustomerName
        };
}
```

## The Composite

I'm no fan of the standard aggregate pattern, where the aggregate route entity is the aggregate.  It breaks the *Single Responsibility Principle*. The aggregate root two responsibilities: maintaining the root data and applying the aggregate business rules.

Instead, I use a *Composite* fascade.  The *aggreagate root* is an object within the composite.

The public interface of the invoice composite looks like this:

```csharp
public sealed partial class InvoiceComposite
{
    public InvoiceRecord InvoiceRecord { get;}
    public IEnumerable<InvoiceItemRecord> InvoiceItems { get;}
    public IEnumerable<InvoiceItemRecord> InvoiceItemsBin { get;}
    public bool IsDirty { get;}

    public event EventHandler<InvoiceId>? StateHasChanged;

    public InvoiceComposite(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> items);

    public static InvoiceComposite Default { get;}
}
```

Points:

1. The class is partial.  We add mutation functionality through partial class files.
2. The invoice and invoice items are exposed as an `InvoiceRecord` and `InvoiceItemRecords`.  Immutable objects that contain the entity object and state.

### Mutable Data and State

The `Invoice` is declared as a private object within the composite:

```csharp
    private readonly Invoice Invoice;
```

And looks like this:

```csharp
internal sealed class Invoice
{
    public CommandState State { get; set; }
        = CommandState.None;

    public DmoInvoice Record { get; private set; }

    public bool IsDirty
        => this.State != CommandState.None;

    public InvoiceRecord AsRecord(List<InvoiceItemRecord> items)
        => new(this.Record, items, this.State);

    public Invoice(DmoInvoice item, bool isNew = false)
    {
        this.Record = item;

        if (isNew || item.Id.IsDefault)
            this.State = CommandState.Add;
    }

    public InvoiceId Id => this.Record.Id;

    public void Update(DmoInvoice invoice)
    {
        this.Record = invoice;
        this.State = this.State.AsDirty;
    }
}
```

And the `InvoiceRecord` looks like this:

```csharp
public record InvoiceRecord(DmoInvoice Record, IEnumerable<InvoiceItemRecord> Items, CommandState State)
{
    public bool IsDirty
        => this.State != CommandState.None;
}
```

The point here is the mutable `Invoice` is a `private`:  internal to the composite object.  It is based on the following pattern:

```
internal sealed class Entity
{
    public CommandState State { get; set; }
    public DmoEntity Record { get; private set; }
    public bool IsDirty { get;}
    public EntityRecord AsRecord(List<EntityItemRecord> items) { get;}

    public Entity(DmoEntity item, bool isNew = false);

    public void Update(DmoEntity entity);
}
```

The `InvoiceItem` and `InvoiceItemRecord` follow the same pattern.

## Managing Mutation

Mutation is managed in a pattern based on the Flux pattern.

Adding an invoice item demonstrates the pattern:

```csharp

public static partial class InvoiceActions
{
    public readonly record struct AddInvoiceItemAction(DmoInvoiceItem Item, bool IsNew = true);
}

public sealed partial class InvoiceComposite
{
    public Result Dispatch(AddInvoiceItemAction action)
    {
        if (_items.Any(item => item.Record == action.Item))
            return Result.Fail(new ActionException($"The Invoice Item with Id: {action.Item.Id} already exists in the Invoice."));

        var invoiceItemRecord = action.Item with { InvoiceId = this.InvoiceRecord.Record.Id };
        var invoiceItem = new InvoiceItem(invoiceItemRecord, action.IsNew);
        _items.Add(invoiceItem);
        this.Process();

        return Result.Success();
    }
}
```

We define a simple `AddInvoiceItemAction` value object which is passed to the Composite *Dispatcher*.  The dispatcher method is declared in a partial Composite class.

All the actions and dispatchers follow the same pattern.

The *Actions* are declared within an `InvoiceActions` static class to keep them together and simplify naming. 

Each `Action` and it's *Dispatcher* are in the same code file.

## The Boundary Decision

What entities belong within the aggregate is a difficult decision.  It's very easy to add too much.

In our classic example we have `Invoice`, `InvoiceItem` and `Customer` objects.  They are all related, so do all three belong with the composite?

An `InvoiceItem` is intrinsically linked to an invoice.  It has no context outside the `Invoice`.  Changing the `Amount` on an `InvoiceItem` changes the `TotalAmount` in the `Invoice`.

On the other hand a `Customer` is a stand alone item.  Changing data on the invoice doesn't affect the integrity of the `Customer` object.  It doesn't belong inside the aggregate/composite.
