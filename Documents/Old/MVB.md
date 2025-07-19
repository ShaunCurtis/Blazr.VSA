# The Model-View-Broker Pattern

**MVB** is my Blazor centric version of the **MVx** patterns.  It's specific focus in on Blazor, and how to structure a Blazor Applicatiion.

It fits within multi-project Clean Design solutions or single project Vertical Slice solutions.  The demo solution uses a *Vertical Slice* structure with namespace segregation of the clean design domains.

## TL;DR

In a nutshell, the older design patterns don't sit well with Blazor.  They were the products of the architectures of their time.  The relationship between the model and the glue (the Cointroller/Presenter/ViewModel) has changed: it requires a different operational context and division of labour.  You can continue applying the older models, but what you will find is you start breaking some of their fundimentsl rules to implement sound solutions.

## The Context

The primary three view contexts you need to consider are:

1. Page Context - the `@Body` content in the layout.
2. Form Context - a dialog or inline form  within the page context.
3. Layout Context - the who displayed browser window.

Each of these contexts has a different lifespan, none of which conform exactly to the `Scoped` concept of Dependancy Injection.  This creates a dilemna in how do you configure the services that contexts use.

Consider the simple Customer Display Page.  It's broker needs to interface with the core domain to get it's data, so it must use DI to access these services.  However, it also needs the Id of the record it needs to retrieve.  This can only be provided by the page context.

If you implement the Customer Dispay UI Broker as a scoped service you need a two stage process:

1. Inject the broker from DI into the page context.
2. Call a load method on the service to load the necessary record.


 
## CQS, Mediator, Flux and the Readonly World

C# now provides built-in immutable objects, so we can apply some functional programming paradigms.  I use records and readonly structs extensively.

The data pipeline is read only and base on CQS.  There are three standard operations:

1. Read a Record by providing an identity
2. Read a Collection by providing a list request.
3. Execute a Command by providing the updated record and a command action (Create/Update/Delete).

Mediator provides the glue between the CQS implementation and the core application.

I mention the *Flux Pattern* in the section header because you will see it's influence in my code, particularly in my aggregate implementation.

## The View

The Customer Viewer is used as the example view. 

The view is built using a `ViewerModalFormBase` base class which contains all the boilerplate code.

`OnInitializedAsync` gets a `IReadUIBroker` instance from the `IReadUIBrokerFactory` and wires up a handler to render on a record state change.


```csharp
public abstract partial class ViewerModalFormBase<TRecord, TKey> : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    [Inject] protected IReadUIBrokerFactory UIBrokerFactory { get; set; } = default!;
    [Inject] protected IUIEntityProvider<TRecord> UIEntityService { get; set; } = default!;

    [Parameter] public TKey Uid { get; set; } = default!;
    [CascadingParameter] protected IModalDialog? ModalDialog { get; set; }

    protected IReadUIBroker<TRecord, TKey> UIBroker { get; set; } = default!;
    protected string FormTitle => $"{this.UIEntityService.SingleDisplayName} Viewer";

    protected async override Task OnInitializedAsync()
    {
        // check we have the necessary parameter objects to function
        ArgumentNullException.ThrowIfNull(Uid);
        ArgumentNullException.ThrowIfNull(ModalDialog);

        this.UIBroker = await this.UIBrokerFactory.GetAsync<TRecord, TKey>(this.Uid);

        this.UIBroker.RecordChanged += OnRecordChanged;
    }

    protected void OnRecordChanged(object? sender, EventArgs e)
    {
        this.StateHasChanged();
    }

    protected Task OnExit()
    {
        ModalDialog?.Close(new ModalResult());
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        this.UIBroker.RecordChanged -= OnRecordChanged;
        this.UIBroker.Dispose();
    }
}
```

The view form itself is all UI code:

```csharp
@inherits ViewerModalFormBase<DmoCustomer, CustomerId>

<PageTitle>@this.FormTitle</PageTitle>

<div class="modal-header">
    <div class="h4">
        @this.FormTitle
    </div>
</div>

<div class="modal-body px-3">

    <div class="row">

        <div class="col-12 col-lg-6 mb-2">
            <BlazrTextViewControl Label="Id" Value="@this.UIBroker.Item.Id.Value.ToDisplayId()" />
        </div>

    </div>

    <div class="row">

        <div class="col-12 col-lg-8 mb-2">
            <BlazrTextViewControl Label="Customer Name" Value="@this.UIBroker.Item.CustomerName" />
        </div>

    </div>
</div>

<div class="modal-footer bg-light text-end">
    <UIButton ButtonColourType=UIButtonColourType.Exit ClickEvent=this.OnExit>Exit</UIButton>
</div>
```

## EntityProvider

Before we move on to the broker, we need to understand the role of the `EntityProvider`.

Most entities use the generic code base.  However, there are several generic to specific switching issues and operations based on the Id Keys.  These are solved by DI registered `IEntityProvider`s for specific entities. 

```csharp
public interface IEntityProvider<TRecord, TKey>
{
public interface IReadUIBroker<TRecord, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    public TRecord Item { get; }
    public event EventHandler? RecordChanged;
    public IDataResult LastResult { get;}
    public ValueTask LoadAsync(TKey id);
    public void Dispose();
}}
```

The `IEntityProvider` implementation for the Customer Entity.

```csharp
public class CustomerEntityProvider : IEntityProvider<DmoCustomer, CustomerId>
{
    private readonly IMediator _mediator;

    public Func<CustomerId,  Task<Result<DmoCustomer>>> RecordRequest
        => (id) => _mediator.Send(new CustomerRecordRequest(id));

    public Func<DmoCustomer, CommandState,  Task<Result<CustomerId>>> RecordCommand
        => (record, state) => _mediator.Send(new CustomerCommandRequest(record, state));

    public CustomerEntityProvider(IMediator mediator)   
    {
        _mediator = mediator;
    }

    public CustomerId GetKey(object obj)
    {
        return obj switch
        {
            CustomerId id => id,
            DmoCustomer record => record.Id,
            Guid guid => new CustomerId(guid),
            _ => CustomerId.Default
        };
    }

    public bool TryGetKey(object obj, out CustomerId key)
    {
        key = GetKey(obj);
        return key != CustomerId.Default;
    }

    public DmoCustomer NewRecord
        => DefaultRecord;

    public static DmoCustomer DefaultRecord
        => new DmoCustomer { Id = CustomerId.Default };
}
```

## UIBroker

We have an interface:

```csharp
public interface IEntityProvider<TRecord, TKey>
{
    public Func<TKey, Task<Result<TRecord>>> RecordRequest { get; }
    public Func<TRecord, CommandState, Task<Result<TKey>>> RecordCommand { get; }
    public TKey GetKey(object obj);
    public bool TryGetKey(object obj, out TKey key);
    public TRecord NewRecord { get; }
}
```

And implementation:

```csharp
public class ReadUIBroker<TRecord, TKey> : IReadUIBroker<TRecord, TKey>, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    private readonly IEntityProvider<TRecord, TKey> _entityProvider;
    private readonly IMessageBus _messageBus;
    private TKey _key = default!;

    public TRecord Item { get; protected set; } = new TRecord();
    public event EventHandler? RecordChanged;
    public IDataResult LastResult { get; protected set; } = DataResult.Success();

    public ReadUIBroker(IEntityProvider<TRecord, TKey> entityProvider, IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _entityProvider = entityProvider;

        _messageBus.Subscribe<TKey>(OnRecordChanged);
    }

    public async ValueTask LoadAsync(TKey id)
        => await GetRecordItemAsync(id);

    private async ValueTask GetRecordItemAsync(TKey id)
    {
        _key = id;

        // Call the RecordRequest on the record specific EntityProvider to get the record
        var result = await _entityProvider.RecordRequest.Invoke(id);

        LastResult = result.ToDataResult;

        if (result.HasSucceeded(out TRecord? record))
            this.Item = record ?? _entityProvider.NewRecord;
    }

    private async void OnRecordChanged(object? record)
    {
        // test to see if we have a key of the same type
        // if so and it doesn't match the current key, we dont need to do anything
        // if we don't have a key, we need to load the record just in case
        if (record is TKey value)
        {
            if (!_key.Equals(value))
                return;
        }

        await this.GetRecordItemAsync(_key);
        this.RecordChanged?.Invoke(this, EventArgs.Empty);

        return;
    }

    public void Dispose()
    {
        _messageBus.UnSubscribe<TKey>(OnRecordChanged);
    }
}
```
