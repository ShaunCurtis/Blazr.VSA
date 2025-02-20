/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Presentation.Bootstrap;
using Blazr.App.Presentation;
using Microsoft.Extensions.DependencyInjection;
using Blazr.App.UI;

namespace Blazr.App.Infrastructure;

public static class InvoiceServices
{
    public static void AddInvoiceServices(this IServiceCollection services)
    {
        services.AddScoped<IEntityProvider<DmoInvoice, InvoiceId>, InvoiceEntityProvider>();
        services.AddSingleton<IUIEntityProvider<DmoInvoice>, InvoiceUIEntityProvider>();
        services.AddScoped<InvoiceAggregatePresenter>();

        services.AddTransient<IGridPresenter<DmoInvoice>, InvoiceGridPresenter>();
        services.AddTransient<IReadPresenter<DmoInvoice, InvoiceId>, ReadPresenter<DmoInvoice, InvoiceId>>();

        services.AddTransient<InvoiceEditPresenter>();
        services.AddTransient<InvoiceItemEditPresenterFactory>();

    }
}
