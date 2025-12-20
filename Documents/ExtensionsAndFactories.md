# Extensions and Factories

Why do you need them?

Let's look at the `InvoiceEntity` and `InvoiceEntityMutor` to demonstrate their purpose.

## Object Creation Control

Creating a `InvoiceEntityMutor` is a two step process:

1. Create the object.
2. Asynchronously load the `InvoiceEntity` from the data pipeline [and reportand deal with problems].

You can't use `new`.  It's purely synchronous.

A factory provides a framework for asynchronous object creation.

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

The answer is a factory.  Here's the `InvoiceMutorFactory`.


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


