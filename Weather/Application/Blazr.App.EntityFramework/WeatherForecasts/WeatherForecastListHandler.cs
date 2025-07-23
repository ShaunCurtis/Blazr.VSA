/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.EntityFramework;

/// <summary>
/// Mediator Handler for executing list requests against a WeatherForecast Entity in a Entity Framework Context
/// </summary>
public sealed class WeatherForecastListHandler : IRequestHandler<WeatherForecastListRequest, Result<ListItemsProvider<DmoWeatherForecast>>>
{
    private readonly IDbContextFactory<InMemoryWeatherTestDbContext> _factory;

    public WeatherForecastListHandler(IDbContextFactory<InMemoryWeatherTestDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Result<ListItemsProvider<DmoWeatherForecast>>> HandleAsync(WeatherForecastListRequest request, CancellationToken cancellationToken)
    {
        var result = await _factory
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
            );

        return result.ApplyTransform<ListItemsProvider<DmoWeatherForecast>>(  
            mapping: items =>
            {
                var mappedItems = items.Items.Select(item => WeatherForecastMap.Map(item));
                return Result<ListItemsProvider<DmoWeatherForecast>>.Create(new ListItemsProvider<DmoWeatherForecast>(mappedItems, items.TotalCount));
            }
        );
    }

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
