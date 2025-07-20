/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Presentation;
using Blazr.App.UI;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App;

public static class WeatherForecastServices
{
    public static void AddWeatherForecastServices(this IServiceCollection services)
    {
        services.AddScoped<IEntityProvider<DmoWeatherForecast, WeatherForecastId>, WeatherForecastEntityProvider>();
        services.AddScoped<IUIEntityProvider<DmoWeatherForecast, WeatherForecastId>, WeatherForecastUIEntityProvider>();
    }
}
