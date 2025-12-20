/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
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
    public async ValueTask<IResult> DispatchAsync(PersistWeatherForecastAction action, CancellationToken cancellationToken = new())
    {
        if (action.item is not null)
        {
            _item.Update(action.item);
            await this.UpdatedAsync();
        }

        var result = await _mediator.Send(new WeatherForecastCommandRequest(_item.Record, _item.State), cancellationToken);

        if (result.IsSuccess)
            _item.State = CommandState.None;

        return result;
    }
}
