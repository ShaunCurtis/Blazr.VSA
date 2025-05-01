/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Mediator;

namespace Blazr.App.Weather.Core;

public readonly record struct WeatherForecastCommandRequest(DmoWeatherForecast Item, CommandState State)
    : IRequest<Result<WeatherForecastId>>;
