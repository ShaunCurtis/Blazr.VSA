/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Weather.Core.WeatherForecastActions;

namespace Blazr.App.Weather.Core;
public static partial class WeatherForecastActions
{
    public readonly record struct ResetWeatherForecastAction();
}

public sealed partial class WeatherForecastEntity
{
    /// <summary>
    /// Resets the Weather Foerecast to the original state
    /// </summary>
    /// <returns></returns>
    public async ValueTask<Result> DispatchAsync(object? sender, ResetWeatherForecastAction action )
    {
        var result = await this.ResetWeatherForecastAsync();

        if (sender != this)
            this.StateHasChanged?.Invoke(sender, this.Id);

        return Result.Success();
    }

    public async ValueTask<Result> ResetWeatherForecastAsync()
    {
        _item.Update(_baseItem);
        _item.State = _baseState;

        var result = await this.ApplyRulesAsync();

        return result;
    }
}
