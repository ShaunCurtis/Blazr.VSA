/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.Antimony.Mediator;

namespace Blazr.App.Weather.Core;

public record WeatherForecastListRequest
    : BaseListRequest, IRequest<Result<ListItemsProvider<DmoWeatherForecast>>>
{
    public string? Summary { get; init; }
}
