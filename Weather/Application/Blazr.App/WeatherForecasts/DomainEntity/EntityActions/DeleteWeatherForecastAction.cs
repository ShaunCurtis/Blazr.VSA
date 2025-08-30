using Blazr.Diode;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

// Deletes the WeatherForecast
// If he delete fails, we roll back the last update and return the error
// And if the rules fail, we roll back the update and return the error
public sealed partial class WeatherForecastEntity
{
    public record DeleteWeatherForecastAction : BaseAction<DeleteWeatherForecastAction>
    {
        public Result<WeatherForecastEntity> ExecuteAction(WeatherForecastEntity entity)
            => entity._weatherForecast
            .MarkAsDeleted(this.TransactionId)
            .ToResult<WeatherForecastEntity>(entity)
            .NotifyOnSuccess((entity) => entity.StateHasChanged?.Invoke(this.Sender, entity.Id))
            .RollbackOnFailure(() => entity._weatherForecast.RollBackLastUpdate(this.TransactionId));

        public static DeleteWeatherForecastAction CreateAction()
            => new() { TransactionId = Guid.NewGuid() };
    }
}
