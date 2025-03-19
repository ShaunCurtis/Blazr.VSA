/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Infrastructure.Server;

public static class InvoiceApplicationServerServices
{
    public static void AddInvoiceAppServices(this IServiceCollection services)
    {
        // Add the InMemory Database
        services.AddDbContextFactory<InMemoryInvoiceTestDbContext>(options
            => options.UseInMemoryDatabase($"TestDatabase-{Guid.NewGuid().ToString()}"));

        // Add Custom Handlers
        services.AddScoped<ICommandBroker<InvoiceComposite>, InvoiceCommandServerBroker<InMemoryInvoiceTestDbContext>>();

        // Add any individual entity services
        services.AddCustomerServices();
        services.AddInvoiceServices();
    }

    public static void AddTestData(IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryInvoiceTestDbContext>>();

        if (factory is not null)
            InvoiceTestDataProvider.Instance().LoadDbContext<InMemoryInvoiceTestDbContext>(factory);
    }
}
