/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;
using Blazr.App.Core;
using Blazr.App.Invoice.Core;
using Blazr.App.Invoice.Infrastructure.Server;
using Blazr.App.Invoice.Presentation;
using Blazr.App.Invoice.UI;
using Blazr.App.Presentation;
using Blazr.App.UI;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Invoice.Infrastructure;

public static class InvoiceServices
{
    public static void AddInvoiceServices(this IServiceCollection services)
    {
        services.AddScoped<IEntityProvider<DmoInvoice, InvoiceId>, InvoiceEntityProvider>();
        services.AddSingleton<IUIEntityProvider<DmoInvoice>, InvoiceUIEntityProvider>();
        services.AddScoped<InvoiceCompositeBroker>();

        services.AddTransient<IGridUIBroker<DmoInvoice>, GridUIBroker<DmoInvoice, InvoiceId>>();
        services.AddTransient<IReadUIBroker<DmoInvoice, InvoiceId>, ReadUIBroker<DmoInvoice, InvoiceId>>();

        services.AddTransient<InvoiceEditBroker>();
        services.AddTransient<InvoiceItemEditBrokerFactory>();

        services.AddScoped<ICommandBroker<InvoiceComposite>, InvoiceCommandServerBroker<InMemoryInvoiceTestDbContext>>();
    }
}
