# Mutors

> **Mutor** is my word to describe a set of patterns for implementing the mutaton of immutable objects.

Entity Mutors are classes that provide *aggregate* functionality over a complex domain entity such as the `InvoiceEntity`.  It maintains a copy of the original record and thus provides state information such as `IsDirty` and `IsNew`.  A copy of the current record is obtained from the `Record` property.  Mutors can only be created though two *static* methods: `Load` and `Create`.  Mutors are typically used as the `model` for an edit form.

## The Immutable Invoice Entity

First the two simple domain objects that represent the invoice and the invoice items:

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

The base locked down `InvoiceEntity`.  Everything is immutable, the constructor is private and the `Load` static consructor is `internal` to *Blazr.App*.

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

    internal static InvoiceEntity Load(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems) 
        => new InvoiceEntity(invoice, invoiceItems);
}
```

## The Mutor

Some key points to note about the Mutor:

1. It's designed to be created through a DI based factory class.  It has a two step creation proccess: assigment of a set of injected services and an *async* call to get the record through the data pipeline.

1. It's mutated by calling `Dispatch` with a function that matches the `Func<InvoiceEntity, Return<InvoiceEntity>>` pattern.  It invokes the passed function and stores the resulting new `InvoiceEntity`. 

1. The Mutor is saved by calling `SaveAsync`.

1. `Reset` resets the mutor.
 
1. `GetInvoiceItemRecordMutor` gets a `InvoiceItemRecordMutor` based on the provided `InvoiceItemId`.
 
```csharp
public sealed class InvoiceEntityMutor
{
    private readonly IMediatorBroker _mediator;
    private readonly IMessageBus _messageBus;

    public InvoiceEntity BaseEntity { get; private set; }
    public InvoiceEntity InvoiceEntity { get; private set; }
    public Return LastResult { get; private set; } = Return.Success();
    public bool IsNew => this.BaseEntity.InvoiceRecord.Id.IsNew;
    public Task LoadTask { get; private set; } = Task.CompletedTask;
    public bool IsDirty => !this.InvoiceEntity.Equals(BaseEntity);

    public InvoiceEntityMutor(IMediatorBroker mediator, IMessageBus messageBus, InvoiceId id)
    {
        _mediator = mediator;
        _messageBus = messageBus;
        this.BaseEntity = InvoiceEntityFactory.Create();
        this.InvoiceEntity = this.BaseEntity;
        this.LoadTask = this.LoadAsync(id);
    }

    public EditState State => (this.IsNew, this.IsDirty) switch
    {
        (true, _) => EditState.New,
        (false, false) => EditState.Clean,
        (false, true) => EditState.Dirty,
    };

    public Return Dispatch(Func<InvoiceEntity, Return<InvoiceEntity>> dispatcher)
        => dispatcher.Invoke(InvoiceEntity)
            .SetReturn(ret => LastResult = ret)
            .Notify(hasValue: value => this.InvoiceEntity = value)
            .Notify(hasValue: value => _messageBus.Publish<InvoiceEntity>(value.InvoiceRecord.Id))
            .ToReturn();

    private async Task LoadAsync(InvoiceId id)
        => await _mediator.DispatchAsync(new InvoiceEntityRequest(id))
            .SetReturnAsync(ret => this.LastResult = ret)
            .NotifyAsync( 
                hasValue: this.Set,
                hasNoValue: this.SetAsNew
            )
            .ToReturnAsync();

    public async Task<Return> SaveAsync()
        => await _mediator.DispatchAsync(new InvoiceCommandRequest(this.InvoiceEntity, EditState.Dirty, Guid.NewGuid()))
            .SetReturnAsync(ret => this.LastResult = ret);

    public InvoiceItemRecordMutor GetInvoiceItemRecordMutor(InvoiceItemId id)
        => this.InvoiceEntity.GetInvoiceItem(id)
            .Write(
                hasValue: value => InvoiceItemRecordMutor.Load(value),
                hasNoValue: () => InvoiceItemRecordMutor.NewMutor(this.InvoiceEntity.InvoiceRecord.Id));

    public Return Reset()
    {
        this.Set(this.BaseEntity);
        this.LastResult = Return.Success();
        return this.LastResult;
    }
    
    private void Set(InvoiceEntity entity)
    {
        this.InvoiceEntity = entity;
        this.BaseEntity = entity;
    }

    private void SetAsNew()
    {
        this.InvoiceEntity = InvoiceEntityFactory.Create();
        this.BaseEntity = this.InvoiceEntity;
    }
}
```

The factory class uses the `ActivatorUtilities` class in `GetInvoiceEntityMutorAsync` to initialize a class instance with the DI container and ensure it's loaded.

```csharp
public sealed class InvoiceMutorFactory
{
    private IServiceProvider _serviceProvider;

    public InvoiceMutorFactory(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task<InvoiceEntityMutor> GetInvoiceEntityMutorAsync(InvoiceId id)
    {
        var mutor = ActivatorUtilities.CreateInstance<InvoiceEntityMutor>(_serviceProvider, new object[] { id });
        await mutor.LoadTask;
        return mutor;
    }
}
```

## InvoiceEntity Extensions

`InvoiceEntityExtensions` adds some helper extensions to apply mutations by the Mutor. 

The important two are `Mutate`.  These create new mutated `InvoiceEntity` instances based on the parameters provided.

```csharp
internal static class InvoiceEntityExtensions
{
    extension(InvoiceEntity entity)
    {
        internal Return<InvoiceEntity> ToReturnT => Return<InvoiceEntity>.Read(entity);

        internal bool IsDirty(InvoiceEntity control) => !entity.Equals(control);

        internal Return<DmoInvoiceItem> GetInvoiceItem(InvoiceItemId id)
            => Return<DmoInvoiceItem>.Read(
                value: entity.InvoiceItems.SingleOrDefault(_item => _item.Id == id),
                errorMessage: "The record does not exist in the Invoice Items");

        internal Return<DmoInvoiceItem> GetInvoiceItem(DmoInvoiceItem item)
            => entity.GetInvoiceItem(item.Id);

        internal Return<InvoiceEntity> Mutate(DmoInvoice invoice)
            => InvoiceEntityFactory.Load(invoice, entity.InvoiceItems)
                .Map(InvoiceEntityFactory.ApplyEntityRules);

        internal Return<InvoiceEntity> Mutate(IEnumerable<DmoInvoiceItem> invoiceItems)
            => InvoiceEntityFactory.Load(entity.InvoiceRecord, invoiceItems)
                .Map(InvoiceEntityFactory.ApplyEntityRules);
    }
}
```

## Actions

At this point we have the mechanisms in place to read in an Invoice Entity, mutate it and then save it.  I'll use a et of tests to demonstrate the usage.  You can review the full test setup in the *Repo*.

Actions provide the data and code to be executed by the Mutor's dispatcher.

### Update An Invoice

The Action:

```csharp
public record UpdateInvoiceAction
{
    private readonly DmoInvoice _invoice;

    public UpdateInvoiceAction(DmoInvoice invoice)
        => _invoice = invoice;

    public Return<InvoiceEntity> Dispatcher(InvoiceEntity entity)
        => entity.Mutate(_invoice);

    public static UpdateInvoiceAction Create(DmoInvoice invoice)
            => (new UpdateInvoiceAction(invoice));
}
```
It calls `entity.Mutate` to do tthe actual mutation.

And the test with inline commentary:

```csharp
    public async Task UpdateAnInvoice()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;
        var mutorFactory = provider.GetRequiredService<InvoiceMutorFactory>()!;

        // Get a sample Invoice Mutor
        var entity = await this.GetASampleEntityAsync(mediator);
        var Id = entity.InvoiceRecord.Id;

        var entityMutor = await mutorFactory.GetInvoiceEntityMutorAsync(entity.InvoiceRecord.Id);
        var invoiceMutor = InvoiceRecordMutor.Load(entityMutor.InvoiceEntity.InvoiceRecord);

        // Update the Mutor
        invoiceMutor.Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-5));

        // Update the Entity Mutor by dispatching the itemMutor's Dispatcher
        var actionResult = entityMutor.Dispatch(invoiceMutor.Dispatcher);

        // Commit the changes to the data store
        var commandResult = await entityMutor.SaveAsync();
    }
```

The `invoiceMutor.Dispatcher` looks like this, creating and then dispatching the action:

```cshrp
public Func<InvoiceEntity, Return<InvoiceEntity>> Dispatcher =>
    entity => (this.IsNew, this.IsDirty) switch
    {
        (false, true) => UpdateInvoiceAction.Create(this.Record).Dispatcher(entity),
        _ => ReturnT.Read(entity),
    };
```