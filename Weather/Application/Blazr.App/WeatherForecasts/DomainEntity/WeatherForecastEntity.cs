/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

/// <summary>
/// The entity for the WeatherForecast domain object.
/// This provides the state management for the WeatherForecast domain object.
/// Obtain instances through the WeatherForecastEntityProvider.
/// </summary>
public sealed partial class WeatherForecastEntity
{
    private readonly EntityState<DmoWeatherForecast> _weatherForecast;

    public event EventHandler<WeatherForecastId>? StateHasChanged;

    public WeatherForecastId Id => _weatherForecast.Record.Id;

    public DmoWeatherForecast WeatherForecast => _weatherForecast.Record;
    public StateRecord<DmoWeatherForecast> WeatherForecastRecord => _weatherForecast.AsStateRecord;
    public bool IsDirty => _weatherForecast.IsDirty;

    public WeatherForecastEntity(DmoWeatherForecast weatherForecast, bool? isNew = null)
    {
        _weatherForecast = new(weatherForecast, isNew ?? weatherForecast.Id.IsDefault);
    }

    public Result<WeatherForecastEntity> AsResult
        => Result<WeatherForecastEntity>.Create(this);

    public static WeatherForecastEntity Create()
            => new WeatherForecastEntity(new DmoWeatherForecast { Id = WeatherForecastId.Create }, true);

    public static WeatherForecastEntity Load(DmoWeatherForecast weatherForecast)
            => new WeatherForecastEntity(weatherForecast);

    public static WeatherForecastEntity Load(DmoWeatherForecast weatherForecast, bool isNew)
            => new WeatherForecastEntity(weatherForecast, isNew);

    public void RaiseStateHasChanged(object? sender, WeatherForecastId id)
        => this.StateHasChanged?.Invoke(sender, id);
}
