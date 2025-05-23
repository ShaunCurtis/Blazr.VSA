# Item Requests

Generically we can define a request for a single item like this:

```csharp
ItemResult GetItemAsync(ItemRequest request);
```

## The CQS Handler

We can define a generic CQS handler like this:

```
public static async ValueTask<Result<TRecord>> GetRecordAsync<TRecord>(TDbContext dbContext, RecordQueryRequest<TRecord> request)
    where TRecord : class
{
    dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

    var record = await dbContext.Set<TRecord>()
        .FirstOrDefaultAsync(request.FindExpression)
        .ConfigureAwait(ConfigureAwaitOptions.None);

    if (record is null)
        return Result<TRecord>.Fail(new RecordQueryException($"No record retrieved with the Key provided"));

    return Result<TRecord>.Success(record);
}
```

And attach it to the DBContext as an extension method:

```csharp
    public static async ValueTask<Result<TRecord>> GetRecordAsync<TRecord>(this DbContext dbContext, RecordQueryRequest<TRecord> request)
        where TRecord : class
    {
        return await CQSEFBroker<DbContext>.GetRecordAsync(dbContext, request);
    }
```

The CQS request:

```csharp
public record RecordQueryRequest<TRecord>(
    Expression<Func<TRecord, bool>> FindExpression,
    CancellationToken? Cancellation = null
);
```

## The Mediator Request

All we need is the record Id.  Here's the Customer Record Request:

```csharp
public readonly record struct CustomerRecordRequest(CustomerId Id) : IRequest<Result<DmoCustomer>>;
```

Which is used by the delegate defined in the CustomerEntityProvider:

```csharp
public Func<CustomerId,  Task<Result<DmoCustomer>>> RecordRequest
    => (id) => _mediator.DispatchAsync(new CustomerRecordRequest(id));
```

And invoked from the generic `ReadUIBroker`:   

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
