/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.UI;

namespace Blazr.App.Core;

public sealed record WeatherForecastUIEntityProvider : IUIEntityProvider<DmoWeatherForecast>
{
    public string SingleDisplayName { get; } = "Weathjer Forecast";
    public string PluralDisplayName { get; } = "Weather Forecasts";
    public Type? EditForm { get; } = null;
    public Type? ViewForm { get; } = null;
    public string Url { get; } = "/weatherForecast";
}
