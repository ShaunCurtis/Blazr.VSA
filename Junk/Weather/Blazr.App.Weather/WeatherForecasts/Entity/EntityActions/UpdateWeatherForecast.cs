/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Weather.Core.WeatherForecastActions;

namespace Blazr.App.Weather.Core;

public static partial class WeatherForecastActions
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
    public async ValueTask<Result> DispatchAsync(object? sender, UpdateWeatherForecastAction action)
    {
        var result = await this.UpdateWeatherForecastAsync(action);

        if (result.IsSuccess && sender != this)
            this.StateHasChanged?.Invoke(sender, this.Id);

        return result;
    }

    private async ValueTask<Result> UpdateWeatherForecastAsync(UpdateWeatherForecastAction action)
    {
        var currentitem = _item.Record;

        _item.Update(action.Item);
        var result = await this.ApplyRulesAsync();

        if (result.IsSuccess)
            return Result.Success();

        //Update failed the rules so we need to roll back the change
        _item.Update(action.Item);
        return result;
    }
}
