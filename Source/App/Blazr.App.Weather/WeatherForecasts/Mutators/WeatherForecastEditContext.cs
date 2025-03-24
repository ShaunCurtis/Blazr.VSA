/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.App.Weather.Core;
using Blazr.EditStateTracker.Core;

namespace Blazr.App.Presentation;

public sealed class WeatherForecastEditContext : BaseRecordEditContext<DmoWeatherForecast, WeatherForecastId>, IRecordEditContext<DmoWeatherForecast>
{
    [TrackState] public string? Summary { get; set; }
    [TrackState] public decimal Temperature { get; set; }
    [TrackState] public DateTime? Date { get; set; }

    public override DmoWeatherForecast AsRecord =>
        this.BaseRecord with
        {
            Date = new(this.Date ?? DateTime.Now),
            Summary = this.Summary ?? "Not Set",
            Temperature = new(this.Temperature)
        };
    public WeatherForecastEditContext() : base() { }

    public WeatherForecastEditContext(DmoWeatherForecast record) : base(record) { }

    public override IResult Load(DmoWeatherForecast record)
    {
        if (!this.BaseRecord.Id.IsDefault)
            return Result.Failure("A record has already been loaded.  You can't overload it.");

        this.BaseRecord = record;
        this.Summary = record.Summary;
        this.Temperature = record.Temperature.TemperatureC;
        this.Date = record.Date.Value.ToDateTime(TimeOnly.MinValue);

        return Result.Success();
    }
}
