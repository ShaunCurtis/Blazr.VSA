# Extensions and Factories

Why do you need them?

Let's look at the `InvoiceEntity` and `InvoiceEntityMutor` to demonstrate their purpose.

## Object Creation Control

Creating an `InvoiceEntityMutor` is a two step process:

1. Create the object.
2. Asynchronously load the `InvoiceEntity` from the data pipeline [and reportand deal with problems].

You can't use `new`.  It's purely synchronous.

Let's look at the `InvoiceEntityMutor` new method:

```csharp
    public InvoiceEntityMutor(IMediatorBroker mediator, IMessageBus messageBus, InvoiceId id)
    {
        _mediator = mediator;
        _messageBus = messageBus;
        this.BaseEntity = InvoiceEntityFactory.Create();
        this.InvoiceEntity = this.BaseEntity;
        this.LoadTask = this.LoadAsync(id);
    }
```

It's looks like a *DI* constructor, but how do you provide `id`.

The answer is a factory.  Here's the `InvoiceMutorFactory` and the `GetInvoiceEntityMutorAsync` method.


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

It uses the `ActivatorUtilities` utility to create an instance of `InvoiceEntityMutor` within the DI context, and then awaits the `LoadTask`.

## Extensions

Extensions provide a way to add functionity to an existing object.  They also conversely provide a mechanism to provide extra functionality where it's needed, and hide it from where it's not.

Let's look at `InvoiceEntity`.


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

Externally to **Blazr.App, such as from the UI code, it's a very simple object.  You can view it's data.  You can't even create an instancedirectly: the creation process is internal to the *Blazr.App* project/assembly.

However, within *Blazr.App*, the following extension methods are implemented. 

```csharp
internal static class InvoiceEntityExtensions
{
    extension(InvoiceEntity @this)
    {
        internal Return<InvoiceEntity> ToReturnT;
        internal bool IsDirty(InvoiceEntity control);
        internal Return<InvoiceEntity> AddInvoiceItem(DmoInvoiceItem item);
        internal Return<InvoiceEntity> DeleteInvoiceItem(DmoInvoiceItem record);
        internal Return<InvoiceEntity> ReplaceInvoiceItem(DmoInvoiceItem record);
        internal Return<InvoiceEntity> ReplaceInvoice(DmoInvoice record);
        internal Return<DmoInvoiceItem> GetInvoiceItem(InvoiceItemId id);

        private Return<DmoInvoiceItem> GetInvoiceItem(DmoInvoiceItem item);
        private Return<InvoiceEntity> Mutate(DmoInvoice invoice);
        private Return<InvoiceEntity> Mutate(IEnumerable<DmoInvoiceItem> invoiceItems);
    }
}
```

And used by the aggregate mutuation processes to update the entity mutor.
