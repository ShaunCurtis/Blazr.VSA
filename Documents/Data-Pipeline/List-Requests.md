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

The UI request may take several forms, based on the UI grid control or table you use.  This needs to be translated in the Presentation layer into a Mediator request.

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

Each request is record specific.  The *Customer* list request looks like this:

```csharp
public record CustomerListRequest
    : BaseListRequest, IRequest<Result<ListResult<DmoCustomer>>>
{ }
```

Requests may contain additional information such as filter values.

This is a weather forecast request with a `Summary` filter:

```csharp
public record WeatherForecastListRequest
    : BaseListRequest, IRequest<Result<ListItemsProvider<DmoWeatherForecast>>>
{
    public string? Summary { get; init; }
}
```

## The Mediator Handler

The handler provides the interface into the data store.  It implements the `IRequestHandler` that takes an `IRequest` request and returns a `Result<ListResult<DmoCustomer>>`.  The handler is responsible for getting the data from the database and returning it in the requested format.

It builds a `ListQueryRequest` object from the request and passes it to the `GetItemsAsync` method on a `DbContext` instance.  This returns a `ListResult` object that is mapped to the `DmoCustomer` object and returned as a `Result<ListResult<DmoCustomer>>`.  The `GetSorter` and `GetFilter` methods build the `SortExpression` and `FilterExpression` objects from data provided in the list request.

```csharp
public sealed class CustomerListHandler : IRequestHandler<CustomerListRequest, Result<ListItemsProvider<DmoCustomer>>>
{
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public CustomerListHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Result<ListItemsProvider<DmoCustomer>>> HandleAsync(CustomerListRequest request, CancellationToken cancellationToken)
    {
        var dbContext = _factory.CreateDbContext();

        IEnumerable<DmoCustomer> forecasts = Enumerable.Empty<DmoCustomer>();

        var query = new ListQueryRequest<DvoCustomer>()
        {
            PageSize = request.PageSize,
            StartIndex = request.StartIndex,
            SortDescending = request.SortDescending,
            SortExpression = this.GetSorter(request.SortColumn),
            FilterExpression = this.GetFilter(request),
            Cancellation = cancellationToken
        };

        var result = await dbContext.GetItemsAsync<DvoCustomer>(query);

        if (!result.HasSucceeded(out ListItemsProvider<DvoCustomer>? listResult))
            return result.ConvertFail<ListItemsProvider<DmoCustomer>>();

        var list = listResult.Items.Select(item => item.ToDmo());

        return Result<ListItemsProvider<DmoCustomer>>.Success( new(list, listResult.TotalCount));
    }

    private Expression<Func<DvoCustomer, object>> GetSorter(string? field)
        => field switch
        {
            AppDictionary.Customer.CustomerName => (Item) => Item.CustomerName,
            _ => (item) => item.CustomerID
        };

    // No Filter Defined
    private Expression<Func<DvoCustomer, bool>>? GetFilter(CustomerListRequest request)
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

### CQS DBContext Extensions

The extension methods are defined in a static `public static class CQSEFBroker<TDbContext> where TDbContext : DbContext
` class.

The method builds a `IQueryable` object frtom the request to execute against the `DBContext`.  Once to retrieve the total number of rows and then a second time with the paging applied.

```csharp
    public static async ValueTask<Result<ListItemsProvider<TRecord>>> GetItemsAsync<TRecord>(TDbContext dbContext, ListQueryRequest<TRecord> request)
        where TRecord : class
    {
        int totalRecordCount;

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

        return Result<ListItemsProvider<TRecord>>.Success(new(list, totalRecordCount));
    }
```

The actual extension method is definedian a `DbContextExtensions` class:

```csharp
    public static async ValueTask<Result<ListItemsProvider<TRecord>>> GetItemsAsync<TRecord>(this DbContext dbContext, ListQueryRequest<TRecord> request)
    where TRecord : class
    {
        return await CQSEFBroker<DbContext>.GetItemsAsync(dbContext, request);
    }
```

## Service Registration

No objects are manually registered.  Everything is handled by through the Mediator implementation.
