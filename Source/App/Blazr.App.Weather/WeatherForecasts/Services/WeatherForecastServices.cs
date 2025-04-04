﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Presentation;
using Blazr.App.UI;
using Blazr.App.Weather.Core;
using Blazr.App.Weather.Presentation;
using Blazr.App.Weather.UI;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Weather;

public static class WeatherForecastServices
{
    public static void AddWeatherForecastServices(this IServiceCollection services)
    {
        services.AddScoped<IEntityProvider<DmoWeatherForecast, WeatherForecastId>, WeatherForecastEntityProvider>();
        services.AddSingleton<IUIEntityProvider<DmoWeatherForecast>, WeatherForecastUIEntityProvider>();

        services.AddTransient<IGridUIBroker<DmoWeatherForecast>, GridUIBroker<DmoWeatherForecast, WeatherForecastId>>();
        services.AddTransient<IEditUIBroker<WeatherForecastEditContext, WeatherForecastId>, EditUIBroker<DmoWeatherForecast, WeatherForecastEditContext, WeatherForecastId>>();
        services.AddTransient<IReadUIBroker<DmoWeatherForecast, WeatherForecastId>, ReadUIBroker<DmoWeatherForecast, WeatherForecastId>>();
    }
}
