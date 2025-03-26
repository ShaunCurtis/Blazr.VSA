/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;

namespace Blazr.App.Weather.Core;

public sealed partial class WeatherForecastEntity
{
    /// <summary>
    /// Resets the Invoice to the base Invoice
    /// </summary>
    /// <returns></returns>
    public Result ResetWeatherForecast()
    {

        _item.Equals ;

        this.Invoice.Update(_baseInvoice);

        return Result.Success();
    }
}
