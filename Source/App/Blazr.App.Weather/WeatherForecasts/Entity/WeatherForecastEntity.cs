/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Weather.Core.WeatherForecastActions;

namespace Blazr.App.Weather.Core;

public sealed partial class WeatherForecastEntity
{
    private readonly IMediatorBroker _mediator;
    private readonly WeatherForecastContext _item;
    private readonly DmoWeatherForecast _baseItem;
    private readonly CommandState _baseState;
    private bool _processing;

    public WeatherForecastId Id => _item.Id;

    public WeatherForecastStateRecord WeatherForecastRecord
        => _item.AsRecord();

    public bool IsDirty => _item.IsDirty;

    public event EventHandler<WeatherForecastId>? StateHasChanged;

    public WeatherForecastEntity(IMediatorBroker mediator, DmoWeatherForecast item)
    {
        _item = new WeatherForecastContext(item);

        // Detect if the Invoice is a new record
        if (item.Id.IsDefault)
            _item.State = CommandState.Add;

        _mediator = mediator;
        _baseItem = item;
        _baseState = _item.State;
    }

    /// <summary>
    /// Event handler for when the Weather Forecast is updated
    /// </summary>
    /// <param name="sender"></param>
    private async ValueTask UpdatedAsync(object? sender = null)
    {
        await this.DispatchAsync(new ApplyRulesAction(sender));
    }
}
