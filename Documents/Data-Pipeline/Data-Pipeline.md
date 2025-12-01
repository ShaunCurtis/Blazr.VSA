#  Data Pipeline

The data pipeline is based on the following coding patterns and concepts:

1. *CQS*  
2. *Mediator*
3. *Records* i.e. *Read Only*
4. *Monads* - the `Bool<T>` Monad

There are three pipelines:

1. List Query - Get a list.
2. Record Query - Get a single record.
3. Command - Create/Update/Delete a record.

*Functional Programming* semantics are used at various points.  The pipeline returns `Bool<T>` objects floing errors and failure to the caller.

The Customer feature demonstrates a normal implementation.

The UI and presentation layers use a `IUIConnector` interface to get configuration information and talk to the *Core*.  Each Entity has a specific implementation that defines a set of functions and delegates to execute for each entity.

The `IUIConnector` interface:

```csharp
public interface IUIConnector<TRecord, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    public string SingleDisplayName { get; }
    public string PluralDisplayName { get; }
    public Type? EditForm { get; }
    public Type? ViewForm { get; }
    public string Url { get; }

    public Task<Bool<GridItemsProviderResult<TRecord>>> GetItemsAsync(GridState<TRecord> state);
    public Func<TKey, Task<Bool<TRecord>>> RecordRequestAsync { get; }
    public Func<StateRecord<TRecord>, Task<Bool<TKey>>> RecordCommandAsync { get; }
    public IRecordMutor<TRecord> GetRecordMutor(TRecord record);
}
```

And the `CustomerUIConnector` implementation:

```csharp
public class CustomerUIConnector
   : IUIConnector<DmoCustomer, CustomerId>
{
    private readonly IMediatorBroker _mediator;

    public string SingleDisplayName { get; } = "Customer";
    public string PluralDisplayName { get; } = "Customers";
    public Type? EditForm { get; } = typeof(CustomerEditForm);
    public Type? ViewForm { get; } = typeof(CustomerViewForm);
    public string Url { get; } = "/Customer";

    public CustomerUIConnector(IMediatorBroker mediator)
    {
        _mediator = mediator;
    }

    public Func<CustomerId, Task<Bool<DmoCustomer>>> RecordRequestAsync
        => id => id.IsDefault ? NewRecordRequestAsync(id) : ExistingRecordRequestAsync(id);

    public Func<StateRecord<DmoCustomer>, Task<Bool<CustomerId>>> RecordCommandAsync
        => record => _mediator.DispatchAsync(new CustomerCommandRequest(record));

    public Task<Bool<GridItemsProviderResult<DmoCustomer>>> GetItemsAsync(GridState<DmoCustomer> state)
        => CustomerListRequest.Create(state)
            .BindAsync((request) => _mediator.DispatchAsync(request))
            .MapAsync((itemsProvider) => GridItemsProviderResult
                    .From<DmoCustomer>(itemsProvider.Items
                    .ToList()
                , itemsProvider.TotalCount));

    public IRecordMutor<DmoCustomer> GetRecordMutor(DmoCustomer customer)
        => CustomerRecordMutor.Read(customer);

    private Func<CustomerId, Task<Bool<DmoCustomer>>> ExistingRecordRequestAsync
        => id => _mediator.DispatchAsync(new CustomerRecordRequest(id));

    private Func<CustomerId, Task<Bool<DmoCustomer>>> NewRecordRequestAsync
        => id => Task.FromResult(BoolT.Read(new DmoCustomer { Id = CustomerId.Default }));
}
```

The entity provider injects the Mediator service.  Each delegate builds its specific request, dispatches it to Mediator and returns the result.

For example the delegate dispatches the provider `id` to Mediator broker rto get an existing record.

```csharp
private Func<CustomerId, Task<Bool<DmoCustomer>>> ExistingRecordRequestAsync
    => id => _mediator.DispatchAsync(new CustomerRecordRequest(id));
```

The request:

```csharp
public readonly record struct CustomerRecordRequest(CustomerId Id) 
    : IRequest<Bool<DmoCustomer>>;
```

And the Mediator implementation runs the registered handler:

```csharp
public sealed class CustomerRecordHandler : IRequestHandler<CustomerRecordRequest, Bool<DmoCustomer>>
{
    private IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public CustomerRecordHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> dbContextFactory)
        => _factory = dbContextFactory;

    public async Task<Bool<DmoCustomer>> HandleAsync(CustomerRecordRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        return await dbContext
            .GetRecordAsync<DvoCustomer>(new RecordQueryRequest<DvoCustomer>(item => item.CustomerID == request.Id.Value))
            .BindAsync<DvoCustomer, DmoCustomer>(DvoCustomer.MapToBool);
    }
}
```

The method:

1. Creates a `DbContext`
2. Calls `GetRecordAsync` - an extension method on `DbContext`:

```csharp
    public static async Task<Bool<TRecord>> GetRecordAsync<TRecord>(this DbContext dbContext, RecordQueryRequest<TRecord> request)
        where TRecord : class
            => await CQSEFBroker<DbContext>.GetRecordAsync(dbContext, request).ConfigureAwait(ConfigureAwaitOptions.None);
```

that calls the `CQSEFBroker` method `GetRecordAsync`:

```csharp
    public static async Task<Bool<TRecord>> GetRecordAsync<TRecord>(TDbContext dbContext, RecordQueryRequest<TRecord> request)
        where TRecord : class
    {
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        var record = await dbContext.Set<TRecord>()
            .FirstOrDefaultAsync(request.FindExpression)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (record is null)
            return Bool<TRecord>.Failure($"No record retrieved with the Key provided");

        return Bool<TRecord>.Success(record);
    }
```

3. Calls `DvoCustomer.MapToBool` to map the database object `DvoCustomer` to a `DmoCustomer` domain object.

```csharp
    public static Bool<DmoCustomer> MapToBool(DvoCustomer item)
        => Bool<DmoCustomer>.Read(Map(item));

    public static DmoCustomer Map(DvoCustomer item)
        => new()
        {
            Id = new(item.CustomerID),
            Name = new (item.CustomerName ?? string.Empty)
        };
```
