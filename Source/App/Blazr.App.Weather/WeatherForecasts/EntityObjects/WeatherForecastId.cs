/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly record struct WeatherForecastId(Guid Value) : IEntityId
{
    public bool IsDefault => this == Default;
    public static WeatherForecastId Default => new(Guid.Empty);

    public override string ToString()
    {
        return Value.ToString();
    }
}
