/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.Core;

public sealed partial class WeatherForecastEntity
{
    /// <summary>
    /// Resets the Weather Forecast to the original values
    /// </summary>
    /// <returns></returns>
    public Result ResetWeatherForecast()
    {
        _item.Update(_baseItem);

        this.Updated();

        return Result.Success();
    }
}
