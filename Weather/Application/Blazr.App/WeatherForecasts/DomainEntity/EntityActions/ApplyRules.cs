/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed partial class WeatherForecastEntity
{
    private bool _processing;

    private Result ApplyRules(object? sender)
        => SetProcessing()
            .ApplyTransform(RunRules)
            .UpdateState(() => this.StateHasChanged?.Invoke(sender ?? this, this.Id)
        );

    private Result RunRules()
    {
        var result = NewDateNotInTheFutureRule();
        _processing = false;

        return result;
    }

    private Result SetProcessing()
    {
        var result = _processing
            ? Result.Failure("Rules already running.")
            : Result.Success();

        _processing = true;
        return result;
    }

    private Result NewDateNotInTheFutureRule()
        => (_weatherForecast.State == EditState.New && _weatherForecast.Record.Date.Value > DateOnly.FromDateTime(DateTime.Now))
            ? Result.Failure(new ValidationException("A new weather forecast must have a future date."))
            : Result.Success();
}
