# Mutors

**Mutor** is a word I *invented* to describe a group of patterns for creating an edit context for immutable objects.

## The Immutable Entity

Consider the `Invoice` entity.

First the base domain objects:

```csharp
public sealed record DmoInvoice
{
    public InvoiceId Id { get; init; } = InvoiceId.Default;
    public InvoiceCustomer Customer { get; init; } =  InvoiceCustomer.Default;
    public Money TotalAmount { get; init; } = Money.Default;
    public Date Date { get; init; }

    public static DmoInvoice Create()
        => new() { Id = InvoiceId.Create, Date = new(DateTime.Now) };
}
```

And:

```csharp
public sealed record DmoInvoiceItem
{
    public InvoiceItemId Id { get; init; } = InvoiceItemId.Default;
    public InvoiceId InvoiceId { get; init; } = InvoiceId.Default;
    public Title Description { get; init; } = Title.Default;
    public Money Amount { get; init; }

    public static DmoInvoiceItem Create(InvoiceId invoiceId)
        => new() { Id = InvoiceItemId.Create, InvoiceId = invoiceId };
}
```

The base `InvoiceEntity`.  It's locked down.  Everything is immutable and the constructor is private.

```csharp
public sealed record InvoiceEntity
{
    public DmoInvoice InvoiceRecord { get; private init; }
    public ImmutableList<DmoInvoiceItem> InvoiceItems { get; private init; }

    private InvoiceEntity(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceInvoiceItems)
    {
        this.InvoiceRecord = invoice;
        this.InvoiceItems = invoiceInvoiceItems.ToImmutableList();
    }

    public bool IsDirty(InvoiceEntity control)
        => !this.Equals(control);

    public Result<InvoiceEntity> ToResult => Result<InvoiceEntity>.Create(this);
}
```

There are three static constructors.

```csharp
public static InvoiceEntity CreateNewEntity
    => new InvoiceEntity(DmoInvoice.Create(), Enumerable.Empty<DmoInvoiceItem>());

public static Result<InvoiceEntity> CreateWithRulesValidation(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceInvoiceItems)
    => CheckEntityRules(new InvoiceEntity(invoice, invoiceInvoiceItems));

public static Result<InvoiceEntity> CreateWithEntityRulesApplied(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceInvoiceItems)
    => ApplyEntityRules(new InvoiceEntity(invoice, invoiceInvoiceItems));
```

And the validation and Entity rules are defined as two static pure functions.

```csharp
public static Result<InvoiceEntity> CheckEntityRules(InvoiceEntity entity)
{
    var total = entity.InvoiceItems.Sum(item => item.Amount.Value);

    return entity.InvoiceRecord.TotalAmount.Value == total
        ? Result<InvoiceEntity>.Success(entity)
        : Result<InvoiceEntity>.Failure("The Invoice Total Amount is incorrect.");
}

public static Result<InvoiceEntity> ApplyEntityRules(InvoiceEntity entity)
{
    var total = entity.InvoiceItems.Sum(item => item.Amount.Value);

    if (entity.InvoiceRecord.TotalAmount.Value == total)
        return Result<InvoiceEntity>.Success(entity);

    var newInvoice = entity.InvoiceRecord with { TotalAmount = new(total) };

    return InvoiceEntity.Create(newInvoice, entity.InvoiceItems);
}
```

## The Mutor

Again locked down.  Everything is immutable and the constructor is private.

```csharp
public sealed record InvoiceMutor
{
    public InvoiceEntity BaseEntity { get; private init; }
    public InvoiceEntity MutatedEntity { get; private init; }

    public InvoiceId Id => BaseEntity.InvoiceRecord.Id;
    public Result<InvoiceMutor> ToResult => Result<InvoiceMutor>.Create(this);
    public bool IsDirty => this.MutatedEntity.IsDirty(BaseEntity);

    private InvoiceMutor(InvoiceEntity baseEntity)
    {
        this.BaseEntity = baseEntity;
        this.MutatedEntity = baseEntity;
    }

    private InvoiceMutor(InvoiceEntity mutatedEntity, InvoiceEntity baseEntity)
    {
        this.BaseEntity = baseEntity;
        this.MutatedEntity = mutatedEntity;
    }
}
```

You can either *Create* a Mutor:

```csharp
public static InvoiceMutor CreateNew()
    => Create(InvoiceEntity.CreateNewEntity);

public static InvoiceMutor Create(InvoiceEntity baseEntity)
    => new InvoiceMutor(baseEntity);

public static InvoiceMutor Create(InvoiceEntity mutatedEntity, InvoiceEntity baseEntity)
    => new InvoiceMutor(mutatedEntity, baseEntity);
```

Or *Mutate* an existing *Mutor*.

```csharp
public Result<InvoiceMutor> Mutate(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
    => InvoiceEntity.CreateWithEntityRulesApplied(invoice, invoiceItems)
        .ExecuteTransform(entity => InvoiceMutor.Create(entity, this.BaseEntity).ToResult);

public Result<InvoiceMutor> Mutate(DmoInvoice invoice)
    => InvoiceEntity.CreateWithEntityRulesApplied(invoice, this.MutatedEntity.InvoiceItems)
        .ExecuteTransform(entity => InvoiceMutor.Create(entity, this.BaseEntity).ToResult);

public Result<InvoiceMutor> Mutate(IEnumerable<DmoInvoiceItem> invoiceItems)
    => InvoiceEntity.CreateWithEntityRulesApplied(this.MutatedEntity.InvoiceRecord, invoiceItems)
        .ExecuteTransform(entity => InvoiceMutor.Create(entity, this.BaseEntity).ToResult);
```

The key functionality is the creation of a new copy of the *Mutor* with each mutation, but the maintenance of the initial base entity state with each copy.  As all the objects within the entity are *records*, you can use simple equality checking to detect if the current copy is dirty (compared with the original).