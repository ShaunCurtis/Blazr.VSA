/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly record struct WeatherForecastId(Guid Value) : IEntityId
{
    public bool IsDefault => this == Default;
    public static WeatherForecastId Create => new(Guid.CreateVersion7());
    public static WeatherForecastId Default => new(Guid.Empty);

    public WeatherForecastId ValidatedId => this.IsDefault ? Create : this;

    public Result<WeatherForecastId> ToResult() => Result<WeatherForecastId>.Create(this);

    public override string ToString()
    {
        if (Value == Guid.Empty)
            return "-- New Record --";

        // The lowest 8 numbers of the timestamp section
        //return value.ToString();
        return Value.ToString().Substring(6, 18).Replace("-", "");
    }
}
