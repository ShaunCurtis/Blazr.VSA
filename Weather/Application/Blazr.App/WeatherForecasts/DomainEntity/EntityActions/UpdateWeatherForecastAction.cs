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

        private UpdateWeatherForecastAction() { }

        public Result<WeatherForecastEntity> ExecuteAction(WeatherForecastEntity entity)
            =>  entity._weatherForecast
                .Update(this.Item, this.TransactionId)
                .ExecuteFunction(() => entity.ApplyRules(this.Sender))
                .MutateState(
                    hasNoException: () => entity.StateHasChanged?.Invoke(this.Sender, this.Item.Id),
                    hasException: ex => entity._weatherForecast.RollBackLastUpdate(this.TransactionId)
                )
                .ExecuteFunction<WeatherForecastEntity>(() => Result<WeatherForecastEntity>.Success(entity));

        public static UpdateWeatherForecastAction CreateAction(DmoWeatherForecast item)
            => new() { Item = item, TransactionId = Guid.NewGuid() };
    }
}
