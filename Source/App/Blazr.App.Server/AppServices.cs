/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazored.Toast;
using Blazr.App.Invoice.Core;
using Blazr.App.Invoice.Infrastructure.Server;
using Blazr.App.Presentation;
using Blazr.App.Weather.Core;
using Blazr.Gallium;

namespace Blazr.App.Infrastructure.Server;

public static class ApplicationServerServices
{
    public static void AddAppServices(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                typeof(DmoCustomer).Assembly
                ,typeof(DmoWeatherForecast).Assembly
                ));

        // Add the Gallium Message Bus Server services
        services.AddScoped<IMessageBus, MessageBus>();

        // Add the Blazored Toast services
        services.AddBlazoredToast();

        // InMemory Scoped State Store 
        services.AddScoped<ScopedStateProvider>();

        // Presenter Factories
        services.AddScoped<ILookupUIBrokerFactory, LookupUIBrokerFactory>();
        services.AddScoped<IEditUIBrokerFactory, EditUIBrokerFactory>();
        services.AddTransient<IReadUIBrokerFactory, ReadUIBrokerFactory>();

        // Add the QuickGrid Entity Framework Adapter
        services.AddQuickGridEntityFrameworkAdapter();

        services.AddInvoiceAppServices();
        services.AddWeatherAppServices();
    }
}
