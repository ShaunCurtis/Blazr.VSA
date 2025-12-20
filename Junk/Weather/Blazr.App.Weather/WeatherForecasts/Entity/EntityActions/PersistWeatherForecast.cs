/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Threading;
using static Blazr.App.Weather.Core.WeatherForecastActions;

namespace Blazr.App.Weather.Core;

public static partial class WeatherForecastActions
{
    public readonly record struct PersistWeatherForecastAction(DmoWeatherForecast? item = null)
    {
        public static PersistWeatherForecastAction Empty => new();
    }
}

public sealed partial class WeatherForecastEntity
{
    public async ValueTask<IResult> DispatchAsync(object? sender, PersistWeatherForecastAction action)
    {
        if (action.item is not null)
        {
            var updateResult = await this.UpdateWeatherForecastAsync(new(action.item));
            if (updateResult.IsFailure)
                return updateResult;
        }

        var result = await _mediator.Send(new WeatherForecastCommandRequest(_item.Record, _item.State));

        if (result.IsSuccess)
        {
            _item.State = CommandState.None;
            if (sender != this)
                this.StateHasChanged?.Invoke(sender, this.Id);
        }

        return result;
    }
}
