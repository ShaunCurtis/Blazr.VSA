# List Requests

Generically we can define a request for a list like this:

```csharp
ListResult GetItemsAsync(ListRequest request);
```

## The Challenges

Lists present the greatest challenges in data retrieval.

Why can't you just do:

```
var list = await dbContext.Set<TRecord>().ToListAsync();
```

Consider the following:

1. Do you really want to retrieve all the records at once.
2. What are you going to do with 2000 retrieved records?  Display them on a single page?
3. Can you gracefully handle retrieving a million records?
4. What order do you want the data set in?
5. Do you want to filter the data set?

## The CQS Request

Building these requirments into the design, we can define a struct like this for the data pipleine server side backend:

```
public readonly record struct ListQueryRequest<TRecord>
{
    public int StartIndex { get; init; }
    public int PageSize { get; init; }
    public CancellationToken Cancellation { get; init; }
    public Expression<Func<TRecord, bool>>? FilterExpression { get; init; }
    public Expression<Func<TRecord, object>>? SortExpression { get; init; }
    public bool SortDescending { get; init; } = true;

    public ListQueryRequest()
    {
        StartIndex = 0;
        PageSize = 1000;
        Cancellation = new();
        FilterExpression = null;
        SortExpression = null;
    }
}
```

`StartIndex` and `PageSize` define the data chunk we want to retrieve.  We can apply the values as `Skip` and `Take` actions in the `IQueryable` query.

Sorting is defined by an `Expression<Func<TRecord, object>>` and a `SortDescending` flag. 

Filtering is defined by an `Expression<Func<TRecord, bool>>`.  This is a little more complex and we'll see how we handle this shortly.

## The Mediator Request

The Mediator request is record specific.  The *Customer* list request looks like this:

```csharp
public record CustomerListRequest
    : BaseListRequest, IRequest<Result<ListResult<DmoCustomer>>>
{ }
```

The base implementation:

```csharp
public record BaseListRequest
{
    public int StartIndex { get; init; } = 0;
    public int PageSize { get; init; } = 1000;
    public string? SortColumn { get; init; } = null;
    public bool SortDescending { get; init; } = false;
}
```


## The Mediator Handler

The handler is where the work is done.  The handler is a `IRequestHandler` that takes in the request and returns a `Result<ListResult<DmoCustomer>>`.  The handler is responsible for getting the data from the database and returning it in the requested format.  In this case, it uses the generic `IListRequestBroker` to get the data.

It builds a `ListQueryRequest` object from the request and passes it to the broker.  The broker returns a `ListResult` object that is then mapped to the `DmoCustomer` object and returned as a `Result<ListResult<DmoCustomer>>`.  The `GetSorter` and `GetFilter` methods are used to build the `SortExpression` and `FilterExpression` objects from data provided in the `CustomerListRequest`.

```csharp
public sealed class CustomerListHandler : IRequestHandler<CustomerListRequest, Result<ListResult<DmoCustomer>>>
{
    private IListRequestBroker _broker;

    public CustomerListHandler(IListRequestBroker broker)
    {
        this._broker = broker;
    }

    public async Task<Result<ListResult<DmoCustomer>>> Handle(CustomerListRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<DmoCustomer> forecasts = Enumerable.Empty<DmoCustomer>();

        var query = new ListQueryRequest<DboCustomer>()
        {
            PageSize = request.PageSize,
            StartIndex = request.StartIndex,
            SortDescending = request.SortDescending,
            SortExpression = this.GetSorter(request.SortColumn),
            FilterExpression = this.GetFilter(request),
            Cancellation = cancellationToken
        };

        var result = await _broker.ExecuteAsync<DboCustomer>(query);

        if (!result.HasSucceeded(out ListResult<DboCustomer> listResult))
            return result.ConvertFail<ListResult<DmoCustomer>>();

        var list = listResult.Items.Select(item => DboCustomerMap.Map(item));

        return Result<ListResult<DmoCustomer>>.Success( new(list, listResult.TotalCount));
    }

    private Expression<Func<DboCustomer, object>> GetSorter(string? field)
        => field switch
        {
            AppDictionary.Customer.CustomerName => (Item) => Item.CustomerName,
            _ => (item) => item.CustomerID
        };

    // No Filter Defined
    private Expression<Func<DboCustomer, bool>>? GetFilter(CustomerListRequest request)
    {
        return null;
    }
}
```

## The Result

The data pipeline returns a `ListResult`.  This object contains the requested list of records and the total number of records in the query.

```csharp
public readonly record struct  ListResult<TRecord>(IEnumerable<TRecord> Items, int TotalCount);
```

This is wrapped in a `Result` object returnd by Mediator that contains the list result and status of the request.

### CQS Server Broker

The broker basic structure looks like this.  `TDbContext` defines the `DbContext` to obtain through the DbContext Factory service.   

```csharp
public sealed class ListRequestServerBroker<TDbContext>
    : IListRequestBroker
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public ListRequestServerBroker(IDbContextFactory<TDbContext> factory)
    {
        _factory = factory;
    }

    public async ValueTask<Result<ListResult<TRecord>>> ExecuteAsync<TRecord>(ListQueryRequest<TRecord> request)
        where TRecord : class
    {
        return await this.GetItemsAsync<TRecord>(request);
    }

    private async ValueTask<Result<ListResult<TRecord>>> GetItemsAsync<TRecord>(ListQueryRequest<TRecord> request)
        where TRecord : class
    {
    }
}
```

The default server method looks like this.  It gets a *unit of work* `DbContext` from the factory, turns off tracking [this is only a query] and gets the records through the `DbSet` in the `DbContext`.  It applied the filter, sorter and paging to the `IQueryable` before materializing the list and building the `ListResult` object.

```csharp
    private async ValueTask<Result<ListResult<TRecord>>> GetItemsAsync<TRecord>(ListQueryRequest<TRecord> request)
        where TRecord : class
    {
        int totalRecordCount = 0;

        // Get a Unit of Work DbContext for the scope of the method
        using var dbContext = _factory.CreateDbContext();
        // Turn off tracking.  We're only querying, no changes
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        // Get the IQueryable DbSet for TRecord
        IQueryable<TRecord> query = dbContext.Set<TRecord>();

        // If we have a filter defined, apply the predicate delegate to the IQueryable instance
        if (request.FilterExpression is not null)
            query = query.Where(request.FilterExpression).AsQueryable();

        // Get the total record count after applying the filters
        totalRecordCount = query is IAsyncEnumerable<TRecord>
            ? await query.CountAsync(request.Cancellation).ConfigureAwait(ConfigureAwaitOptions.None)
            : query.Count();

        // If we have a sorter, apply the sorter to the IQueryable instance
        if (request.SortExpression is not null)
        {
            query = request.SortDescending
                ? query.OrderByDescending(request.SortExpression)
                : query.OrderBy(request.SortExpression);
        }

        // Apply paging to the filtered and sorted IQueryable
        if (request.PageSize > 0)
            query = query
                .Skip(request.StartIndex)
                .Take(request.PageSize);

        // Finally materialize the list from the data source
        var list = query is IAsyncEnumerable<TRecord>
            ? await query.ToListAsync().ConfigureAwait(ConfigureAwaitOptions.None)
            : query.ToList();

        return Result<ListResult<TRecord>>.Success(new(list, totalRecordCount));
    }
```

## Service Registration

The broker is registered as a service:

```csharp
        services.AddScoped<IListRequestBroker, ListRequestServerBroker<InMemoryInvoiceTestDbContext>>();
```

Everything else is handled by Mediator.
