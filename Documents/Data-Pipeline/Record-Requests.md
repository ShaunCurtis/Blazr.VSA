# Item Requests

Generically we can define a request for a single item like this:

```csharp
ItemResult GetItemAsync(ItemRequest request);
```

## The CQS Request

We can define a generic request object:

```
public readonly record struct RecordQueryRequest<TRecord>
{
    public Expression<Func<TRecord, bool>> FindExpression { get; private init; }
    public CancellationToken Cancellation { get; private init; }

    public RecordQueryRequest(Expression<Func<TRecord, bool>> expression, CancellationToken? cancellation = null)
    {
        this.FindExpression = expression;
        this.Cancellation = cancellation ?? new(); 
    }

    public static RecordQueryRequest<TRecord> Create(Expression<Func<TRecord, bool>> expression, CancellationToken? cancellation = null)
        => new RecordQueryRequest<TRecord>(expression, cancellation ?? new());
}
```
## The Mediator Request

All we need is the record Id.  Here's the Customer Record Request:

```csharp
public readonly record struct CustomerRecordRequest(CustomerId Id) : IRequest<Result<DmoCustomer>>;
```

## The Mediator Handler


```csharp
public sealed class CustomerRecordHandler : IRequestHandler<CustomerRecordRequest, Result<DmoCustomer>>
{
    private IRecordRequestBroker _broker;

    public CustomerRecordHandler(IRecordRequestBroker broker)
    {
        _broker = broker;
    }

    public async Task<Result<DmoCustomer>> Handle(CustomerRecordRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<DboCustomer, bool>> findExpression = (item) =>
            item.CustomerID == request.Id.Value;

        var query = new RecordQueryRequest<DboCustomer>(findExpression);

        var result = await _broker.ExecuteAsync<DboCustomer>(query);

        if (!result.HasSucceeded(out DboCustomer? record))
            return result.ConvertFail<DmoCustomer>();

        var returnItem = DboCustomerMap.Map(record);

        return Result<DmoCustomer>.Success(returnItem);
    }
}
```

## The CQS Broker

```csharp
public sealed class RecordRequestServerBroker<TDbContext>
    : IRecordRequestBroker
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public RecordRequestServerBroker(IDbContextFactory<TDbContext> factory)
    {
        _factory = factory;
    }

    public async ValueTask<Result<TRecord>> ExecuteAsync<TRecord>(RecordQueryRequest<TRecord> request)
        where TRecord : class
    {
        return await this.GetItemAsync<TRecord>(request);
    }

    private async ValueTask<Result<TRecord>> GetItemAsync<TRecord>(RecordQueryRequest<TRecord> request)
        where TRecord : class
    {
        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        var record = await dbContext.Set<TRecord>()
            .FirstOrDefaultAsync(request.FindExpression)
            .ConfigureAwait(false);

        if (record is null)
            return Result<TRecord>.Fail(new ItemQueryException($"No record retrieved with the Key provided"));

        return Result<TRecord>.Success(record);
    }
}
```

## The Result

All requests return data and status information.  We should never return a `null` without explaining why!

We can define an interface so we handle the status information regardless of the type of data: we may want to display or log errors.

```csharp
public interface IDataResult
{
    public bool Successful { get; }
    public string Message { get; }
}
```

And then the result using generics.

```csharp
public sealed record ItemQueryResult<TRecord> : IDataResult
{
    public TRecord? Item { get; init;} 
    public bool Successful { get; init; }
    public string Message { get; init; } = string.Empty;

    private ItemQueryResult() { }

    public static ItemQueryResult<TRecord> Success(TRecord Item, string? message = null)
        => new ItemQueryResult<TRecord> { Successful=true, Item= Item, Message= message ?? string.Empty };

    public static ItemQueryResult<TRecord> Failure(string message)
        => new ItemQueryResult<TRecord> { Message = message};
}
```

There are two static constructors to control how a result is constructed: it either succeeded or failed.
