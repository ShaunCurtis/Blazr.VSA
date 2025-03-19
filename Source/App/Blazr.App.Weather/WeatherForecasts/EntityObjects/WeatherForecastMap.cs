/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Weather.Core;

namespace Blazr.App.Weather.Infrastructure;

public sealed class WeatherForecastMap 
{
    public static DmoWeatherForecast Map(DvoWeatherForecast item)
        => new()
        {
      Id = new(item.WeatherForecastID),
            Date = new(item.Date),
            OwnerId = new(item.OwnerID),
            Owner = item.Owner ?? "Not Defined",
            Temperature = new(item.Temperature),
            Summary = item.Summary ?? "Not Defined"
        };
    public static DmoWeatherForecast Map(DboWeatherForecast item)
        => new()
        {
            Id = new(item.WeatherForecastID),
            Date = new(item.Date),
            Temperature = new(item.Temperature),
            OwnerId = new(item.OwnerID),
            Owner = "Not Defined",
            Summary = item.Summary ?? "Not Defined"
        };

    public static DboWeatherForecast Map(DmoWeatherForecast item)
        => new()
        {
            WeatherForecastID = item.Id.Value,
            OwnerID = item.OwnerId.Value,
            Date = item.Date.Value.ToDateTime(TimeOnly.MinValue),
            Temperature = item.Temperature.TemperatureC,
            Summary = item.Summary
        };
}
