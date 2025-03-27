/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Weather.Core.InvoiceActions;

namespace Blazr.App.Weather.Core;

public static partial class InvoiceActions
{
    public readonly record struct UpdateWeatherForecastAction(DmoWeatherForecast Item);
}

public sealed partial class WeatherForecastEntity
{
    /// <summary>
    /// Updates the Weather Forecast record
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public async ValueTask<Result> DispatchAsync(UpdateWeatherForecastAction action)
    {
        _item.Update(action.Item);
        await this.UpdatedAsync();
        return Result.Success();
    }
}
