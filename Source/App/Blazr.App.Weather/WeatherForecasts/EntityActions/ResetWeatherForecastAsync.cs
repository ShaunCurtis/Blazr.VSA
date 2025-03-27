/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.Core;

public sealed partial class WeatherForecastEntity
{
    /// <summary>
    /// Resets the Weather Foerecast to the original state
    /// </summary>
    /// <returns></returns>
    public async ValueTask<Result> ResetWeatherForecastAsync()
    {
        _item.Update(_baseItem);
        _item.State = _baseState;

        await this.UpdatedAsync();

        return Result.Success();
    }
}
