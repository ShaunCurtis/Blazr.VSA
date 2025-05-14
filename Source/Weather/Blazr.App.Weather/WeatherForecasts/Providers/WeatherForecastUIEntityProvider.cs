/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.UI;
using Blazr.App.Weather.Core;

namespace Blazr.App.Weather.UI;

public sealed record WeatherForecastUIEntityProvider : IUIEntityProvider<DmoWeatherForecast>
{
    public string SingleDisplayName { get; } = "Weather Forecast";
    public string PluralDisplayName { get; } = "Weather Forecasts";
    public Type? EditForm { get; } = typeof(WeatherForecastEditForm);
    public Type? ViewForm { get; } = typeof(WeatherForecastViewForm);
    public string Url { get; } = "/weatherForecast";
}
