/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode;
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public readonly record struct WeatherForecastEntityCommandRequest(
        WeatherForecastEntity Item)
    : IRequest<Result<WeatherForecastId>>;
