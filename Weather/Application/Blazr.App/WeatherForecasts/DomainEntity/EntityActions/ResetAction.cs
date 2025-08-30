/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed partial class WeatherForecastEntity
{
    /// <summary>
    /// Resets the Weather Forecast to the original state
    /// </summary>
    /// <returns></returns>
    public record ResetAction : BaseAction<ResetAction>
    {
        public static ResetAction CreateAction()
            => new() { Sender = null, TransactionId = Guid.CreateVersion7() };

        public Result<WeatherForecastEntity> ExecuteAction(WeatherForecastEntity entity)
            => entity._weatherForecast.Reset()
            .ToResult<WeatherForecastEntity>(entity)
            .ExecuteTransaction(WeatherForecastEntity.ApplyRules)
            .NotifyOnSuccess((value) => entity.StateHasChanged?.Invoke(this.Sender, entity.Id))
            .RollbackOnFailure(() => entity._weatherForecast.RollBackLastUpdate(this.TransactionId));
    }
}
