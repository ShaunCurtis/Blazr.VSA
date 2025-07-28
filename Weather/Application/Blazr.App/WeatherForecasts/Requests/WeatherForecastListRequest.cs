/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public record WeatherForecastListRequest
    : BaseListRequest, IRequest<Result<ListItemsProvider<DmoWeatherForecast>>>
{
    public string? Summary { get; init; }

    public static Result<WeatherForecastListRequest> Create(GridState<DmoWeatherForecast> state)
        => Result<WeatherForecastListRequest>.Create(new WeatherForecastListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });
}
