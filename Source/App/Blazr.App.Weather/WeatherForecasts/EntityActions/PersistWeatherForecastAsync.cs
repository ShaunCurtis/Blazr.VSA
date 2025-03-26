/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.Core;

public sealed partial class WeatherForecastEntity
{
    /// <summary>
    /// Persists the Composite to the data store and sets it as saved.
    /// i.e. it sets the CommandState on all the internal items as none. 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<IResult> PersistWeatherForecastAsync(CancellationToken cancellationToken = new())
    {
        var result = await _mediator.Send(new WeatherForecastCommandRequest(_item.Record, _item.State), cancellationToken);

        if (result.IsSuccess)
            _item.State = CommandState.None;
        
        return result;
    }

    public async ValueTask<IResult> PersistWeatherForecastAsync(DmoWeatherForecast item, CancellationToken cancellationToken = new())
    {
        _item.Update(item);
        this.Updated();

        var result = await _mediator.Send(new WeatherForecastCommandRequest(_item.Record, _item.State), cancellationToken);

        if (result.IsSuccess)
            _item.State = CommandState.None;

        return result;
    }
}
