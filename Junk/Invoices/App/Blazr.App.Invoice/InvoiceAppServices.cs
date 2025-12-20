/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.App.Invoice.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Invoice.Infrastructure.Server;

public static class InvoiceApplicationServerServices
{
    public static void AddInvoiceAppServices(this IServiceCollection services)
    {
        // Add the InMemory Database
        services.AddDbContextFactory<InMemoryInvoiceTestDbContext>(options
            => options.UseInMemoryDatabase($"TestDatabase-{Guid.NewGuid().ToString()}"));

        // Add any individual entity services
        services.AddCustomerServices();
        services.AddInvoiceServices();
    }

    public static void AddInvoiceTestData(this IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryInvoiceTestDbContext>>();

        if (factory is not null)
            InvoiceTestDataProvider.Instance().LoadDbContext<InMemoryInvoiceTestDbContext>(factory);
    }
}
