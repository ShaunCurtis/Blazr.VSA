/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.EntityFramework;

public static partial class WeatherApplicationServerServices
{
    public static void AddWeatherAppEFServices(this IServiceCollection services)
    {
        // Add the InMemory Database
        services.AddDbContextFactory<InMemoryWeatherTestDbContext>(options
            => options.UseInMemoryDatabase($"TestDatabase-{Guid.NewGuid().ToString()}"));

        // Add any individual entity services
        services.AddWeatherForecastServices();
    }

    public static void AddWeatherTestData(this IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryWeatherTestDbContext>>();

        if (factory is not null)
            WeatherTestDataProvider.Instance().LoadDbContext<InMemoryWeatherTestDbContext>(factory);
    }
}
