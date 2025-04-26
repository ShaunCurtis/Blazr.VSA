/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.Core;

/// <summary>
/// This is the public entity readonly object that respreseents a DmoWeatherForecast record and its state
/// </summary>
/// <param name="Record"></param>
/// <param name="State"></param>
public record WeatherForecastStateRecord(DmoWeatherForecast Record, CommandState State)
{
    public bool IsDirty
        => this.State != CommandState.None;
}
