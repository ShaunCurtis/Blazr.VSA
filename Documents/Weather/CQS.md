# CQS and the Data Pipeline

The Blazor.Diode package provides the basic infrastructure for impkementing CQS.  You can read up about CQS elsewhere, so I'll assume you either alreeady know what CQS is, or have now acquainted yourself.

The data pipeline has three distinct pathways:

1. Commands - a Create/Update/Delete command to the datastore.
2. Item Query - a request for a single item based on their unique identifier
3. List Query - a request for a paged collection of items with optional sorting and filtering.

## Entities

This is the `DmoWeatherForecast` domain object:

> Dmo = Domain Objects

```csharp
public sealed record DmoWeatherForecast
{
    public WeatherForecastId Id { get; init; } = new(Guid.Empty);
    public string Owner { get; init; } = string.Empty;
    public Date Date { get; init; }
    public Temperature Temperature { get; init; }
    public string Summary { get; init; } = "Not Defined";
}
```

Note it's immutable and uses of value objects rather than primitiies.

Here are the two data store objects with built-in mappers.  This is **Command/Query Separation**, so there are two objects.

`DvoWeatherForecast` is the query object for getting data from the data store.

> Dvo = Data View Object.  Normally accessed through views in the data store

```csharp
public sealed record DvoWeatherForecast
{
    [Key] public Guid WeatherForecastID { get; init; } = Guid.Empty;
    public DateTime Date { get; init; }
    public decimal Temperature { get; set; }
    public string? Summary { get; set; }

    public static DmoWeatherForecast Map(DvoWeatherForecast item)
        => new()
        {
            Id = new(item.WeatherForecastID),
            Date = new(item.Date),
            Temperature = new(item.Temperature),
            Summary = item.Summary ?? "Not Defined"
        };
}
```

`DboWeatherForecast` is the command object for writing data back to the data store.

> Dbo = Database Object.  Normally direct table acceas for Insert/Update/Delete operations.

```csharp
public sealed record DboWeatherForecast : ICommandEntity
{
    [Key] public Guid WeatherForecastID { get; init; } = Guid.Empty;
    public DateTime Date { get; init; }
    public decimal Temperature { get; init; }
    public string? Summary { get; init; }

    public static DboWeatherForecast Map(DmoWeatherForecast item)
        => new()
        {
            WeatherForecastID = item.Id.ValidatedId.Value,
            Date = item.Date.Value.ToDateTime(TimeOnly.MinValue),
            Temperature = item.Temperature.TemperatureC,
            Summary = item.Summary
        };
}
```

## Mediator

*Blazr.Diode* contains a Mediator Pattern implementation with the necessary request and result objects.   

`IMedaitorBroker` defines the DI Mediator service interface: which defines a single `DispatchAsync` method.

```csharp
Task<TResponse> DispatchAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
```

### Mediator Requests

The Mediatr request for getting a record is defined as:

```csharp
public readonly record struct WeatherForecastRecordRequest(WeatherForecastId Id) 
    : IRequest<Result<DmoWeatherForecast>>;
```

Which is used like this:

```csharp
public async ValueTask<Result<DmoWeatherForecast>> GetRecordAsync(WeatherForecastId id)
{
    return await _mediator.DispatchAsync(new WeatherForecastRecordRequest(id));
}
```

## Mediator Handlers

The Mediator handler for `DmoWeatherForecast` is defined in *Blazr.App.EntityFramework*:

```csharp
public sealed class WeatherForecastRecordHandler : IRequestHandler<WeatherForecastRecordRequest, Result<DmoWeatherForecast>>
{
    private IDbContextFactory<InMemoryWeatherTestDbContext> _factory;

    public WeatherForecastRecordHandler(IDbContextFactory<InMemoryWeatherTestDbContext> dbContextFactory)
    {
        _factory = dbContextFactory;
    }

    public Task<Result<DmoWeatherForecast>> HandleAsync(WeatherForecastRecordRequest request, CancellationToken cancellationToken)
        => _factory.CreateDbContext()
                .GetRecordAsync<DvoWeatherForecast>(new RecordQueryRequest<DvoWeatherForecast>(item => item.WeatherForecastID == request.Id.Value))
                .ExecuteFunctionAsync(DvoWeatherForecast.Map);

}
```

It:

1. Gets a transaction based `DbContext` from the factory.
1. Executes the `GetRecordAsync` method on the context to get the `DvoWeatherForecast`.
1. Maps the `Dvo` to the `Dmo` object and returns it in a `Result<DmoWeatherForecast>`.

`GetRecordAsync`, along with `GetItemsAsync` and `ExecuteCommandAsync` are extension methods added to the `DbContext`.

This is `GetRecordAsync`:

```csharp
public static async Task<Result<TRecord>> GetRecordAsync<TRecord>(this DbContext dbContext, RecordQueryRequest<TRecord> request)
    where TRecord : class
        => await CQSEFBroker<DbContext>.GetRecordAsync(dbContext, request).ConfigureAwait(ConfigureAwaitOptions.None);
```

And this is `CQSEFBroker<DbContext>.GetRecordAsync`

```csharp
public static async Task<Result<TRecord>> GetRecordAsync<TRecord>(TDbContext dbContext, RecordQueryRequest<TRecord> request)
    where TRecord : class
{
    dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

    var record = await dbContext.Set<TRecord>()
        .FirstOrDefaultAsync(request.FindExpression)
        .ConfigureAwait(ConfigureAwaitOptions.None);

    if (record is null)
        return Result<TRecord>.Failure($"No record retrieved with the Key provided");

    return Result<TRecord>.Success(record);
}
```

