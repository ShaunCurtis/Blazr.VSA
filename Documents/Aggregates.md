# Aggregate Entities in Functional Programing

Updating objects where the consequences of the change are limited to the object are simple.  Changing an object with business rules that encompass other objects is complex.  In OOP the aggregate pattern sets out to address this problem.  

> An aggregate entity is a group of objects bound by one or more application rules.  The purpose of the aggregate is to ensure those rules are applied, and cannot be broken.  
 
The OOP pattern treats the aggregate as a black box.  All changes are submitted to the black box, not the individual objects within it.  The black box applies the changes and runs the logic to ensure consistency.

A really important point to note is:

> An aggregate only makes sense in a mutation context: you don't need aggregates to list or display data.  

An invoice is a good example of an aggregate. Delete a line item, and the aggregate needs to track the deletion of the item, calculate the new total amount and update the invoice.  Persist the aggregate to the data store, and the aggregate needs to hold the necessary state information to apply the appropriate update/add/delete actions as a *Unit of Work* to the data store.

## The Problems with Aggregates

Conceptually coding aggregates seems plain sailing.  The problem is in the detail.  It's easy to slip the boundary, include more related objects.  Complex aggregate entities quickly grow: they become god classes.

## The Functional Challenge

At first glance the aggregate concept and Functional Programming's demand for immutability see at odds: how do you fit and square peg in a round hole?
Let's look at how we can overcome it.

### A Note Before We Start

Much of the code is written in *Functional Programming style*.  You will see a lot of small, sometimes one line functions.  Many are `static.  There's extensive use of extension methods to add *local* functionality to existing objects.  Chaining is common to *compose* more complex functions from small code snippets. 

### Invoice Entity

The first step is to define the container and it's data objects.

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

    internal static InvoiceEntity Read(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) 
        => new InvoiceEntity(invoice, invoiceItems);
}
```

Everything is immutable, and the `inits` and primary constructor are private.  There's a single static `Read` to initialize an instance. 

Invoice Entities are obtained from the static `InvoiceEntityFactory`.


```csharp
internal static InvoiceEntity Create();

internal static InvoiceEntity Create(DmoInvoice invoice);

public static Return<InvoiceEntity> TryLoad(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems);

public static Return<InvoiceEntity> Load(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems);

internal static Return<InvoiceEntity> CheckEntityRules(InvoiceEntity entity);

internal static InvoiceEntity ApplyEntityRules(InvoiceEntity entity);
```

Note the *Entity Rules*.

 - The first attempts to load an invoice entity from the specified invoice and its associated items, It returns a `Return` in failure state if validation fails.

 - The second creates an invoice entity from the specified invoice and its items, applies the business rules and updates the entity if necessary.

```csharp
internal static Return<InvoiceEntity> CheckEntityRules(InvoiceEntity entity)
    => entity.InvoiceItems.Sum(item => item.Amount.Value) == entity.InvoiceRecord.TotalAmount.Value
        ? Return<InvoiceEntity>.Success(entity)
        : Return<InvoiceEntity>.Failure("The Invoice Total Amount is incorrect.");

internal static InvoiceEntity ApplyEntityRules(InvoiceEntity entity)
    => InvoiceEntity.Read(
        invoice: entity.InvoiceRecord with { TotalAmount = new(entity.InvoiceItems.Sum(item => item.Amount.Value)) },
        invoiceItems: entity.InvoiceItems);
```

#### Mutation

Mutation happens by creating a new `InvoiceEntity` with the mutation applied.

Mutations are defined as extension methods on `InvoiceEntity` in a separate namespace *Blazr.App.Core.Invoices*.  

```csharp
extension(InvoiceEntity @this)
{
    internal Return<InvoiceEntity> AddInvoiceItem(DmoInvoiceItem item);

    internal Return<InvoiceEntity> DeleteInvoiceItem(DmoInvoiceItem record);

    internal Return<InvoiceEntity> ReplaceInvoiceItem(DmoInvoiceItem record);

    internal Return<InvoiceEntity> ReplaceInvoice(DmoInvoice record);

    internal Return<DmoInvoiceItem> GetInvoiceItem(InvoiceItemId id);

    private Return<DmoInvoiceItem> GetInvoiceItem(DmoInvoiceItem item);

    private Return<InvoiceEntity> MutateWithEntityRulesApplied(DmoInvoice invoice);

    private Return<InvoiceEntity> MutateWithEntityRulesApplied(IEnumerable<DmoInvoiceItem> invoiceItems) ;
}
```

And two utility methods.

```csharp
internal Return<InvoiceEntity> ToReturnT => Return<InvoiceEntity>.Read(@this);

internal bool IsDirty(InvoiceEntity control) => !@this.Equals(control);
```

### Invoice Mutor

A mutable object.

```csharp
public sealed class InvoiceEntityMutor
{
    private readonly IMediatorBroker _mediator;
    private readonly IMessageBus _messageBus;

    public InvoiceEntity BaseEntity { get; private set; }
    public InvoiceEntity InvoiceEntity { get; private set; }
    public Return LastResult { get; private set; } = Return.Success();
    public bool IsNew { get; private set; } = true;
    public Task LoadTask { get; private set; } = Task.CompletedTask;

    public InvoiceEntityMutor(IMediatorBroker mediator, IMessageBus messageBus, InvoiceId id)
    {
        _mediator = mediator;
        _messageBus = messageBus;
        this.BaseEntity = InvoiceEntityFactory.Create();
        this.InvoiceEntity = this.BaseEntity;
        this.LoadTask  = this.LoadAsync(id);
    }

    //....
}
```

Note the heavy duty constructor.  `InvoiceEntityMutor` instances are only meant to be obtained from a DI registered `InvoiceMutorFactory`. 

```csharp
public sealed class InvoiceMutorFactory
{
    private IServiceProvider _serviceProvider;

    public InvoiceMutorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<InvoiceEntityMutor> CreateInvoiceEntityMutorAsync(InvoiceId id)
    {
        var mutor = ActivatorUtilities.CreateInstance<InvoiceEntityMutor>(_serviceProvider, new object[] { id });
        await mutor.LoadTask;
        return mutor;
    }
}
```

The skeleton methods:

```csharp
    public bool IsDirty;
    public EditState State;

    public Return Dispatch(Func<InvoiceEntity, Return<InvoiceEntity>> dispatcher);

    private async Task LoadAsync(InvoiceId id);

    public async Task<Return> SaveAsync();

    public InvoiceItemRecordMutor GetInvoiceItemRecordMutor(InvoiceItemId id);

    public Return Reset();
```

The key mutation method is `Dispatch`.  It's a *Monadic Function* and `Dispatch` is a `Bind` operation. 

```csharp
public Return Dispatch(Func<InvoiceEntity, Return<InvoiceEntity>> dispatcher)
{
    InvoiceEntity = dispatcher.Invoke(InvoiceEntity)
        .WriteReturn(ret => LastResult = ret)
        .Write(defaultValue: this.InvoiceEntity);

    _messageBus.Publish<InvoiceEntity>(this.InvoiceEntity.InvoiceRecord.Id);

    return this.LastResult;
}
```

## Usage

The simplest way to look at usage is through *Tests*.

As an example let's look in detail at the `UpdateAnInvoiceItem` test.

First call the helpers to get the DI service container, mediator service and a sample Mutor from the database.  *You can see these in detail in the Repo*.

```csharp
var provider = GetServiceProvider();
var mediator = provider.GetRequiredService<IMediatorBroker>()!;
var mutor = await this.GetASampleMutorAsync(mediator);
```

Get a valid `InvoiceId` for an invoice to edit.

```csharp
// Get an Invoice Id
var entity = await this.GetASampleEntityAsync(mediator);
var Id = entity.InvoiceRecord.Id;
```

Get the entity Mutor for the Invoice Id.

```csharp
var entityMutor = await mutorFactory.GetInvoiceEntityMutorAsync(entity.InvoiceRecord.Id);
```

This uses the factory to instanciate the `InvoiceEntityMutor` and load the data from the data pipeline asynchronously. 

```csharp
public async Task<InvoiceEntityMutor> GetInvoiceEntityMutorAsync(InvoiceId id)
{
    var mutor = ActivatorUtilities.CreateInstance<InvoiceEntityMutor>(_serviceProvider, new object[] { id });
    await mutor.LoadTask;
    return mutor;
}
```
Next we create an `InvoiceItemRecordMutor` from the first item record

```csharp
// Get the Item Mutor
var itemMutor = InvoiceItemRecordMutor.Read(entityMutor.InvoiceEntity.InvoiceItems.First());
```

And simulate updating the value through a UI edit form:

```csharp
itemMutor.Amount = 59;
```

When we click save on the item we update the Entity Mutor by passing the itemMutor's Dispatcher to the entity mutor's dispatcher.

```csharp
var actionResult = entityMutor.Dispatch(itemMutor.Dispatcher);
```

The action dispatcher, as a delegate, depends on the item mutor's state:

```csharp
public Func<InvoiceEntity, Return<InvoiceEntity>> Dispatcher =>
    entity => this.State == EditState.Dirty
        ? UpdateInvoiceItemAction.Create(this.Record).Dispatcher(entity)
        : AddInvoiceItemAction.Create(this.Record).Dispatcher(entity);
```

When save the entity by calling `SaveAsync` on the entity mutor:

```csharp
var commandResult = await entityMutor.SaveAsync();
```

This dispatches an `InvoiceCommandRequest` to Mediator.

```csharp
public async Task<Return> SaveAsync()
    => await _mediator.DispatchAsync(new InvoiceCommandRequest(this.InvoiceEntity, EditState.Dirty, Guid.NewGuid()))
        .WriteReturnAsync(ret => this.LastResult = ret);
```

Finally we test by getting the entity from the data store and comparing it against the new entity.

```csharp
var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(Id));

Assert.True(entityResult.HasValue);

// Get the Mutor Entities
var updatedEntity = entityMutor.InvoiceEntity;
var dbEntity = entityResult.Value!;

// Check the stored data is the same as the edited entity
Assert.Equivalent(updatedEntity, dbEntity);
```

### UpdateInvoiceItemAction in Detail

`UpdateInvoiceItemAction` is a simple record:

```csharp
using Blazr.App.Core.Invoices;
namespace Blazr.App.Core;
public record UpdateInvoiceItemAction
{
    private readonly DmoInvoiceItem _invoiceItem;

    public UpdateInvoiceItemAction(DmoInvoiceItem invoiceItem)
        => _invoiceItem = invoiceItem;

    public Return<InvoiceEntity> Dispatcher(InvoiceEntity entity)
        => entity.ReplaceInvoiceItem(_invoiceItem);

    public static UpdateInvoiceItemAction Create(DmoInvoiceItem invoiceItem)
        => (new UpdateInvoiceItemAction(invoiceItem));
}
```

The relevant `InvoiceEntity` extension methods:

```csharp
internal Return<InvoiceEntity> ReplaceInvoiceItem(DmoInvoiceItem record)
    => @this.GetInvoiceItem(record)
        .Bind(item => @this.MutateWithEntityRulesApplied(@this.InvoiceItems.Replace(item, record)));

private Return<DmoInvoiceItem> GetInvoiceItem(DmoInvoiceItem item)
    => @this.GetInvoiceItem(item.Id);

internal Return<DmoInvoiceItem> GetInvoiceItem(InvoiceItemId id)
    => Return<DmoInvoiceItem>.Read(
        value: @this.InvoiceItems.SingleOrDefault(_item => _item.Id == id),
        errorMessage: "The record does not exist in the Invoice Items");

private Return<InvoiceEntity> MutateWithEntityRulesApplied(IEnumerable<DmoInvoiceItem> invoiceItems) 
    => InvoiceEntityFactory.ApplyEntityRules(InvoiceEntity.Read(@this.InvoiceRecord, invoiceItems)).ToReturnT;

```

And Factory methods:

```csharp
internal static InvoiceEntity ApplyEntityRules(InvoiceEntity entity)
    => InvoiceEntity.Read(
        invoice: entity.InvoiceRecord with { TotalAmount = new(entity.InvoiceItems.Sum(item => item.Amount.Value)) },
        invoiceItems: entity.InvoiceItems);
```

Note that many of the methods are `Internal` to hide them from the UI projects assemblies.

