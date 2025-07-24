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
        private DeleteWeatherForecastAction() { }

        public Result<WeatherForecastEntity> ExecuteAction(WeatherForecastEntity entity)
            => entity._weatherForecast
                .MarkAsDeleted(this.TransactionId)
                .ApplySideEffect(
                    hasNoException: () => entity.StateHasChanged?.Invoke(this, entity.WeatherForecast.Id),
                    hasException: ex => entity._weatherForecast.RollBackLastUpdate(this.TransactionId)
                    )
                .ApplyTransform<WeatherForecastEntity>(() => Result<WeatherForecastEntity>.Success(entity));

        public static DeleteWeatherForecastAction CreateAction()
            => new() { TransactionId = Guid.NewGuid() };
    }
}
