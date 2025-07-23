/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public partial class WeatherForecastEntity
{
    public record MarkAsPersistedAction : BaseAction<MarkAsPersistedAction>
    {
        private MarkAsPersistedAction() { }

        public static MarkAsPersistedAction CreateAction()
            => new() { Sender = null };

        public MarkAsPersistedAction WithSender(object sender)
            => this with { Sender = sender };

        public Result ExecuteAction(WeatherForecastEntity entity)
            => entity._weatherForecast.MarkAsPersisted()
                .ApplySideEffect(() => entity.StateHasChanged?.Invoke(this.Sender, entity.Id));
    }
}
