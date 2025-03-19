/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazored.Toast;
using Blazr.Antimony.Infrastructure.Server;
using Blazr.App.Core;
using Blazr.App.Presentation;
using Blazr.Gallium;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Infrastructure.Server;

public static class WeatherApplicationServerServices
{
    public static void AddWeatherForecastSharedAppServices(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                typeof(DmoWeatherForecast).Assembly
                ));

        // Add the Gallium Message Bus Server services
        services.AddScoped<IMessageBus, MessageBus>();

        // Add the Blazored Toast services
        services.AddBlazoredToast();

        // Add the standard Antimony Server handlers used by simple entities
        services.AddScoped<IListRequestBroker, ListRequestServerBroker<InMemoryWeatherTestDbContext>>();
        services.AddScoped<IRecordRequestBroker, RecordRequestServerBroker<InMemoryWeatherTestDbContext>>();
        services.AddScoped<ICommandBroker, CommandServerBroker<InMemoryWeatherTestDbContext>>();

        // InMemory Scoped State Store 
        services.AddScoped<ScopedStateProvider>();

        // Presenter Factories
        services.AddScoped<ILookupUIBrokerFactory, LookupUIBrokerFactory>();
        services.AddScoped<IEditUIBrokerFactory, EditUIBrokerFactory>();
        services.AddTransient<IReadUIBrokerFactory, ReadUIBrokerFactory>();

        // Add the QuickGrid Entity Framework Adapter
        services.AddQuickGridEntityFrameworkAdapter();
    }

    public static void AddWeatherAppServices(this IServiceCollection services)
    {
        // Add the InMemory Database
        services.AddDbContextFactory<InMemoryWeatherTestDbContext>(options
            => options.UseInMemoryDatabase($"TestDatabase-{Guid.NewGuid().ToString()}"));

        // Add any individual entity services
        services.AddWeatherForecastServices();
   }

    public static void AddTestData(IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryWeatherTestDbContext>>();

        if (factory is not null)
            WeatherTestDataProvider.Instance().LoadDbContext<InMemoryWeatherTestDbContext>(factory);
    }
}
