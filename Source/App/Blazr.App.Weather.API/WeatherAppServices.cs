/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Weather.API;

public static partial class WeatherApplicationServerServices
{
    public static void AddWeatherAppAPIServices(this IServiceCollection services)
    {
        // Add any individual entity services
        services.AddWeatherForecastServices();
    }
}
