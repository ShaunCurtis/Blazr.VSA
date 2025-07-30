# Data Pipeline List Queries

## Grid Form

Within Grid Forms you will find this:

```csharp
public ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync(GridItemsProviderRequest<TRecord> gridRequest)
    => new ValueTask<GridItemsProviderResult<TRecord>>(this.UIBroker.GetItemsAsync(gridRequest));
```

This is the entry point for fetching data in a Grid Form. It calls the `GetItemsAsync` method of the `GridUIBroker`, which is responsible for managing the state and fetching data.

The return is mapped in a `ValueTask` because that's what the QuickGrid controls expect.

## GridUIBroker

Within the the GridUIBroker:

```csharp
public Task<GridItemsProviderResult<TRecord>> GetItemsAsync(GridItemsProviderRequest<TRecord> gridRequest)
    => Result<UpdateGridRequest<TRecord>>
        .Create(UpdateGridRequest<TRecord>.Create(gridRequest))
        .Dispatch(this.DispatchGridStateChange)
        .ExecuteFunctionAsync(_entityProvider.GetItemsAsync)
        .MutateStateAsync((result) => this.LastResult = result)
        .OutputAsync(ExceptionOutput: (ex) => GridItemsProviderResult.From<TRecord>(new List<TRecord>(), 0));
```

Steps:
1. Create a Result of `UpdateGridRequest`
1. Dispatch the request to the dispatcher to update the stored grid state.
1. Call the `GetItemsAsync` method of the `IEntityProvider` to fetch the data.  This is where we switch to async behaviour.
1. Output the Result to `LastResult`.
1. Output the result as a `GridItemsProviderResult<TRecord>`.  If the result is in Exception, return an empty list with a count of 0.

## EntityProvider

Entity Providers are registered against an entity.  This is the WeatherForecast provider. 

Within the the EntityProvider:

```csharp
public Task<Result<GridItemsProviderResult<DmoWeatherForecast>>> GetItemsAsync(GridState<DmoWeatherForecast> state)
    => WeatherForecastListRequest.Create(state)
        .DispatchAsync((request) => _mediator.DispatchAsync(request))
        .ExecuteFunctionAsync((itemsProvider) => GridItemsProviderResult
            .From<DmoWeatherForecast>(itemsProvider.Items.ToList(), itemsProvider.TotalCount));
```

1. Creates a `WeatherForecastListRequest` from the current grid state.
1. Dispatches the request to the mediator.
1. functions the result into a `GridItemsProviderResult<DmoWeatherForecast>`.

## WeatherForecastListHandler

Handlers are specific to the record and data store.  This is the Entity Framework Handler for the WeatherForecast.

It injects the `IDbContextFactory` and gets a `DbContext` for the transaction.  It executes the `GetItemsAsync` extension method on the context.  Note it builds the Sort and Filter Predicate delegates from the fields within the `WeatherForecastListRequest`.

```csharp
public sealed class WeatherForecastListHandler : IRequestHandler<WeatherForecastListRequest, Result<ListItemsProvider<DmoWeatherForecast>>>
{
    private readonly IDbContextFactory<InMemoryWeatherTestDbContext> _factory;

    public WeatherForecastListHandler(IDbContextFactory<InMemoryWeatherTestDbContext> factory)
    {
        _factory = factory;
    }

    public Task<Result<ListItemsProvider<DmoWeatherForecast>>> HandleAsync(WeatherForecastListRequest request, CancellationToken cancellationToken)
        => _factory
            .CreateDbContext()
            .GetItemsAsync<DvoWeatherForecast>(
                new ListQueryRequest<DvoWeatherForecast>()
                {
                    PageSize = request.PageSize,
                    StartIndex = request.StartIndex,
                    SortDescending = request.SortDescending,
                    SortExpression = this.GetSorter(request.SortColumn),
                    FilterExpression = this.GetFilter(request),
                    Cancellation = cancellationToken
                }
            )
            .ExecuteFunctionAsync((provider) =>
                Result<ListItemsProvider<DmoWeatherForecast>>
                    .Create(new ListItemsProvider<DmoWeatherForecast>(
                        Items: provider.Items.Select(item => WeatherForecastMap.Map(item)),
                        TotalCount: provider.TotalCount))
            );

    private Expression<Func<DvoWeatherForecast, object>> GetSorter(string? field)
        => field switch
        {
            "Id" => (Item) => Item.WeatherForecastID,
            "ID" => (Item) => Item.WeatherForecastID,
            "Temperature" => (Item) => Item.Temperature,
            "Summary" => (Item) => Item.Summary ?? string.Empty,
            _ => (item) => item.Date
        };

    // No Filter Defined
    private Expression<Func<DvoWeatherForecast, bool>>? GetFilter(WeatherForecastListRequest request)
    {
        if (request.Summary is not null)
            return (item) => request.Summary.Equals(item.Summary);

        return null;
    }
}
```

## DBContext Extensions

The meat is defined in the static `CQSEFBroker` class: 

```csharp
    public static async Task<Result<TRecord>> ExecuteCommandAsync<TRecord>(TDbContext dbContext, CommandRequest<TRecord> request, CancellationToken cancellationToken = new())
        where TRecord : class
    {
        if ((request.Item is not ICommandEntity))
            return Result<TRecord>.Failure($"{request.Item.GetType().Name} Does not implement ICommandEntity and therefore you can't Update/Add/Delete it directly.");

        var stateRecord = request.Item;
        var result = 0;
        switch (request.State.Index)
        {
            case EditState.StateNewIndex:
                dbContext.Add<TRecord>(request.Item);
                result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

                return result == 1
                    ? Result<TRecord>.Success(request.Item)
                    : Result<TRecord>.Failure("Error adding Record");

            case EditState.StateDeletedIndex:
                dbContext.Remove<TRecord>(request.Item);
                result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

                return result == 1
                    ? Result<TRecord>.Success(request.Item)
                    : Result<TRecord>.Failure("Error deleting Record");

            case EditState.StateDirtyIndex:
                dbContext.Update<TRecord>(request.Item);
                result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

                return result == 1
                    ? Result<TRecord>.Success(request.Item)
                    : Result<TRecord>.Failure("Error saving Record");

            default:
                return Result<TRecord>.Failure("Nothing executed.  Unrecognised State.");
        }
    }
```

And attached to the `DbContext` with an extension method:

```csharp
    public static async Task<Result<ListItemsProvider<TRecord>>> GetItemsAsync<TRecord>(this DbContext dbContext, ListQueryRequest<TRecord> request)
        where TRecord : class
            => await CQSEFBroker<DbContext>.GetItemsAsync(dbContext, request).ConfigureAwait(ConfigureAwaitOptions.None);
```