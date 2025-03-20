/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Weather.Core;
using Blazr.App.Weather.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Blazr.Antimony.Infrastructure.Server;

namespace Blazr.App.Infrastructure.Server;

/// <summary>
/// Mediatr Handler for executing list requests against a WeatherForecast Entity
/// </summary>
public sealed class WeatherForecastListHandler : IRequestHandler<WeatherForecastListRequest, Result<ListItemsProvider<DmoWeatherForecast>>>
{
    private readonly IDbContextFactory<InMemoryWeatherTestDbContext> _factory;

    public WeatherForecastListHandler(IDbContextFactory<InMemoryWeatherTestDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Result<ListItemsProvider<DmoWeatherForecast>>> Handle(WeatherForecastListRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        IEnumerable<DmoWeatherForecast> forecasts = Enumerable.Empty<DmoWeatherForecast>();

        var query = new ListQueryRequest<DvoWeatherForecast>()
        {
            PageSize = request.PageSize,
            StartIndex = request.StartIndex,
            SortDescending = request.SortDescending,
            SortExpression = this.GetSorter(request.SortColumn),
            FilterExpression = this.GetFilter(request),
            Cancellation = cancellationToken
        };

        var result = await dbContext.GetItemsAsync<DvoWeatherForecast>(query);

        if (result.HasNotSucceeded(out ListItemsProvider<DvoWeatherForecast>? listResult))
            return result.ConvertFail<ListItemsProvider<DmoWeatherForecast>>();

        var list = listResult.Items.Select(item => WeatherForecastMap.Map(item));

        return Result<ListItemsProvider<DmoWeatherForecast>>.Success(new(list, listResult.TotalCount));
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
