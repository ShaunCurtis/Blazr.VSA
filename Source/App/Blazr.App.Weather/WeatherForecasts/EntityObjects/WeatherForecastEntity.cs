/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.Core;

public sealed partial class WeatherForecastEntity
{
    private readonly IMediator _mediator;
    private readonly WeatherForecast _item;
    private readonly DmoWeatherForecast _baseItem;
    private bool _processing;

    public WeatherForecastId Id => _item.Id;

    public WeatherForecastRecord WeatherForecastRecord
        => _item.AsRecord();

    public bool IsDirty => _item.IsDirty;

    public event EventHandler<WeatherForecastId>? StateHasChanged;

    public WeatherForecastEntity(IMediator mediator, DmoWeatherForecast item)
    {
        _mediator = mediator;
        _baseItem = item;
        // We create new records for the Invoice and InvoiceItems
        _item = new WeatherForecast(item);

        // Detect if the Invoice is a new record
        if (item.Id.IsDefault)
            _item.State = CommandState.Add;
    }

    private void Updated()
    {
        this.ApplyRules();
    }

    private void ApplyRules()
    {
        // prevent calling oneself
        if (_processing)
            return;

        _processing = true;

        // apply rules

        this.StateHasChanged?.Invoke(this, this.Id);

        _processing = false;
    }
}
