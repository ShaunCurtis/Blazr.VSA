/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;

namespace Blazr.App.Core;

public sealed class WeatherForecastEditContext : BaseRecordEditContext<DmoWeatherForecast, WeatherForecastId>, IRecordEditContext<DmoWeatherForecast>
{
    [TrackState] public string? Summary { get; set; }
    [TrackState] public decimal Temperature { get; set; }
    [TrackState] public DateTime? Date { get; set; }

    public WeatherForecastEditContext() : base() { }

    public WeatherForecastEditContext(DmoWeatherForecast record) : base(record) { }

    public override DmoWeatherForecast AsRecord =>
    this.BaseRecord with
    {
        Date = new(this.Date ?? DateTime.MinValue),
        Summary = this.Summary ?? string.Empty,
        Temperature = new(this.Temperature)
    };

    public override Result<DmoWeatherForecast> ToResult 
        => Result<DmoWeatherForecast>.Create(this.BaseRecord with
            {
                Date = new(this.Date ?? DateTime.MinValue),
                Summary = this.Summary ?? string.Empty,
                Temperature = new(this.Temperature)
            });

    public override Result Load(DmoWeatherForecast record)
    {
        if (!this.BaseRecord.Id.IsDefault)
            return Result.Failure("A record has already been loaded.  You can't overload it.");

        this.BaseRecord = record;

        this.Summary = record.Summary;
        this.Temperature = record.Temperature.TemperatureC;
        this.Date = record.Date.Value.ToDateTime(TimeOnly.MinValue);

        return  Result.Success();
    }
}
