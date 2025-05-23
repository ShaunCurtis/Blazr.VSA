#  Data Pipeline

The data pipeline is implemnted using several coding patterns and concepts:

1. *CQS*  
2. *Mediator*
3. *Records* i.e. *Read Only*

There are three pipelines:

1. List Query - Get a list.
2. Record Query - Get a single record.
3. Command - Create/Update/Delete a record.

The Customer feature demonstrates a normal implementation.

The UI and presentation layer `UIBroker` uses boilerplate code, so we need a strategy to plug the generic code into to the data pipeline where each request is specific to an entity.  We do this through an `IEntityProvider` service that defines a set of functions and delegates to execute for each entity.

The `IEntityProvider` interface:

```csharp
public interface IEntityProvider<TRecord, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    public Func<TKey, Task<Result<TRecord>>> RecordRequest { get; }

    public Func<TRecord, CommandState, Task<Result<TKey>>> RecordCommand { get; }

    public Func<GridState<TRecord>, Task<Result<ListItemsProvider<TRecord>>>> ListRequest { get; }

    public TKey GetKey(object obj);

    public bool TryGetKey(object obj, out TKey key);

    public TRecord NewRecord { get; }
}
```

And the `CustomerEntityProvider` implementation:

```csharp
public class CustomerEntityProvider : IEntityProvider<DmoCustomer, CustomerId>
{
    private readonly IMediatorBroker _mediator;

    public Func<CustomerId,  Task<Result<DmoCustomer>>> RecordRequest
        => (id) => _mediator.DispatchAsync(new CustomerRecordRequest(id));

    public Func<DmoCustomer, CommandState,  Task<Result<CustomerId>>> RecordCommand
        => (record, state
    public CustomerEntityProvider(IMediatorBroker mediator)   
    {
        _mediator = mediator;
    }
 => _mediator.DispatchAsync(new CustomerCommandRequest(record, state));

    public Func<GridState<DmoCustomer>, Task<Result<ListItemsProvider<DmoCustomer>>>> ListRequest
        => (state) => _mediator.DispatchAsync(new CustomerListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });
//....
}
```

The entity provider injects the Mediator service and each delegate builds the specific request, dispatches it to Mediator and returns the returned object.

The delegate dispatches the provider `id` to Mediator broker.

```csharp
_mediator.DispatchAsync(new CustomerRecordRequest(id));
```


To demonstrate this in action, so following is the relevant code snippet from the generic UI Record Broker.  It injects the specific Entity Provider through DI and then executes the delegate. 

```csharp
private async ValueTask GetRecordItemAsync(TKey id)
{
    _key = id;

    // Call the RecordRequest on the record specific EntityProvider to get the record
    var result = await _entityProvider.RecordRequest.Invoke(id);

    LastResult = result;

    if (result.HasSucceeded(out TRecord? record))
        this.Item = record ?? _entityProvider.NewRecord;
}
```

And Mediator runs the registered handler:


```csharp
public sealed class CustomerRecordHandler : IRequestHandler<CustomerRecordRequest, Result<DmoCustomer>>
{
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public CustomerRecordHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Result<DmoCustomer>> HandleAsync(CustomerRecordRequest request, CancellationToken cancellationToken)
    {
        var dbContext = _factory.CreateDbContext();

        Expression<Func<DvoCustomer, bool>> findExpression = (item) =>
            item.CustomerID == request.Id.Value;

        var query = new RecordQueryRequest<DvoCustomer>(findExpression);

        var result = await dbContext.GetRecordAsync<DvoCustomer>(query);

        if (!result.HasSucceeded(out DvoCustomer? record))
            return result.ConvertFail<DmoCustomer>();

        var returnItem = record.ToDmo();

        return Result<DmoCustomer>.Success(returnItem);
    }
}
```