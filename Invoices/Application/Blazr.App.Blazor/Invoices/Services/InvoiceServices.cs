/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Presentation;
using Blazr.Cadmium.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App;

public static class InvoiceServices
{
    public static void AddInvoiceServices(this IServiceCollection services)
    {
        services.AddScoped<IUIConnector<DmoInvoice, InvoiceId>, InvoiceUIConnector>();
    }
}
