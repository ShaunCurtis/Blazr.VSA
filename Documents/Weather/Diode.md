# Diode

**Diode** is a low ambition *No Frills* library for building *CQS* based data pipelines.

It's:
 - A set of interfaces, definitions and base implementations,
 - A *Mediator Pattern* dispatcher.
 - Built on the functional paradigm using the Result monad.

It consists of three channels or paths:

1. List Queries returning a collection of `TRecord`.
2. Record Queries returning a single `TRecord`.
3. Commands issuing *Create/Update/Delete* instructions.

## `Result<T>` and `Result`

All functions within the data pipeline return a `Result` or `Result<T>`.

You can read more about the `Result` monad in the [Result.md](Result.md) article.

## List Queries

A List Query is a request for a collection of `TRecord` objects defined as follows:

```csharp
public record ListQueryRequest<TRecord>
{
    public int StartIndex { get; init; }
    public int PageSize { get; init; }
    public Expression<Func<TRecord, bool>>? FilterExpression { get; init; }
    public Expression<Func<TRecord, object>>? SortExpression { get; init; }
    public bool SortDescending { get; init; } = true;
    public CancellationToken Cancellation { get; init; }

    public ListQueryRequest()
    {
        StartIndex = 0;
        PageSize = 1000;
        FilterExpression = null;
        SortExpression = null;
        Cancellation = new();
    }
}
```

Note the filter and sort fields are expressions that can be plugged directly into Linq.  The `StartIndex` and `PageSize` properties are used for pagination.

The server-side Entity Framework Handler for this request is defined in the `CQSEFBroker` static class as follows:

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

    return Result<ListItemsProvider<TRecord>>.Success(new ListItemsProvider<TRecord>(list, totalRecordCount));
}
```

It builds an `IQueryable<TRecord>` based on the request parameters, applies the filter and sort expressions, and then materializes the list with pagination.

It's attached to `DbContext` by an extension method:

```csharp

    public static async Task<Result<ListItemsProvider<TRecord>>> GetItemsAsync<TRecord>(this DbContext dbContext, ListQueryRequest<TRecord> request)
    where TRecord : class
    {
        return await CQSEFBroker<DbContext>.GetItemsAsync(dbContext, request);
    }
```

And accessed directly from a `DbContext` instance.
```csharp