#  Data Pipeline

The data pipeline is implemnted using several coding patterns and concepts:

1. *CQS*  
2. *Mediator*
3. *Records* i.e. *Read Only*


## The Implemntation

## Lists

UI grid and table forms have a variety of implementations using different request and result objects.  I'll use `QuickGrid` in this discussion.

The first step is to define a common dataset for the pipeline.  All the different UI results and requests will be translated into and out of these objects.

The `BaseListRequest` looks like this:

```csharp
public record BaseListRequest
{
    public int StartIndex { get; init; } = 0;
    public int PageSize { get; init; } = 1000;
    public string? SortColumn { get; init; } = null;
    public bool SortDescending { get; init; } = false;
}
```

And the result object:

```csharp
public record ListItemsProvider<TRecord>(IEnumerable<TRecord> Items, int TotalCount);
```

The `WeatherForecastRequest` can then be defined like this:

```csharp
public record WeatherForecastListRequest
    : BaseListRequest, IRequest<Result<ListItemsProvider<DmoWeatherForecast>>>
{
    public string? Summary { get; init; }
}
```

It defines an additional `Summary` property for filtering results.

Note the `IRequest` implementation for mapping the request to the correct handler.


