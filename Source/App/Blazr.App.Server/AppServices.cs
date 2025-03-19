/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazored.Toast;
using Blazr.Antimony.Core;
using Blazr.Antimony.Infrastructure.Server;
using Blazr.App.Core;
using Blazr.App.Invoice.Core;
using Blazr.App.Invoice.Infrastructure;
using Blazr.App.Invoice.Infrastructure.Server;
using Blazr.App.Presentation;
//using Blazr.App.Weather.Core;
using Blazr.Gallium;
using Microsoft.EntityFrameworkCore;

namespace Blazr.App.Infrastructure.Server;

public static class ApplicationServerServices
{
    public static void AddAppServices(this IServiceCollection services)
    {
        // Add the InMemory Database
        services.AddDbContextFactory<InMemoryInvoiceTestDbContext>(options
            => options.UseInMemoryDatabase($"TestDatabase-{Guid.NewGuid().ToString()}"));

        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                typeof(DmoCustomer).Assembly
                //,typeof(DmoWeatherForecast).Assembly
                ));

        // Add the Gallium Message Bus Server services
        services.AddScoped<IMessageBus, MessageBus>();

        // Add the Blazored Toast services
        services.AddBlazoredToast();

        // Add the standard Antimony Server handlers used by simple entities
        services.AddScoped<IListRequestBroker, ListRequestServerBroker<InMemoryInvoiceTestDbContext>>();
        services.AddScoped<IRecordRequestBroker, RecordRequestServerBroker<InMemoryInvoiceTestDbContext>>();
        services.AddScoped<ICommandBroker, CommandServerBroker<InMemoryInvoiceTestDbContext>>();

        // InMemory Scoped State Store 
        services.AddScoped<ScopedStateProvider>();

        // Presenter Factories
        services.AddScoped<ILookupUIBrokerFactory, LookupUIBrokerFactory>();
        services.AddScoped<IEditUIBrokerFactory, EditUIBrokerFactory>();
        services.AddTransient<IReadUIBrokerFactory, ReadUIBrokerFactory>();

        // Add the QuickGrid Entity Framework Adapter
        services.AddQuickGridEntityFrameworkAdapter();

        // Add any individual entity services
        services.AddCustomerServices();
        services.AddInvoiceServices();
        //services.AddInvoiceItemInfrastructureServices();
    }

    public static void AddTestData(IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryInvoiceTestDbContext>>();

        if (factory is not null)
            InvoiceTestDataProvider.Instance().LoadDbContext<InMemoryInvoiceTestDbContext>(factory);
    }
}
