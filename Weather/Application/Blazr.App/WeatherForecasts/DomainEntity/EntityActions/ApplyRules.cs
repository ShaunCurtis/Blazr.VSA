/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed partial class WeatherForecastEntity
{
    private bool _processing;

    private static Result<WeatherForecastEntity> ApplyRules(WeatherForecastEntity entity)
     => entity._processing
            ? Result<WeatherForecastEntity>.Failure("Rules already running.")
            : Result<WeatherForecastEntity>.Success(entity)
        .ExecuteAction((WeatherForecastEntity entity) => entity._processing = true)
        .ExecuteTransaction(NewDateNotInTheFutureRule);

    private static Result<WeatherForecastEntity> NewDateNotInTheFutureRule(WeatherForecastEntity entity)
        => (entity._weatherForecast.State == EditState.New && entity._weatherForecast.Record.Date.Value > DateOnly.FromDateTime(DateTime.Now))
            ? Result<WeatherForecastEntity>.Failure(new ValidationException("A new weather forecast must have a future date."))
            : Result<WeatherForecastEntity>.Success(entity);
}
