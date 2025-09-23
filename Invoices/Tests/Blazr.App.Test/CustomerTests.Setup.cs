/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.EntityFramework;
using Blazr.App.Infrastructure;
using Blazr.Cadmium.Presentation;
using Blazr.Diode.Mediator;
using Blazr.Gallium;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Blazr.Test;

public partial class CustomerTests
{
    private InvoiceTestDataProvider _testDataProvider;

    public CustomerTests()
        => _testDataProvider = InvoiceTestDataProvider.Instance();

    private ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();

        // Add Blazor Mediator Service
        services.AddMediator(new Assembly[] {
                typeof(Blazr.App.EntityFramework.AppServerServices).Assembly
        });

        // Add the Gallium Message Bus Server services
        services.AddScoped<IMessageBus, MessageBus>();

        // InMemory Scoped State Store 
        services.AddScoped<ScopedStateProvider>();

        // Presenter Factories
        services.AddScoped<ILookupUIBrokerFactory, LookupUIBrokerFactory>();

        services.AddAppEFServices();
        //        services.AddLogging(builder => builder.AddDebug());

        var provider = services.BuildServiceProvider();

        // get the DbContext factory and add the test data
        var factory = provider.GetService<IDbContextFactory<InMemoryWeatherTestDbContext>>();
        if (factory is not null)
            WeatherTestDataProvider.Instance().LoadDbContext<InMemoryWeatherTestDbContext>(factory);

        return provider!;
    }

    private DmoCustomer AsDmoCustomer(DboCustomer customer)
        => new DmoCustomer
        {
            Id = new CustomerId(customer.CustomerID),
            Name = new Title(customer.CustomerName ?? string.Empty)
        };

}
