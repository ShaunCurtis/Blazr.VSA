/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.Core;

/// <summary>
/// This is an internal entity class that respreseents a DmoWeatherForecast record and its state
/// The exteenal readonly object is <see cref="WeatherForecastStateRecord"/>
/// </summary>
public sealed partial class WeatherForecastEntity
{
    private sealed class WeatherForecastContext
    {
        public CommandState State { get; set; }
            = CommandState.None;

        public DmoWeatherForecast Record { get; private set; }

        public bool IsDirty
            => this.State != CommandState.None;
        public WeatherForecastStateRecord AsRecord()
            => new(this.Record, this.State);

        public WeatherForecastContext(DmoWeatherForecast item, bool isNew = false)
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
}