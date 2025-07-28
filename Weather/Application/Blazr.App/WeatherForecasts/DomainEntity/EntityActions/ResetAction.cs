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
        private ResetAction() { }

        public static ResetAction CreateAction()
            => new() { Sender = null, TransactionId = Guid.CreateVersion7() };

        public Result ExecuteAction(WeatherForecastEntity entity)
            => entity._weatherForecast.Reset(entity._baseWeatherForecast)
                .ApplyTransform(() => entity.ApplyRules(this.Sender))
                .UpdateState(() => entity.StateHasChanged?.Invoke(this.Sender, entity.Id));
    }
}
