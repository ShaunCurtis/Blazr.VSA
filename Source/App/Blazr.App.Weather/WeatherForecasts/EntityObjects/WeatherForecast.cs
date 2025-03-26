/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.Core;

internal sealed class WeatherForecast
{
    public CommandState State { get; set; }
        = CommandState.None;

    public DmoWeatherForecast Record { get; private set; }

    public bool IsDirty
        => this.State != CommandState.None;

    public WeatherForecast(DmoWeatherForecast item, bool isNew = false)
    {
        this.Record = item;

        if (isNew || item.Id.IsDefault)
            this.State = CommandState.Add;
    }

    public WeatherForecastId Id => this.Record.Id;

    public void Update(DmoWeatherForecast item)
    {
        this.Record = item;
        this.State = this.State.AsDirty;
    }
}
