/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;

namespace Blazr.App.Infrastructure.Server;

/// <summary>
/// Mediatr Handler for executing list requests against a WeatherForecast Entity
/// </summary>
public sealed class WeatherForecastListHandler : IRequestHandler<WeatherForecastListRequest, Result<ListItemsProvider<DmoWeatherForecast>>>
{
    private IListRequestBroker _broker;

    public WeatherForecastListHandler(IListRequestBroker broker)
    {
        this._broker = broker;
    }

    public async Task<Result<ListItemsProvider<DmoWeatherForecast>>> Handle(WeatherForecastListRequest request, CancellationToken cancellationToken)
    {
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

        var result = await _broker.ExecuteAsync<DvoWeatherForecast>(query);

        if (!result.HasSucceeded(out ListItemsProvider<DvoWeatherForecast>? listResult))
            return result.ConvertFail<ListItemsProvider<DmoWeatherForecast>>();

        var list = listResult.Items.Select(item => WeatherForecastMap.Map(item));

        return Result<ListItemsProvider<DmoWeatherForecast>>.Success(new(list, listResult.TotalCount));
    }

    private Expression<Func<DvoWeatherForecast, object>> GetSorter(string? field)
        => field switch
        {
            "Id" => (Item) => Item.WeatherForecastID,
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
