/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.EntityFramework;

public static class AppEFServerServices
{
    public static void AddAppEFServices(this IServiceCollection services)
    {
        // Add the InMemory Database
        services.AddDbContextFactory<InMemoryInvoiceTestDbContext>(options
            => options.UseInMemoryDatabase($"TestDatabase-{Guid.NewGuid().ToString()}"));
    }
    public static void AddInvoiceTestData(this IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryInvoiceTestDbContext>>();

        if (factory is not null)
            InvoiceTestDataProvider.Instance().LoadDbContext<InMemoryInvoiceTestDbContext>(factory);
    }
}
