# Aggregate Entities in Functional Programing

Updating an object where the consequences of the change are limited to the object being changed is simple.  Changing an object with business rules that encompass other objects is complex.  In OOP the aggregate pattern sets out to address this problem.  

> An aggregate entity is a group of objects bound by one or more application rules.  The purpose of the aggregate is to ensure those rules are applied, and cannot be broken.  
 
The pattern treats the aggregate as a black box.  All changes are submitted to the black box, not the individual objects within it.  The black box applies the changes and runs the logic to ensure consistency.

> An aggregate only has purpose in a mutation context: you don't need aggregates to list or display data.  

An invoice is a good example of an aggregate. Delete a line item, and the aggregate needs to track the deletion of the item, calculate the new total amount and update the invoice.  Persist the aggregate to the data store, and the aggregate needs to hold the necessary state information to apply the appropriate update/add/delete actions as a *Unit of Work* to the data store.

## The Problems with Aggregates

Coding aggregates is not plain sailing.  It's easy to slip the boundary, include more related objects.  Complex aggregate entities quickly grow  into a monster black box class.

## The Functional Challenge

In Functional programming, all data objects are immutable.  Our invoice entity is a container.  Dealing with container immutability is a challenge.

### Invoice Entity

Our first step is to define the container and it's data objects.

```csharp
public sealed record InvoiceEntity
{
    public DmoInvoice InvoiceRecord { get; private init; }
    public ImmutableList<DmoInvoiceItem> InvoiceItems { get; private init; }
}
```

Everything is immutable, and the `inits` are private.

Next the constructor.  It's private: we'll use static constructor methods to control creation.

```csharp
private InvoiceEntity(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceInvoiceItems)
{
    this.InvoiceRecord = invoice;
    this.InvoiceItems = invoiceInvoiceItems.ToImmutableList();
}
```

And the constructors:

```csharp
public static InvoiceEntity CreateNewEntity() =>
    new InvoiceEntity(DmoInvoice.Create(), Enumerable.Empty<DmoInvoiceItem>());

public static Result<InvoiceEntity> CreateWithRulesValidation(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) =>
    CheckEntityRules(new InvoiceEntity(invoice, invoiceItems));

public static Result<InvoiceEntity> CreateWithEntityRulesApplied(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) =>
    ApplyEntityRules(new InvoiceEntity(invoice, invoiceItems));

private static InvoiceEntity Create(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) =>
    new InvoiceEntity(invoice, invoiceItems);
```

We need to define our business rules as static functions.  There's both *Check* and *Apply* versions.  Note the *pure* nature of these functions.The *Check* either returns a Result in error state, or the original entity.  The *Apply* function creates and returns a new `InvoiceEntity`. 

```csharp
public static Result<InvoiceEntity> CheckEntityRules(InvoiceEntity entity)
    => entity.InvoiceItems.Sum(item => item.Amount.Value) == entity.InvoiceRecord.TotalAmount.Value
        ? Result<InvoiceEntity>.Success(entity)
        : Result<InvoiceEntity>.Failure("The Invoice Total Amount is incorrect.");

public static Result<InvoiceEntity> ApplyEntityRules(InvoiceEntity entity)
    => InvoiceEntity.Create(
        invoice: entity.InvoiceRecord with { TotalAmount = new(entity.InvoiceItems.Sum(item => item.Amount.Value)) },
        invoiceItems: entity.InvoiceItems)
    .ToResult();
```

Finally two utility methods.

```csharp
public bool IsDirty(InvoiceEntity control)
    => !this.Equals(control);

public Result<InvoiceEntity> ToResult()
    => Result<InvoiceEntity>.Create(this);
```

### Invoice Mutor

Another immutable object.

```csharp
public sealed record InvoiceMutor
{
    private readonly InvoiceEntity BaseEntity;
    
    public InvoiceEntity CurrentEntity { get; private init; }
    public bool IsNew { get; private init; }
}
```

With private constructors

```csharp
private InvoiceMutor(InvoiceEntity baseEntity)
{
    this.BaseEntity = baseEntity;
    this.CurrentEntity = baseEntity;
}

private InvoiceMutor(InvoiceEntity mutatedEntity, InvoiceEntity baseEntity)
{
    this.BaseEntity = baseEntity;
    this.CurrentEntity = mutatedEntity;
}
```

And static constructors.  Note `CreateMutation` is used to create a mutated copy of the mutor that preserves the intitial state.

```csharp
public static InvoiceMutor CreateNew()
    => new InvoiceMutor(InvoiceEntity.CreateNewEntity()) { IsNew = true };

public static InvoiceMutor Create(InvoiceEntity baseEntity)
    => new InvoiceMutor(baseEntity);

private static InvoiceMutor CreateMutation(InvoiceEntity mutatedEntity, InvoiceEntity baseEntity)
    => new InvoiceMutor(mutatedEntity, baseEntity);
```

Some specific mutate object methods

```csharp
public Result<InvoiceMutor> Mutate(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
    => InvoiceEntity.CreateWithEntityRulesApplied(invoice, invoiceItems)
        .ExecuteTransform(entity => InvoiceMutor.CreateMutation(entity, this.BaseEntity).ToResult);

public Result<InvoiceMutor> Mutate(DmoInvoice invoice)
    => InvoiceEntity.CreateWithEntityRulesApplied(invoice, this.CurrentEntity.InvoiceItems)
        .ExecuteTransform(entity => InvoiceMutor.CreateMutation(entity, this.BaseEntity).ToResult);

public Result<InvoiceMutor> Mutate(IEnumerable<DmoInvoiceItem> invoiceItems)
    => InvoiceEntity.CreateWithEntityRulesApplied(this.CurrentEntity.InvoiceRecord, invoiceItems)
        .ExecuteTransform(entity => InvoiceMutor.CreateMutation(entity, this.BaseEntity).ToResult);
```

And finally the utility properties.

```csharp
    public InvoiceId Id => BaseEntity.InvoiceRecord.Id;

    public Result<InvoiceMutor> ToResult => Result<InvoiceMutor>.Create(this);

    public bool IsDirty => this.CurrentEntity.IsDirty(BaseEntity);

    public EditState State => this.IsNew 
        ? EditState.New 
        : IsDirty 
            ? EditState.Dirty 
            : EditState.Clean;
```

## Usage

The simplest way to look at usage is through *Tests*.

Here we'll look in detail at `UpdateAnInvoiceItem`.

First call the helpers to get the DI service container, mediator service and a sample Mutor from the database.  *You can see these in detail in the Repo*.

```csharp
var provider = GetServiceProvider();
var mediator = provider.GetRequiredService<IMediatorBroker>()!;
var mutor = await this.GetASampleMutorAsync(mediator);
```

```csharp
var itemToUpdate = mutor.CurrentEntity.InvoiceItems.First() with { Amount = new(59) };
```

```csharp
```
```csharp
```