/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;

namespace Blazr.App.Infrastructure;

public sealed class WeatherForecastMap
{
    public static DmoWeatherForecast Map(DvoWeatherForecast item)
        => new()
        {
            Id = new(item.WeatherForecastID),
            Date = new(item.Date),
            Temperature = new(item.Temperature),
            Summary = item.Summary ?? "Not Defined"
        };

    public static DboWeatherForecast Map(DmoWeatherForecast item)
        => new()
        {
            WeatherForecastID = item.Id.ValidatedId.Value,
            Date = item.Date.Value.ToDateTime(TimeOnly.MinValue),
            Temperature = item.Temperature.TemperatureC,
            Summary = item.Summary
        };

    public static Result<DmoWeatherForecast> ExecuteFunction(DvoWeatherForecast item)
        => Result<DmoWeatherForecast>.Create(Map(item));

    public static Result<DboWeatherForecast> MapResult(DmoWeatherForecast item)
        => Result<DboWeatherForecast>.Create(Map(item));
}
