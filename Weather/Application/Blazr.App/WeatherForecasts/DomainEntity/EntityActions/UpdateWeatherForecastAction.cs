/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

// Updates the WeatherForecast
// If the update fails, we roll back the last update and return the error
// If it succeeds, we apply the rules
// And if the rules fail, we roll back the update and return the error
public sealed partial class WeatherForecastEntity
{
    public record UpdateWeatherForecastAction : BaseAction<UpdateWeatherForecastAction>
    {
        public DmoWeatherForecast Item { get; private init; } = default!;

        public UpdateWeatherForecastAction(DmoWeatherForecast item)
            => this.Item = item;

        public Result<WeatherForecastEntity> ExecuteAction(WeatherForecastEntity entity)
            => entity._weatherForecast
            .Update(this.Item, this.TransactionId)
            .ToResult<WeatherForecastEntity>(entity)
            .ExecuteTransaction(WeatherForecastEntity.ApplyRules)
            .NotifyOnSuccess((entity) => entity.StateHasChanged?.Invoke(this.Sender, this.Item.Id))
            .RollbackOnFailure(() => entity._weatherForecast.RollBackLastUpdate(this.TransactionId));

        public static UpdateWeatherForecastAction CreateAction(DmoWeatherForecast item)
            => new(item) { TransactionId = Guid.NewGuid() };
    }
}
